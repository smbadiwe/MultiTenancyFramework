using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Threading.Tasks;

namespace MultiTenancyFramework.Mvc.Identity
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<IdentityUser, long>
    {
        public ApplicationUserManager(IUserStore<IdentityUser, long> store)
            : base(store)
        {

        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var userStore = new UserStore<IdentityUser>();
            var manager = new ApplicationUserManager(userStore);
            var UsernameAndPasswordRule = MvcUtility.SystemSettings.UsernameAndPasswordRule;
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<IdentityUser, long>(manager)
            {
                AllowOnlyAlphanumericUserNames = UsernameAndPasswordRule.AllowOnlyAlphanumericUserNames,
                RequireUniqueEmail = UsernameAndPasswordRule.RequireUniqueEmail
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = UsernameAndPasswordRule.PasswordRequiredLength,
                RequireNonLetterOrDigit = UsernameAndPasswordRule.PasswordRequireNonLetterOrDigit,
                RequireDigit = UsernameAndPasswordRule.PasswordRequireDigit,
                RequireLowercase = UsernameAndPasswordRule.PasswordRequireLowercase,
                RequireUppercase = UsernameAndPasswordRule.PasswordRequireUppercase,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = UsernameAndPasswordRule.UserLockoutEnabledByDefault;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(UsernameAndPasswordRule.DefaultAccountLockoutTimeSpanInMinutes);
            manager.MaxFailedAccessAttemptsBeforeLockout = UsernameAndPasswordRule.MaxFailedAccessAttemptsBeforeLockout;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<IdentityUser, long>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<IdentityUser, long>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<IdentityUser, long>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        /// <summary>
        ///     Reset a user's password when user is forced by the app to do so
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> ResetPasswordAsync(IdentityUser user, string newPassword)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var passwordStore = GetPasswordStore();
            var result = await UpdatePasswordInternal(passwordStore, user, newPassword).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                return result;
            }
            user.ForceChangeOfPassword = false;
            return await UpdateAsync(user).ConfigureAwait(false);
        }

        private async Task<IdentityResult> UpdatePasswordInternal(IUserPasswordStore<IdentityUser, long> passwordStore,
            IdentityUser user, string newPassword)
        {
            var result = await PasswordValidator.ValidateAsync(newPassword).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                return result;
            }
            await passwordStore.SetPasswordHashAsync(user, PasswordHasher.HashPassword(newPassword)).ConfigureAwait(false);
            //await UpdateSecurityStampInternal(user).ConfigureAwait(false);
            return IdentityResult.Success;
        }

        // IUserPasswordStore methods
        private IUserPasswordStore<IdentityUser, long> GetPasswordStore()
        {
            var cast = Store as IUserPasswordStore<IdentityUser, long>;
            if (cast == null)
            {
                throw new NotSupportedException("Store Not IUserPasswordStore");
            }
            return cast;
        }
    }
}
