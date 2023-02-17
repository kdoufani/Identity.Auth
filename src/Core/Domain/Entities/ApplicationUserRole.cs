namespace Identity.Auth.Core.Domain.Entities;

using Microsoft.AspNetCore.Identity;

public class ApplicationUserRole : IdentityUserRole<Guid>
{
    public virtual ApplicationUser? User { get; set; }
    public virtual ApplicationRole? Role { get; set; }
}
