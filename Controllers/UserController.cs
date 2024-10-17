using computerapi.DTO;
using computerapi.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace computerapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var newUser = new ApplicationUser()
            {
                Email = registerDto.Email,
                PasswordHash = registerDto.Password,
                UserName = registerDto.Email?.Split('@')[0]
            };

            var user = await userManager.FindByEmailAsync(registerDto.Email);
            if (user is not null) return BadRequest("User already exists!");

            var createUser = await userManager.CreateAsync(newUser, registerDto.Password);
            //var checkAdmin = await roleManager.FindByNameAsync("Admin");
            //if (checkAdmin is null)
            //{
            //    await roleManager.CreateAsync(new IdentityRole() { Name = "Admin" });
            //    await userManager.AddToRoleAsync(newUser, "Admin");
            //    return Ok("Admin account has been created.");
            //}

            //var checkUser = await roleManager.FindByNameAsync("User");
            //if (checkUser is null)
            //{
            //    await roleManager.CreateAsync(new IdentityRole() { Name = "User" });
            //}

            //await userManager.AddToRoleAsync(newUser, "User");
            return Ok("User created!");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (loginDto == null) return BadRequest("Please fill the empty parameters");

            var user = await userManager.FindByNameAsync(loginDto.Username!);
            Console.WriteLine(user);
            if (user is null) return NotFound("User not exists!");

            bool checkUserPassword = await userManager.CheckPasswordAsync(user, loginDto.Password!);
            if (!checkUserPassword) return BadRequest("Incorrenct credentials");

            var userRole = await userManager.GetRolesAsync(user);
            string token = GenerateToken(user.Id, user.UserName!, user.Email!, userRole.First());

            return Ok(token);
        }

        private string GenerateToken(string id, string name, string email, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };
            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
