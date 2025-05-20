using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementApi;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] AuthenticateRequest request)
    {
        var response = await _authService.AuthenticateAsync(request);
        return Ok(response);
    }
}