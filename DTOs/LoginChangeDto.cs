using System.ComponentModel.DataAnnotations;

namespace UserManagementService.DTOs
{
    public record LoginChangeDto(
        [Required(ErrorMessage = "Новый логин обязателен.")]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Новый логин должен содержать только латинские буквы и цифры.")]
        string NewLogin
        )
    { }
}
