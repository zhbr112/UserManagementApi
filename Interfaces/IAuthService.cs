namespace UserManagementApi;

public interface IAuthService
{
    Task<AuthResponse> AuthenticateAsync(AuthenticateRequest request);
    string GenerateJwtToken(User user);
}