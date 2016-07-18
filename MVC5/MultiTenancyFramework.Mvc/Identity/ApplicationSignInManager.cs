using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using MultiTenancyFramework.Entities;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MultiTenancyFramework.Mvc.Identity
{
    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<IdentityUser, long>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(IdentityUser user)
        {
            if (user.ForceChangeOfPassword) throw new ForceChangeOfPasswordException();

            return user.GenerateUserIdentityAsync(UserManager);
        }
        
        /// <summary>
        /// Sign in the user in using the user name and password
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="isPersistent"></param>
        /// <param name="shouldLockout"></param>
        /// <returns></returns>
        public override async Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            if (UserManager == null)
            {
                return SignInStatus.Failure;
            }
            var user = await UserManager.FindByNameAsync(userName);
            if (user == null)
            {
                return SignInStatus.Failure;
            }
            if (user.LockoutEnabled) //  (await UserManager.IsLockedOutAsync(user.Id))
            {
                return SignInStatus.LockedOut;
            }
            if (user.ForceChangeOfPassword)
            {
                throw new ForceChangeOfPasswordException();
            }
            if (await UserManager.CheckPasswordAsync(user, password))
            {
                return await SignInOrTwoFactor(user, isPersistent);
            }
            if (shouldLockout)
            {
                // If lockout is requested, increment access failed count which might lock out the user
                await UserManager.AccessFailedAsync(user.Id);
                if (await UserManager.IsLockedOutAsync(user.Id))
                {
                    return SignInStatus.LockedOut;
                }
            }
            return SignInStatus.Failure;
        }

        private async Task<SignInStatus> SignInOrTwoFactor(IdentityUser user, bool isPersistent)
        {
            var id = Convert.ToString(user.Id);
            //if (await UserManager.GetTwoFactorEnabledAsync(user.Id)
            //    && (await UserManager.GetValidTwoFactorProvidersAsync(user.Id)).Count > 0
            //    && !await AuthenticationManager.TwoFactorBrowserRememberedAsync(id))
            if (user.TwoFactorEnabled
                && (await UserManager.GetValidTwoFactorProvidersAsync(user.Id)).Count > 0
                && !await AuthenticationManager.TwoFactorBrowserRememberedAsync(id))
            {
                var identity = new ClaimsIdentity(DefaultAuthenticationTypes.TwoFactorCookie);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, id));
                AuthenticationManager.SignIn(identity);
                return SignInStatus.RequiresVerification;
            }
            await SignInAsync(user, isPersistent, false);
            return SignInStatus.Success;
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
