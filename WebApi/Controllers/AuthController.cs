using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : Controller
    {
        [HttpPost]
        public IActionResult Auth(string username, string password)
        {
            if (username == "admin" && password == "admin123")
            {
                var token = TokenService.GenerateToken(new Model.Employee("employeeTest", 99, ""));
                return Ok(token);
            }

            return BadRequest("username or password invalid");
        }
    }
}