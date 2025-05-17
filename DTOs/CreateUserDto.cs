using System.ComponentModel.DataAnnotations;

namespace UserManagementService.DTOs
{
    public record CreateUserDto(
        [Required(ErrorMessage = "Логин не указан")]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Логин должен содержать только латинские буквы и цифры.")]
        string Login,
        [Required(ErrorMessage = "Пароль обязателен")]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Пароль должен содержать только латинские буквы и цифры.")]
        string Password,
        [Required(ErrorMessage = "Имя не указано")]
        [RegularExpression("^[a-zA-Zа-яА-ЯёЁ ]+$", ErrorMessage = "Имя должно содержать только латинские или русские буквы и пробелы.")]
        string Name,
        [Required(ErrorMessage = "Пол обязателеен. 0 - женский пол, 1 - мужской пол, 2 - неизвестно")]
        [Range(0, 2, ErrorMessage = "0 - женский пол, 1 - мужской пол, 2 - неизвестно")]
        int Gender,
        DateTime? Birthday,
        [Required(ErrorMessage = "Указание поля IsAdmin обязвтельно")]
        bool IsAdmin
        )
    {

    }
}
