namespace UserManagementApi;

public interface IUserRepository
{
    Task<User> CreateAsync(User user);
    Task<User?> GetByLoginAsync(string login);
    Task<User?> GetByCredentialsAsync(string login, string password);
    Task<IEnumerable<User>> GetAllActiveAsync();
    Task<IEnumerable<User>> GetAllAsync();
    Task<IEnumerable<User>> GetOlderThanAsync(int age);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(string login, string revokedBy, bool softDelete = true);
    Task RestoreAsync(string login);
    Task<bool> LoginExistsAsync(string login);
}