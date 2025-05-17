using System.ComponentModel.DataAnnotations;

namespace UserManagementService.DTOs
{
    public record PasswordChangeDto(
        [Required(ErrorMessage = "Указание нового пароля обязвтельно")]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Новый пароль должен содержать только латинские буквы и цифры.")]
        string NewPassword
        )
    { }
}
