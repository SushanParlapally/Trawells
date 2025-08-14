using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TravelDesk.Data;
using TravelDesk.Models;
using TravelDesk.ViewModel;


namespace TravelDesk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class LoginController : ControllerBase
    {
        private readonly TravelDeskContext _context;
        private readonly IConfiguration _configuration;

    

    public LoginController(TravelDeskContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;

    }
    [HttpPost]
    public IActionResult Login(LoginModel loginModel)
    {
        try
        {
            Console.WriteLine($"Login attempt for email: {loginModel?.Email}");
            
            if (loginModel == null || string.IsNullOrEmpty(loginModel.Email) || string.IsNullOrEmpty(loginModel.Password))
            {
                Console.WriteLine("Invalid login model - missing email or password");
                return BadRequest("Email and password are required.");
            }

            IActionResult response = Unauthorized();

            // Authenticate the user
            var user = Authenticate(loginModel);

            if (user == null)
            {
                Console.WriteLine($"Authentication failed for email: {loginModel.Email}");
                return Unauthorized("Invalid email or password.");
            }
            
            Console.WriteLine($"User authenticated successfully: {user.Email}");

            // Retrieve the role from Role table based on RoleId
            var role = _context.Roles.FirstOrDefault(x => x.RoleId == user.RoleId);
            string roleName = role?.RoleName ?? "Employee";

        if (roleName == null)
        {
            return Unauthorized("Role not found.");
        }

            loginModel.RoleName = roleName;

            // Generate JWT token
            var tokenString = GenerateJSONWebToken(user, roleName);
            //var tokenString = GenerateJSONWebToken(user.UserId, user.Email, loginModel.RoleName);
          

            // Return the token, user details, and employee details
            response = Ok(new
            {
                token = tokenString
                //employeeId = user.UserId, // for employee Autofill
                //firstName = user.FirstName,
                //lastName = user.LastName,
                //department = user.Department,
                //roleName = loginModel.RoleName,
                //roleId = user.RoleId,
                //employees = employeeDetails // All employee details
            });

            Console.WriteLine($"Login successful for user: {user.Email}, role: {roleName}");
            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return StatusCode(500, "Internal server error during login");
        }
        }

    private string GenerateJSONWebToken(User user,String role)
    {
        var claims = new[]
        {
           // new claim(jwtregisteredclaimnames.jti, guid.newguid().tostring()),
           
            //new claim(jwtregisteredclaimnames.sid, user.id.tostring()),
            new Claim("Email", user.Email ?? string.Empty),

           new Claim("userid",user.UserId.ToString()),
           new Claim("firstname",user.FirstName ?? string.Empty),
              new Claim("lastname",user.LastName ?? string.Empty),
              new Claim("departmentid",user.DepartmentId.ToString()),
            new Claim("role", role ?? string.Empty),
            new Claim("managerid",user.ManagerId?.ToString() ?? "0"), 
            new Claim("roleId",user.RoleId.ToString()) ,
           

            //new Claim("date", datetime.now.tostring())
        };

        var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? _configuration["Jwt:Key"] ?? "default-secret-key-for-development-only";
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Issuer"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private User? Authenticate(LoginModel loginModel)
    {
        return _context.Users.FirstOrDefault(x => x.Email == loginModel.Email && x.Password == loginModel.Password);
    }
}
}
