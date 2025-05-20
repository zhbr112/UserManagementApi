using Microsoft.EntityFrameworkCore;

namespace UserManagementApi;

public class UserRepository(AppDbContext context, IPasswordHasher passwordHasher) : IUserRepository
{
    private readonly AppDbContext _context = context;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<User> CreateAsync(User user)
    {
        user.Password = _passwordHasher.HashPassword(user.Password);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetByLoginAsync(string login)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Login.Equals(login, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<User?> GetByCredentialsAsync(string login, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Login.Equals(login, StringComparison.OrdinalIgnoreCase));

        if (user == null || !_passwordHasher.VerifyPassword(user.Password, password))
            return null;

        return user;
    }

    public async Task<IEnumerable<User>> GetAllActiveAsync()
    {
        return await _context.Users
            .Where(u => u.RevokedOn == null)
            .OrderBy(u => u.CreatedOn)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<IEnumerable<User>> GetOlderThanAsync(int age)
    {
        var minBirthDate = DateTime.UtcNow.AddYears(-age - 1);
        return await _context.Users
            .Where(u => u.Birthday.HasValue && u.Birthday < minBirthDate)
            .ToListAsync();
    }

    public async Task<User> UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task DeleteAsync(string login, string revokedBy, bool softDelete = true)
    {
        var user = await GetByLoginAsync(login);
        if (user == null) return;

        if (softDelete)
        {
            user.RevokedOn = DateTime.UtcNow;
            user.RevokedBy = revokedBy;
            await UpdateAsync(user);
        }
        else
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task RestoreAsync(string login)
    {
        var user = await GetByLoginAsync(login);
        if (user == null) return;

        user.RevokedOn = null;
        user.RevokedBy = null;
        await UpdateAsync(user);
    }

    public async Task<bool> LoginExistsAsync(string login)
    {
        return await _context.Users
            .AnyAsync(u => u.Login.Equals(login, StringComparison.OrdinalIgnoreCase));
    }
}