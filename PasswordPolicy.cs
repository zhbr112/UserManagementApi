namespace UserManagementApi;

public class PasswordPolicy
{
    public static bool IsStrongPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        // Минимум 8 символов
        if (password.Length < 8)
            return false;

        // Хотя бы одна цифра
        if (!password.Any(char.IsDigit))
            return false;

        // Хотя бы одна буква в верхнем регистре
        if (!password.Any(char.IsUpper))
            return false;

        // Хотя бы одна буква в нижнем регистре
        if (!password.Any(char.IsLower))
            return false;

        return true;
    }
}
