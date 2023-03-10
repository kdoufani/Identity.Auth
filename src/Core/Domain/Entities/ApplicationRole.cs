namespace Identity.Auth.Core.Domain.Entities;

using System.Globalization;
using Identity.Auth.Core.Domain.Constants;
using Microsoft.AspNetCore.Identity;


public class ApplicationRole : IdentityRole<Guid>
{
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = default!;

    public static ApplicationRole User => new()
    {
        Name = IdentityRoleConstants.User, 
        NormalizedName = nameof(User).ToUpper(CultureInfo.InvariantCulture),
    };

    public static ApplicationRole Admin => new()
    {
        Name = IdentityRoleConstants.Admin,
        NormalizedName = nameof(Admin).ToUpper(CultureInfo.InvariantCulture)
    };
}
