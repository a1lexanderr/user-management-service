using System.ComponentModel.DataAnnotations;

namespace UserManagementService.DTOs
{
    public record UpdateUserDto(
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Логин должен содержать только латинские буквы и цифры.")]
        string? Name,
        [Range(0, 2, ErrorMessage = "0 - женский пол, 1 - мужской пол, 2 - неизвестно")]
        int? Gender,
        DateTime? Birthday
        )
    {

    }
}
