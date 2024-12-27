using Microsoft.AspNetCore.Mvc;
using EmploymentSystemProject.Entities;
using EmploymentSystemProject.DSL;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly LoginDSL _loginService;

    public LoginController(LoginDSL loginService)
    {
        _loginService = loginService;
    }
    [AllowAnonymous]
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] UserLoginDTO registrationDTO)
    {
        await _loginService.RegisterAsync(registrationDTO);
        return Ok("Registration successful.");
    }
    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDTO loginDTO)
    {
        var token = await _loginService.LoginAsync(loginDTO);
        return Ok(new { Token = token });
    }
}
