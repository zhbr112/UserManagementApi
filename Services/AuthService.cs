using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UserManagementApi;

public class AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IConfiguration configuration) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IConfiguration _configuration = configuration;

    public async Task<AuthResponse> AuthenticateAsync(AuthenticateRequest request)
    {
        var user = await _userRepository.GetByLoginAsync(request.Login);

        if (user == null || !_passwordHasher.VerifyPassword(user.Password, request.Password) || !user.IsActive)
            throw new UnauthorizedAccessException("Неверный логин или пароль");

        var token = GenerateJwtToken(user);
        return new AuthResponse { Token = token, User = MapToAuthUserDto(user) };
    }

    public string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Login),
            new Claim(ClaimTypes.Role, user.Admin ? "Admin" : "User")
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static AuthUserDto MapToAuthUserDto(User user)
    {
        return new AuthUserDto
        {
            Login = user.Login,
            Name = user.Name,
            IsAdmin = user.Admin
        };
    }
}