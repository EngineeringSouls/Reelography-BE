using System.ComponentModel.DataAnnotations;

namespace Reelography.Entities;

public class RegisterMethodType
{
    /// <summary>
    /// Primary Key
    /// </summary>
    [Key]
    public required int Id { get; set; }
    
    /// <summary>
    ///  Name
    /// </summary>
    [StringLength(100)]
    public required string Name { get; set; }
}