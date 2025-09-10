using Microsoft.EntityFrameworkCore;
using Reelography.Data;
using Reelography.Dto.User;
using Reelography.Service.Contracts.User;

namespace Reelography.Service.Services.User;

public class UserService:IUserService
{
    private readonly ReelographyDbContext _dbContext;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dbContext"></param>
    public UserService(ReelographyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserClaimDto> GetUserClaimDto(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.AuthUsers.Include(col=>col.AuthUserRole)
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == userId, cancellationToken: cancellationToken);
        if(user == null)
            throw new KeyNotFoundException("User not found");
        return new UserClaimDto()
        {
            Id = user.Id,
            Name = $"{user.FirstName} {user.LastName}",
            Email = user.Email,
            Role = user.AuthUserRole.Role,
        };
    }
}