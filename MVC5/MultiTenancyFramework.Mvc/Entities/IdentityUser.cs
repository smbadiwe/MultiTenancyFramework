using Microsoft.AspNet.Identity;
using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.Entities
{
    public class IdentityUser : AppUser, IUser<long>
    {
    }
}
