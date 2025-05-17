using UserManagementService.Models;

namespace UserManagementService.Repositories
{
    public class InMemoryUserRepository : IUserRepository
    {
        private static readonly List<User> _users = new List<User>();
        private static readonly object _lock = new object();
        public Task AddUserAsync(User user, CancellationToken cancellationToken)
        {
            lock (_lock)
            {
                _users.Add(user);
            }
            return Task.CompletedTask;
        }

        public Task DeleteUserAsync(User userToDelete, CancellationToken cancellationToken)
        {
            lock (_lock)
            {
                _users.Remove(userToDelete);
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<User>> GetAllActiveUsers()
        {
            return Task.FromResult((IEnumerable<User>)_users.Where(x => x.RevokedOn == null).OrderBy(x => x.CreatedOn));
        }

        public Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return Task.FromResult<IEnumerable<User>>(_users);
        }

        public Task<IEnumerable<User>> GetOlderThanAsync(int age, CancellationToken cancellationToken)
        {
            DateTime dateTime = DateTime.Today;
            DateTime date = dateTime.AddYears(-age);
            return Task.FromResult(_users.Where(x => x.Birthday.HasValue && x.Birthday <= date));
        }

        public Task<User?> GetUserByGuidAsync(Guid guid)
        {
            return Task.FromResult(_users.FirstOrDefault(x => x.Guid == guid));
        }

        public Task<User?> GetUserByLoginAsync(string login)
        {
            return Task.FromResult(_users.FirstOrDefault(x => x.Login.Equals(login, StringComparison.OrdinalIgnoreCase)));
        }

        public Task<bool> LoginExistsAsync(string login)
        {
            return Task.FromResult(_users.Any(x => x.Login.Equals(login, StringComparison.OrdinalIgnoreCase)));
        }

        public Task UpdateUserAsync(User newUser, CancellationToken cancellationToken)
        {
            int index = _users.FindIndex(x => x.Guid.Equals(newUser.Guid));
            _users.Insert(index, newUser);
            return Task.CompletedTask;

        }
    }
}
