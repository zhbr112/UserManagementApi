namespace UserManagementApi;

public class UserService(IUserRepository userRepository, IPasswordHasher passwordHasher, ILogger<UserService> logger) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly ILogger<UserService> _logger = logger;

    public async Task<UserDto> CreateUserAsync(UserCreateDto userDto, string currentUserLogin)
    {
        if (await _userRepository.LoginExistsAsync(userDto.Login))
            throw new ArgumentException("Логин уже существует");

        if (!PasswordPolicy.IsStrongPassword(userDto.Password))
            throw new ArgumentException("Пароль должен содержать минимум 8 символов, включая цифры, заглавные и строчные буквы");

        var user = new User
        {
            Login = userDto.Login,
            Password = _passwordHasher.HashPassword(userDto.Password),
            Name = userDto.Name,
            Gender = userDto.Gender,
            Birthday = userDto.Birthday,
            Admin = userDto.IsAdmin,
            CreatedBy = currentUserLogin
        };

        var createdUser = await _userRepository.CreateAsync(user);
        return MapToUserDto(createdUser);
    }

    public async Task<UserDto> UpdateUserDetailsAsync(string login, UserDetailsUpdateDto detailsDto, string currentUserLogin)
    {
        var user = await GetUserAndCheckAccess(login, currentUserLogin);

        user.Name = detailsDto.Name;
        user.Gender = detailsDto.Gender;
        user.Birthday = detailsDto.Birthday;
        user.ModifiedOn = DateTime.UtcNow;
        user.ModifiedBy = currentUserLogin;

        await _userRepository.UpdateAsync(user);
        return MapToUserDto(user);
    }

    public async Task ChangePasswordAsync(string login, PasswordUpdateDto passwordDto, string currentUserLogin)
    {
        if (!PasswordPolicy.IsStrongPassword(passwordDto.NewPassword))
            throw new ArgumentException("Пароль должен содержать минимум 8 символов, включая цифры, заглавные и строчные буквы");
        var user = await GetUserAndCheckAccess(login, currentUserLogin);

        user.Password = _passwordHasher.HashPassword(passwordDto.NewPassword);
        user.ModifiedOn = DateTime.UtcNow;
        user.ModifiedBy = currentUserLogin;

        await _userRepository.UpdateAsync(user);
    }

    public async Task ChangeLoginAsync(string login, LoginUpdateDto loginDto, string currentUserLogin)
    {
        var user = await GetUserAndCheckAccess(login, currentUserLogin);

        if (await _userRepository.LoginExistsAsync(loginDto.NewLogin))
            throw new ArgumentException("Логин уже существует");

        user.Login = loginDto.NewLogin;
        user.ModifiedOn = DateTime.UtcNow;
        user.ModifiedBy = currentUserLogin;

        await _userRepository.UpdateAsync(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllActiveUsersAsync()
    {
        var users = await _userRepository.GetAllActiveAsync();
        return users.Select(MapToUserDto);
    }

    public async Task<UserInfoDto> GetUserByLoginAsync(string login)
    {
        var user = await _userRepository.GetByLoginAsync(login)
            ?? throw new KeyNotFoundException("Пользователь не найден");

        return new UserInfoDto
        {
            Name = user.Name,
            Gender = user.Gender switch { 0 => "Женский", 1 => "Мужской", _ => "Неизвестно" },
            Birthday = user.Birthday,
            Status = user.IsActive ? "Активный" : "Неактивный"
        };
    }

    public async Task<UserProfileDto> AuthenticateAsync(AuthenticateRequest request)
    {
        var user = await _userRepository.GetByCredentialsAsync(request.Login, request.Password)
            ?? throw new UnauthorizedAccessException("Неверный логин или пароль");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Пользователь неактивен");

        return new UserProfileDto
        {
            Login = user.Login,
            Name = user.Name,
            Gender = user.Gender switch { 0 => "Женский", 1 => "Мужской", _ => "Неизвестно" },
            Birthday = user.Birthday,
            IsAdmin = user.Admin
        };
    }

    public async Task<IEnumerable<UserDto>> GetUsersOlderThanAsync(int age)
    {
        var users = await _userRepository.GetOlderThanAsync(age);
        return users.Select(MapToUserDto);
    }

    public async Task DeleteUserAsync(string login, string revokedBy, bool softDelete = true)
    {
        await _userRepository.DeleteAsync(login, revokedBy, softDelete);
    }

    public async Task RestoreUserAsync(string login)
    {
        await _userRepository.RestoreAsync(login);
    }

    private async Task<User> GetUserAndCheckAccess(string login, string currentUserLogin)
    {
        var currentUser = await _userRepository.GetByLoginAsync(currentUserLogin);
        var user = await _userRepository.GetByLoginAsync(login)
            ?? throw new KeyNotFoundException("Пользователь не найден");

        if (!(currentUser?.Admin ?? false) && (currentUserLogin != login || !user.IsActive))
            throw new UnauthorizedAccessException("Доступ запрещен");

        return user;
    }

    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Login = user.Login,
            Name = user.Name,
            Gender = user.Gender,
            Birthday = user.Birthday,
            IsAdmin = user.Admin,
            CreatedOn = user.CreatedOn,
            IsActive = user.IsActive
        };
    }
}