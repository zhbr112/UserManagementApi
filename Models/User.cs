using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagementApi;

public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(50)]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Логин может содержать только латинские буквы и цифры")]
    public string Login { get; set; }

    [Required]
    [MaxLength(100)]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Пароль может содержать только латинские буквы и цифры")]
    public string Password { get; set; }

    [Required]
    [MaxLength(100)]
    [RegularExpression(@"^[a-zA-Zа-яА-Я]+$", ErrorMessage = "Имя может содержать только русские и латинские буквы")]
    public string Name { get; set; }

    [Required]
    [Range(0, 2, ErrorMessage = "Пол должен быть 0 (женщина), 1 (мужчина) или 2 (неизвестно)")]
    public int Gender { get; set; }

    public DateTime? Birthday { get; set; }

    public bool Admin { get; set; }

    [Required]
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    [Required]
    public string CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }
    public string? ModifiedBy { get; set; }

    public DateTime? RevokedOn { get; set; }
    public string? RevokedBy { get; set; }

    [NotMapped]
    public bool IsActive => string.IsNullOrEmpty(RevokedBy);

    [NotMapped]
    public int Age => Birthday.HasValue ?
        (int)((DateTime.UtcNow - Birthday.Value).TotalDays / 365.25) : 0;
}