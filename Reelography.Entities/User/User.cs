namespace Reelography.Entities.User;

public class User:BaseEntity
{
    
    public Guid AuthUserId { get; set; }
    public required AuthUser AuthUser { get; set; }
}