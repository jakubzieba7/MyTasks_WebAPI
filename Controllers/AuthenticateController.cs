using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyTasks_WebAPI.Models;
using MyTasks_WebAPI.Models.Authentication;
using MyTasks_WebAPI.Models.Domains;
using MyTasks_WebAPI.Models.Response;

namespace MyTasks_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly TokenService _tokenService;
        private readonly UnitOfWork _unitOfWork;

        public AuthenticateController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, TokenService tokenService, UnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Login with creation JWT Bearer
        /// </summary>
        /// <param name="model">UserName and Password from LoginModel</param>
        /// <returns>OkObjectResult with JWT Bearer or UnathorizedResult</returns>
        /// <response code="200">Returns the newly created user</response>
        /// <response code="401">Returns unauthorized when no user was found or password was incorrect</response>
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var token = _tokenService.GenerateToken(user, userRoles);

                return Ok(new
                {
                    bearer_JWT = token,
                    user = user,
                    Role = userRoles,
                    status = "User logged-in successfully"
                });
            }

            return Unauthorized();
        }

        /// <summary>
        /// Register new user for roles authentication purposes
        /// </summary>
        /// <param name="model">UserName, Email, Password, UserRole from RegisterModel</param>
        /// <returns>Status500InternalServerError or OkObjectResult</returns>
        /// <remarks></remarks>
        /// <response code="200">Success</response>
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var IsExist = await _userManager.FindByNameAsync(model.UserName);

            if (IsExist != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            ApplicationUser appUser = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                PasswordHash = model.Password,
            };

            _unitOfWork.CategoryRepository.AddDefaultCategory(appUser.Id);

            var result = await _userManager.CreateAsync(appUser, model.Password);

            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await _roleManager.RoleExistsAsync(model.UserRole))
                await _roleManager.CreateAsync(new IdentityRole(model.UserRole));

            if (await _roleManager.RoleExistsAsync(model.UserRole))
            {
                await _userManager.AddToRoleAsync(appUser, model.UserRole);
            }

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

    }
}
