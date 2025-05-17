namespace UserManagementService.DTOs
{
    public record UserSummaryDto(
        Guid Id,
        string Login,
        string Name,
        bool IsAdmin,
        bool IsActive
        )
    {
    }
}
