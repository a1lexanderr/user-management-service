using System.ComponentModel.DataAnnotations;

namespace UserManagementService.DTOs
{
    public record UserLoginDto(
        [Required(ErrorMessage = "Указание логина необходимо")]
        string Login,
        [Required(ErrorMessage = "Указание пароля необходимо")]
        string Password
        )
    { }
}
