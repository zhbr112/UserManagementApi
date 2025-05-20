using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementApi;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(IUserService userService, ILogger<UsersController> logger) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly ILogger<UsersController> _logger = logger;

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> Create([FromBody] UserCreateDto userDto)
    {
        var currentUserLogin = User.Identity?.Name;
        var user = await _userService.CreateUserAsync(userDto, currentUserLogin);
        return Ok(user);
    }

    [HttpPut("{login}/details")]
    public async Task<ActionResult<UserDto>> UpdateUserDetails(string login, [FromBody] UserDetailsUpdateDto detailsDto)
    {
        var currentUserLogin = User.Identity?.Name;
        var user = await _userService.UpdateUserDetailsAsync(login, detailsDto, currentUserLogin);
        return Ok(user);
    }

    [HttpPut("{login}/password")]
    public async Task<IActionResult> UpdatePassword(string login, [FromBody] PasswordUpdateDto passwordDto)
    {
        var currentUserLogin = User.Identity?.Name;
        await _userService.ChangePasswordAsync(login, passwordDto, currentUserLogin);
        return Ok();
    }

    [HttpPut("{login}/change-login")]
    public async Task<IActionResult> UpdateLogin(string login, [FromBody] LoginUpdateDto loginDto)
    {
        var currentUserLogin = User.Identity?.Name;
        await _userService.ChangeLoginAsync(login, loginDto, currentUserLogin);
        return Ok();
    }

    [HttpGet("active")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllActive()
    {
        var users = await _userService.GetAllActiveUsersAsync();
        return Ok(users);
    }

    [HttpGet("{login}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserInfoDto>> GetByLogin(string login)
    {
        var user = await _userService.GetUserByLoginAsync(login);
        return Ok(user);
    }

    [HttpPost("authenticate")]
    [AllowAnonymous]
    public async Task<ActionResult<UserProfileDto>> Authenticate([FromBody] AuthenticateRequest request)
    {
        var user = await _userService.AuthenticateAsync(request);
        return Ok(user);
    }

    [HttpGet("older-than/{age}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetOlderThan(int age)
    {
        var users = await _userService.GetUsersOlderThanAsync(age);
        return Ok(users);
    }

    [HttpDelete("{login}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string login, [FromQuery] bool softDelete = true)
    {
        var currentUserLogin = User.Identity?.Name;
        await _userService.DeleteUserAsync(login, currentUserLogin, softDelete);
        return Ok();
    }

    [HttpPut("{login}/restore")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Restore(string login)
    {
        await _userService.RestoreUserAsync(login);
        return Ok();
    }
}