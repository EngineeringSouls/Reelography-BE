using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Reelography.Entities.User;

public class AuthUser: IdentityUser<int>
{


    public string FirstName { get; set; }
    
    public string LastName { get; set; }

    /// <summary>
    /// User Active Flag 
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Is Profile Pic Verified
    /// </summary>
    public bool IsProfilePicVerified { get; set; }
    
    /// <summary>
    /// Is Email Verified
    /// </summary>
    public bool IsEmailVerified { get; set; }
    
    /// <summary>
    /// Email Verified On
    /// </summary>
    public DateTime? EmailVerifiedOn { get; set; }
    /// <summary>
    /// IsPhone Verified
    /// </summary>
    public bool IsPhoneVerified { get; set; }
       
    /// <summary>
    /// Phone Verified On
    /// </summary>
    public DateTime? PhoneVerifiedOn { get; set; }
    
    /// <summary>
    /// Login/registered method Type Id
    /// </summary>
    public int RegisterMethodTypeId { get; set; }
    
    [ForeignKey(nameof(RegisterMethodTypeId)), DeleteBehavior(DeleteBehavior.Restrict)]
    public virtual RegisterMethodType? LoginMethodType { get; set; }
    
    public virtual User? User { get; set; }
    public virtual Photographer? Photographer { get; set; }
    public AuthUserRole AuthUserRole { get; set; } = null!;
}