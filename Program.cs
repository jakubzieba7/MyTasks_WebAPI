using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyTasks_WebAPI.Models;
using MyTasks_WebAPI.Models.Data;
using MyTasks_WebAPI.Models.Domains;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System.Configuration;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace MyTasks_WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var isJWTBearerTokenActive = bool.Parse(builder.Configuration["AuthenticationSettings:IsJWTBearerTokenActive"]);
            var isSPABearerTokenActive = bool.Parse(builder.Configuration["AuthenticationSettings:IsSPABearerTokenActive"]);
            var isUsingCookies = bool.Parse(builder.Configuration["SPABearerTokenOptions:IsUsingCookies"]);

            builder.Services.AddScoped<UnitOfWork, UnitOfWork>();

            if (isJWTBearerTokenActive)
                builder.Services.AddScoped<TokenService, TokenService>();               //JWT

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                if (isSPABearerTokenActive)
                {
                    options.AddSecurityDefinition("Bearer Token", new OpenApiSecurityScheme()   //Bearer tokens and cookies
                    {
                        In = ParameterLocation.Header,
                        Name = "Authorization Bearer Token",
                        Type = SecuritySchemeType.ApiKey
                    });
                }

                if (isJWTBearerTokenActive)
                {
                    options.AddSecurityDefinition("JWT Bearer", new OpenApiSecurityScheme() //JWT
                    {
                        Name = "Authorization JWT Bearer",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                    });

                    options.AddSecurityRequirement(new OpenApiSecurityRequirement       //JWT
                    {
                      {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                      }
                    });
                }
                options.OperationFilter<SecurityRequirementsOperationFilter>();

                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "MyTasks API",
                    Description = "An ASP.NET Core Web API for managing MyTasks items",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Example Contact",
                        Url = new Uri("https://example.com/contact")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Example License",
                        Url = new Uri("https://example.com/license")
                    }
                });

                // using System.Reflection;
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            if (isSPABearerTokenActive && isUsingCookies)
                builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme).AddIdentityCookies();       //Bearer tokens and cookies
            else
                builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);                    //Bearer tokens and cookies

            if (isJWTBearerTokenActive)
            {
                builder.Services.AddIdentity<ApplicationUser, IdentityRole>()                                           //JWT
                               .AddEntityFrameworkStores<ApplicationDbContext>();
                //.AddDefaultTokenProviders();

                // Adding Authentication
                builder.Services
                    .AddAuthentication(options =>                                                           //JWT
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    })

                    .AddJwtBearer(options =>                                                                                 //JWT                           
                    {
                        options.SaveToken = true;
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters()
                        {
                            ValidateIssuerSigningKey = bool.Parse(builder.Configuration["JWTSettings:ValidateIssuerSigningKey"]),
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTSettings:IssuerSigningKey"])),
                            ValidateIssuer = bool.Parse(builder.Configuration["JWTSettings:ValidateIssuer"]),
                            ValidAudience = builder.Configuration["JWTSettings:ValidAudience"],
                            ValidIssuer = builder.Configuration["JWTSettings:ValidIssuer"],
                            ValidateAudience = bool.Parse(builder.Configuration["JWTSettings:ValidateAudience"]),
                            RequireExpirationTime = bool.Parse(builder.Configuration["JWTSettings:RequireExpirationTime"]),
                            ValidateLifetime = bool.Parse(builder.Configuration["JWTSettings:ValidateLifetime"])
                        };
                    });
            }
            
            builder.Services.AddAuthorizationBuilder();
            
            Log.Logger=new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();

            builder.Host.UseSerilog();

            if (isSPABearerTokenActive)
                builder.Services.AddIdentityCore<ApplicationUser>().AddEntityFrameworkStores<ApplicationDbContext>().AddApiEndpoints();     //Bearer tokens and cookies

            var app = builder.Build();

            app.UseSerilogRequestLogging();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //adds the Identity endpoints
            if (isSPABearerTokenActive)
                app.MapIdentityApi<ApplicationUser>().WithTags("Identity Bearer Token");                                                    //Bearer tokens and cookies

            app.UseHttpsRedirection();
            
            //app.UseAuthentication();

            app.UseAuthorization();

            app.MapGet("/userInfo", (ClaimsPrincipal user) => $"Zalogowany u¿ytkownik to {user.Identity!.Name}").RequireAuthorization().WithTags("Check User info");

            app.MapControllers();

            app.Run();
        }
    }
}
