using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using WebApi.Application.Services;

namespace WebApi.Controllers.v1
{
    [ApiVersion(1.0)]
    [ApiController]
    [Route("api/v{version:apiVersion}/auth")]
    public class AuthController : Controller
    {
        [HttpPost]
        public IActionResult Auth([FromBody] LoginModel login)
        {
            if (login.Username == "admin" && login.Password == "admin123")
            {
                var token = TokenService.GenerateToken(new Domain.Model.EmployeeAggregate.Employee("employeeTest", 99, "", ""));
                return Ok(token);
            }

            return BadRequest("username or password invalid");
        }
        public class LoginModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}