using Microsoft.EntityFrameworkCore;
using UserManagementService.Data;
using UserManagementService.Models;

namespace UserManagementService.Repositories
{
    public class EfUserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public EfUserRepository(ApplicationDbContext applicationDbContext) {
            _context = applicationDbContext;
        }
        public async Task AddUserAsync(User user, CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteUserAsync(User user, CancellationToken cancellationToken)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<User>> GetAllActiveUsers()
        {
            return await _context.Users.Where(x => !x.RevokedOn.HasValue)
                .OrderBy(x => x.CreatedOn).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<IEnumerable<User>> GetOlderThanAsync(int age, CancellationToken cancellationToken)
        {
            DateTime dateTime = DateTime.Today;
            DateTime date = dateTime.AddYears(-age);
            return await _context.Users.Where(x => x.Birthday.HasValue && x.Birthday.Value <= date).ToListAsync(cancellationToken);
        }

        public async Task<User?> GetUserByGuidAsync(Guid guid)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Guid == guid);
        }

        public async Task<User?> GetUserByLoginAsync(string login)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Login == login);
        }

        public async Task<bool> LoginExistsAsync(string login)
        {
            return await _context.Users.AnyAsync(x => x.Login == login);
        }

        public async Task UpdateUserAsync(User user, CancellationToken cancellationToken)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
