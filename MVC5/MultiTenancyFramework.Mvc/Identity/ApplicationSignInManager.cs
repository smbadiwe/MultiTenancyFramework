using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using MultiTenancyFramework.Entities;
using MultiTenancyFramework.Mvc.Logic;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MultiTenancyFramework.Mvc.Identity
{
    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<IdentityUser, long>
    {
        public Func<Institution, InstitutionAccessValidationResult> ValidateInstitution;

        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }
        
        public override Task<ClaimsIdentity> CreateUserIdentityAsync(IdentityUser user)
        {
            if (user.ForceChangeOfPassword) throw new ForceChangeOfPasswordException();

            return user.GenerateUserIdentityAsync(UserManager);
        }

        public override Task SignInAsync(IdentityUser user, bool isPersistent, bool rememberBrowser)
        {
            ValidateInstitutionAccess();
            return base.SignInAsync(user, isPersistent, rememberBrowser);
        }

        public override Task<SignInStatus> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberBrowser)
        {
            ValidateInstitutionAccess();
            return base.TwoFactorSignInAsync(provider, code, isPersistent, rememberBrowser);
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
            var logger = Utilities.Logger;
            if (UserManager == null)
            {
                return SignInStatus.Failure;
            }
            ValidateInstitutionAccess();
            var user = await UserManager.FindByNameAsync(userName);
            if (user == null)
            {
                logger.Log(LoggingLevel.Error, "Username '{0}' tried signing in but no such user exists.", userName);
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

        protected virtual InstitutionAccessValidationResult OnValidateInstitution(Institution institution)
        {
            return ValidateInstitution?.Invoke(institution) ?? new InstitutionAccessValidationResult { AllowAccess = true };
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
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

        private void ValidateInstitutionAccess()
        {
            InstitutionAccessValidationResult result;
            string instCode = Convert.ToString(System.Web.HttpContext.Current.Request.RequestContext.RouteData.Values["institution"]);

            if (!Utilities.INST_DEFAULT_CODE.Equals(instCode, StringComparison.OrdinalIgnoreCase))
            {
                Institution institution;
                DataCacheMVC.AllInstitutions.TryGetValue(instCode, out institution);

                if (institution == null)
                {
                    result = new InstitutionAccessValidationResult
                    {
                        Remarks = $"We could not retrieve institution with Code: {instCode}"
                    };
                }
                else if (institution.IsDisabled == true)
                {
                    result = new InstitutionAccessValidationResult
                    {
                        Remarks = $"The institution with Code: '{instCode}' is currently disabled. Please contact administrator."
                    };
                }
                else if (institution.IsDeleted == true)
                {
                    result = new InstitutionAccessValidationResult
                    {
                        Remarks = $"The institution with Code: '{instCode}' is no longer maintained by our system. Please contact administrator."
                    };
                }
                else
                {
                    result = OnValidateInstitution(institution);
                }
            }
            else
            {
                result = new InstitutionAccessValidationResult { AllowAccess = true };
            }
            if (!result.AllowAccess)
            {
                throw new GeneralException(result.Remarks, ExceptionType.AccessDeniedInstitution);
            }
        }
    }
}
