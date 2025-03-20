using AddressBookAPI.BusinessLayer.Interface;
using AddressBookAPI.Models.DTOs;
using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTOs;
using System.Threading.Tasks;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserBL _userBL;

    public AuthController(IUserBL userBL)
    {
        _userBL = userBL;
    }

    /// <summary>
    /// REGISTER USER
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDTO model)
    {
        var result = await _userBL.RegisterUser(model);

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
        var token = await _userBL.LoginUser(model);
        if (token == null) return Unauthorized("Invalid credentials!");
        return Ok(new { Token = token });
    }

    /// <summary>
    /// User can request a password reset link
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
    {
        var result = await _userBL.ForgotPassword(model.Email);
        if (!result) return BadRequest("Email not found!");
        return Ok("Password reset email sent.");
    }

    /// <summary>
    /// User can reset their password using the reset token
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
    {
        var result = await _userBL.ResetPassword(model.Email, model.Token, model.NewPassword);
        if (!result) return BadRequest("Invalid token or expired!");
        return Ok("Password reset successful.");
    }
}
