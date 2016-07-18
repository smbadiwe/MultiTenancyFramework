using Microsoft.AspNet.Identity;

namespace MultiTenancyFramework.Entities
{
    public class IdentityUser : AppUser, IUser<long>
    {
    }
}
