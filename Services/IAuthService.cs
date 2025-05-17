using System.Security.Claims;
using UserManagementService.Models;

namespace UserManagementService.Services
{
    public interface IAuthService
    {
        string GenerateToken(User user);
        ClaimsPrincipal ValidateToken(string token);
    }
}
