using UserManagementService.Models;
using UserManagementService.DTOs;

namespace UserManagementService.Services
{
    public interface IUserService
    {
        Task<User> CreateUser(CreateUserDto userCreateDto, string createdBy, CancellationToken cancellationToken);
        Task<User> UpdateUser(string login, UpdateUserDto userUpdateDto, string updatedBy, CancellationToken cancellationToken);
        Task<User> UpdateUserPassword(string login, PasswordChangeDto passwordChangeDto, string updatedBy, CancellationToken cancellationToken);
        Task<User> UpdateUserLogin(string login, LoginChangeDto loginChangeDto, string updatedBy, CancellationToken cancellationToken);
        Task<IEnumerable<User>> GetAllActiveUsers();
        Task<User> GetUserByLogin(string login);
        Task<User> GetUserByLoginAndPassword(string login, string password);
        Task<IEnumerable<User>> FilterUsersByAge(int age, CancellationToken cancellationToken);
        Task<bool> DeleteUser(string login, bool hardDelete, string revokedBy, CancellationToken cancellationToken);
        Task<User> RestoreUser(string login, string modifiedBy, CancellationToken cancellationToken);
    }
}
