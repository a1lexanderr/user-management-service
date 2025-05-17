using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using UserManagementService.DTOs;
using UserManagementService.Models;
using UserManagementService.Services;

namespace UserManagementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        public UsersController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        private string GetCurrentUserLogin()
        {
            return User.FindFirst(ClaimTypes.Name)?.Value;
        }

        private bool IsCurrentUserAdmin()
        {
            return User.IsInRole("Admin");
        }

        [HttpGet("{login}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByLogin(string login)
        {
            try
            {
                User user = await _userService.GetUserByLogin(login);
                if (user == null)
                {
                    return NotFound(new { message = "Пользователь с таким логином не был найден" });
                }
                var result = new UserResponseDto(
                    user.Guid,
                    login,
                    user.Name,
                    user.Gender,
                    user.Birthday,
                    user.Admin,
                    user.CreatedOn,
                    user.CreatedBy,
                    user.ModifiedOn,
                    user.ModifiedBy,
                    !user.RevokedOn.HasValue,
                    user.RevokedOn,
                    user.RevokedBy
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Не удалось получить пользователя");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllActive()
        {
            var users = await _userService.GetAllActiveUsers();
            var result = users.Select(x => new UserSummaryDto(
                x.Guid,
                x.Login,
                x.Name,
                x.Admin,
                !x.RevokedOn.HasValue
                ));

            return Ok(result);
        }

        [HttpGet("older-than/{age}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOlderThan(int age, CancellationToken cancellationToken)
        {
            var users = await _userService.FilterUsersByAge(age, cancellationToken);
            var result = users.Select(x => new UserSummaryDto(
                x.Guid,
                x.Login,
                x.Name,
                x.Admin,
                !x.RevokedOn.HasValue
                ));

            return Ok(result);
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate(UserLoginDto dto)
        {
            try
            {
                var user = await _userService.GetUserByLoginAndPassword(dto.Login, dto.Password);
                var token = _authService.GenerateToken(user);

                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest("Ошибка аутентификации");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateUserDto createUserDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                User user = await _userService.CreateUser(createUserDto, GetCurrentUserLogin(), cancellationToken);
                return CreatedAtAction(nameof(GetByLogin), new { login = user.Login }, 
                    new { message = "Пользователь был успешно создан", id = user.Guid });
            }
            catch (Exception ex)
            {
                return BadRequest("Не удалось создать нового пользователя");
            }
        }

        [HttpPut("{login}")]
        [Authorize]
        public async Task<IActionResult> Update(string login, UpdateUserDto updateUserDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                if (!IsCurrentUserAdmin() && login != GetCurrentUserLogin())
                {
                    return Forbid();
                }

                User user = await _userService.UpdateUser(login, updateUserDto, GetCurrentUserLogin(), cancellationToken);

                return Ok(new { message = "Пользователь успешно обновлен" });
            }
            catch (Exception)
            {
                return BadRequest("Не удалось обновить пользователя");
            }
        }

        [HttpPut("{login}/password")]
        [Authorize]
        public async Task<IActionResult> UpdatePassword(string login, PasswordChangeDto passwordChangeDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                if (!IsCurrentUserAdmin() && login != GetCurrentUserLogin())
                {
                    return Forbid();
                }

                User user = await _userService.UpdateUserPassword(login, passwordChangeDto, GetCurrentUserLogin(), cancellationToken);
                return Ok(new { message = "Пароль успешно обновлен" });
            }
            catch (Exception)
            {
                return BadRequest("Не удалось обновить пароль");
            }
        }

        [HttpPut("{login}/update-login")]
        [Authorize]
        public async Task<IActionResult> UpdateLogin(string login, LoginChangeDto loginChangeDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                if (!IsCurrentUserAdmin() && login != GetCurrentUserLogin())
                {
                    return Forbid();
                }
                User user = await _userService.UpdateUserLogin(login, loginChangeDto, GetCurrentUserLogin(), cancellationToken);
                return Ok(new { message = "Логин успешно обновлен", newLogin = user.Login });
            }
            catch (Exception)
            {
                return BadRequest("Не удалось обновить логин");
            }
        }

        [HttpDelete("{login}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string login, [FromQuery] bool hardDelete, CancellationToken cancellationToken)
        {
            try
            {
                await _userService.DeleteUser(login, hardDelete, GetCurrentUserLogin(), cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest("Не удалось удалить пользователя");
            }
        }

        [HttpPut("{login}/restore")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Restore(string login, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userService.RestoreUser(login, GetCurrentUserLogin(), cancellationToken);
                return Ok("Пользователь успешно восстановлен");
            }
            catch (Exception ex)
            {
                return BadRequest("Не удалось восстановить пользователя");
            }
        }
    }
}
