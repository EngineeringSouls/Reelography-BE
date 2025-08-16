using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Reelography.Shared.Enums.User;

namespace Reelography.Entities.User;

[Table("AuthUserRoles", Schema = "user")]
public class AuthUserRole
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int AuthUserId { get; set; }

    public required AuthUserRoleEnum Role { get; set; }
    [StringLength(100)]
    public required string RoleName { get; set; }

    [ForeignKey(nameof(AuthUserId)), DeleteBehavior(DeleteBehavior.Restrict)]
    public AuthUser AuthUser { get; set; } = null!;
}