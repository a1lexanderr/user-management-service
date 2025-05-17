namespace UserManagementService.DTOs
{
    public record UserResponseDto(
        Guid Id, 
        string Login,
        string Name,
        int Gender,
        DateTime? Birthday,
        bool IsAdmin,
        DateTime CreatedOn,
        string CreatedBy,
        DateTime? ModifiedOn,
        string ModifiedBy,
        bool IsActive,
        DateTime? RevokedOn,
        string RevokedBy
        )
    {
    }
}
