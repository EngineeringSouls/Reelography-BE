namespace Reelography.Entities.User;

public class User:BaseEntity
{
    public int Id { get; set; }
    public required int AuthUserId { get; set; }
    public required AuthUser AuthUser { get; set; }
}