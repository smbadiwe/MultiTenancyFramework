using Microsoft.AspNet.Identity;
using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.Mvc.Identity
{
    public class IdentityUser : AppUser, IUser<long>
    {
    }
}
