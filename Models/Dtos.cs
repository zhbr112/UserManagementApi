using System.ComponentModel.DataAnnotations;

namespace UserManagementApi;

public class UserDto
{
    public Guid Id { get; set; }
    public string Login { get; set; }
    public string Name { get; set; }
    public int Gender { get; set; }
    public DateTime? Birthday { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime CreatedOn { get; set; }
    public bool IsActive { get; set; }
}

public class UserInfoDto
{
    public string Name { get; set; }
    public string Gender { get; set; }
    public DateTime? Birthday { get; set; }
    public string Status { get; set; }
}

public class UserProfileDto
{
    public string Login { get; set; }
    public string Name { get; set; }
    public string Gender { get; set; }
    public DateTime? Birthday { get; set; }
    public bool IsAdmin { get; set; }
}

public class AuthResponse
{
    public string Token { get; set; }
    public AuthUserDto User { get; set; }
}

public class AuthUserDto
{
    public string Login { get; set; }
    public string Name { get; set; }
    public bool IsAdmin { get; set; }
}

public class UserCreateDto
{
    [Required]
    public string Login { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public int Gender { get; set; }
    public DateTime? Birthday { get; set; }
    public bool IsAdmin { get; set; }
}

public class UserDetailsUpdateDto
{
    [Required]
    public string Name { get; set; }
    [Required]
    public int Gender { get; set; }
    public DateTime? Birthday { get; set; }
}

public class PasswordUpdateDto
{
    [Required] public string NewPassword { get; set; }
}

public class LoginUpdateDto
{
    [Required] public string NewLogin { get; set; }
}

public class AuthenticateRequest
{
    [Required] public string Login { get; set; }
    [Required] public string Password { get; set; }
}