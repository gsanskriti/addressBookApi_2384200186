using AddressBookAPI.BusinessLayer.Interface;
using AddressBookAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTOs;
using System.Threading.Tasks;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// REGISTER USER
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDTO model)
    {
        var result = await _authService.RegisterAsync(model);
        if (!result) return BadRequest("User already exists!");
        return Ok("User registered successfully!");
    }


    /// <summary>
    /// LOGIN USER
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDTO model)
    {
        var token = await _authService.LoginAsync(model);
        if (token == null) return Unauthorized("Invalid credentials!");
        return Ok(new { Token = token });
    }


    /// <summary>
    /// user can get forget password option 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
    {
        var result = await _authService.ForgotPasswordAsync(model);
        if (!result) return BadRequest("Email not found!");
        return Ok("Password reset email sent.");
    }


    /// <summary>
    /// user can reset its password
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
    {
        var result = await _authService.ResetPasswordAsync(model);
        if (!result) return BadRequest("Invalid token or expired!");
        return Ok("Password reset successful.");
    }
}
