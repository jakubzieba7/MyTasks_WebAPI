using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MyTasks_WebAPI.Models;
using MyTasks_WebAPI.Models.Data;
using MyTasks_WebAPI.Models.Domains;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Security.Claims;

namespace MyTasks_WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<UnitOfWork, UnitOfWork>();

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
            builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme).AddIdentityCookies();
            builder.Services.AddAuthorizationBuilder();
            
            //builder.Services.AddAuthorization();
            //builder.Services.AddIdentityApiEndpoints<ApplicationUser>().AddEntityFrameworkStores<ApplicationDbContext>().AddApiEndpoints();

            builder.Services.AddIdentityCore<ApplicationUser>().AddEntityFrameworkStores<ApplicationDbContext>().AddApiEndpoints();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapIdentityApi<ApplicationUser>();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapGet("/test",(ClaimsPrincipal user)=>$"Zalogowany u¿ytkownik to {user.Identity!.Name}").RequireAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
