using UserManagementService.Models;


namespace UserManagementService.Repositories
{
    public interface IUserRepository
    {     
        Task<User?> GetUserByLoginAsync(string login);
        Task<User?> GetUserByGuidAsync(Guid guid);
        Task<IEnumerable<User>> GetOlderThanAsync(int age, CancellationToken cancellationToken);
        Task<IEnumerable<User>> GetAllActiveUsers();
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task AddUserAsync(User user, CancellationToken cancellationToken);
        Task UpdateUserAsync(User user, CancellationToken cancellationToken);
        Task DeleteUserAsync(User user, CancellationToken cancellationToken);
        Task<bool> LoginExistsAsync(string login);
    }
}
