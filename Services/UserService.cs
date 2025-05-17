using UserManagementService.DTOs;
using UserManagementService.Models;
using UserManagementService.Repositories;

namespace UserManagementService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository) {
            _userRepository = userRepository;        
        }
        public async Task<User> CreateUser(CreateUserDto userCreateDto, string createdBy, CancellationToken cancellationToken)
        {
            if (await _userRepository.LoginExistsAsync(userCreateDto.Login))
                throw new InvalidOperationException("Пользователь с таким логином уже существует");

            User user = new User
            {
                Guid = Guid.NewGuid(),
                Login = userCreateDto.Login,
                Password = userCreateDto.Password,
                Name = userCreateDto.Name,
                Gender = userCreateDto.Gender,
                Birthday = userCreateDto.Birthday,
                Admin = userCreateDto.IsAdmin,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = createdBy,
                ModifiedOn = DateTime.UtcNow,
                ModifiedBy = createdBy,
            };

            await _userRepository.AddUserAsync(user, cancellationToken);
            return user;
        }

        public async Task<bool> DeleteUser(string login, bool hardDelete, string revokedBy, CancellationToken cancellationToken)
        {
            User? user = await _userRepository.GetUserByLoginAsync(login);
            if (user == null)
                throw new InvalidOperationException("Пользователя с таким логином не существует");
            if (hardDelete)
            {
                await _userRepository.DeleteUserAsync(user, cancellationToken);
                return true;
            }
            else
            {
                user.RevokedOn = DateTime.UtcNow;
                user.RevokedBy = revokedBy;
                user.ModifiedOn = DateTime.UtcNow;
                user.ModifiedBy = revokedBy;
                await _userRepository.UpdateUserAsync(user, cancellationToken);
                return false;
            }

        }

        public async Task<IEnumerable<User>> FilterUsersByAge(int age, CancellationToken cancellationToken)
        {
            return await _userRepository.GetOlderThanAsync(age, cancellationToken);
        }

        public async Task<IEnumerable<User>> GetAllActiveUsers()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<User> GetUserByLogin(string login)
        {
            return await _userRepository.GetUserByLoginAsync(login);
        }

        public async Task<User> GetUserByLoginAndPassword(string login, string password)
        {
            User? user = await _userRepository.GetUserByLoginAsync(login);
            if(user == null)
                throw new InvalidOperationException("Пользователя с таким логином не существует");

            if (user.Password != password)
                throw new InvalidOperationException("Неверный пароль");

            return user;
        }

        public async Task<User> RestoreUser(string login, string modifiedBy, CancellationToken cancellationToken)
        {
            User? user = await _userRepository.GetUserByLoginAsync(login);
            if( user == null )
                throw new InvalidOperationException("Пользователя с таким логином не существует");
            if(!user.RevokedOn.HasValue)
                throw new InvalidOperationException("Профиль пользователя не был удален");

            user.RevokedOn = null;
            user.RevokedBy = null;
            user.ModifiedOn = DateTime.UtcNow;
            user.ModifiedBy = modifiedBy;

            await _userRepository.UpdateUserAsync(user, cancellationToken);
            return user;
        }

        public async Task<User> UpdateUser(string login, UpdateUserDto userUpdateDto, string modifiedBy, CancellationToken cancellationToken)
        {
            User? user = await _userRepository.GetUserByLoginAsync(login);
            if (user == null)
                throw new InvalidOperationException("Пользователя с таким логином не существует");

            if (user.RevokedOn.HasValue)
                throw new InvalidOperationException("Профиль пользователя был удален");

            if (userUpdateDto.Name != null)
            {
                user.Name = userUpdateDto.Name;
            }

            if (userUpdateDto.Gender.HasValue)
            {
                user.Gender = userUpdateDto.Gender.Value;
            }

            if (userUpdateDto.Birthday.HasValue)
            {
                user.Birthday = userUpdateDto.Birthday;
            }

            user.ModifiedOn = DateTime.UtcNow;
            user.ModifiedBy = modifiedBy;

            await _userRepository.UpdateUserAsync(user, cancellationToken);
            return user;
        }

        public async Task<User> UpdateUserLogin(string login, LoginChangeDto loginChangeDto, string modifiedBy, CancellationToken cancellationToken)
        {
            User? user = await _userRepository.GetUserByLoginAsync(login);
            if (user == null)
                throw new InvalidOperationException("Пользователя с таким логином не существует");

            if (user.RevokedOn.HasValue)
                throw new InvalidOperationException("Профиль пользователя был удален");

            if(await _userRepository.GetUserByLoginAsync(loginChangeDto.NewLogin) != null)
            {
                throw new InvalidOperationException("Пользователь с таким логином уже существует");
            }

            user.Login = loginChangeDto.NewLogin;
            user.ModifiedOn = DateTime.UtcNow;
            user.ModifiedBy = modifiedBy;

            await _userRepository.UpdateUserAsync(user, cancellationToken);
            return user;
        }

        public async Task<User> UpdateUserPassword(string login, PasswordChangeDto passwordChangeDto, string modifiedBy, CancellationToken cancellationToken)
        {
            User? user = await _userRepository.GetUserByLoginAsync(login);
            if (user == null)
                throw new InvalidOperationException("Пользователя с таким логином не существует");

            if (user.RevokedOn.HasValue)
                throw new InvalidOperationException("Профиль пользователя был удален");

            user.Password = passwordChangeDto.NewPassword;
            user.ModifiedOn = DateTime.UtcNow;
            user.ModifiedBy = modifiedBy;

            await _userRepository.UpdateUserAsync(user, cancellationToken);
            return user;
        }

    }
}
