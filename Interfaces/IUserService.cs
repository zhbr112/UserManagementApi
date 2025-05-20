namespace UserManagementApi;

public interface IUserService
{
    Task<UserDto> CreateUserAsync(UserCreateDto userDto, string currentUserLogin);
    Task<UserDto> UpdateUserDetailsAsync(string login, UserDetailsUpdateDto detailsDto, string currentUserLogin);
    Task ChangePasswordAsync(string login, PasswordUpdateDto passwordDto, string currentUserLogin);
    Task ChangeLoginAsync(string login, LoginUpdateDto loginDto, string currentUserLogin);
    Task<IEnumerable<UserDto>> GetAllActiveUsersAsync();
    Task<UserInfoDto> GetUserByLoginAsync(string login);
    Task<UserProfileDto> AuthenticateAsync(AuthenticateRequest request);
    Task<IEnumerable<UserDto>> GetUsersOlderThanAsync(int age);
    Task DeleteUserAsync(string login, string revokedBy, bool softDelete = true);
    Task RestoreUserAsync(string login);
}