using Microsoft.AspNet.Identity;
using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using MultiTenancyFramework.Logic;
using MultiTenancyFramework.Mvc.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MultiTenancyFramework.Mvc.Identity
{
    /// <summary>
    /// Class that implements the key ASP.NET Identity user store iterfaces
    /// </summary>
    public class UserStore<TUser> : UserStore<TUser, long> where TUser : IdentityUser
    {

    }

    /// <summary>
    /// Class that implements the key ASP.NET Identity user store interfaces
    /// <para>NB: To support multitenancy, make sure you always set InstitutionCode before usage</para>
    /// </summary>
    public class UserStore<TUser, idT> : IUserLoginStore<TUser, idT>,
        IUserClaimStore<TUser, idT>,
        IUserRoleStore<TUser, idT>,
        IUserPasswordStore<TUser, idT>,
        //IUserSecurityStampStore<TUser, idT>,
        IQueryableUserStore<TUser, idT>,
        IUserEmailStore<TUser, idT>,
        IUserPhoneNumberStore<TUser, idT>,
        IUserTwoFactorStore<TUser, idT>,
        IUserLockoutStore<TUser, idT>,
        IUserStore<TUser, idT>
        where TUser : IdentityUser, IUser<idT>
    {
        private AppUserLogic _userEngine;
        private UserRoleLogic _userRoleEngine;
        private UserClaimLogic _userClaimsEngine;
        private UserLoginLogic _userLoginsEngine;
        public UserStore()
        {
            _userLoginsEngine = new UserLoginLogic(null);
            _userClaimsEngine = new UserClaimLogic(null);
            _userEngine = new AppUserLogic(null);
            _userRoleEngine = new UserRoleLogic(null);
        }

        private IDbQueryProcessor _queryProcessor;

        protected IDbQueryProcessor QueryProcessor
        {
            get
            {
                if (_queryProcessor == null) _queryProcessor = Utilities.QueryProcessor;

                _queryProcessor.InstitutionCode = WebUtilities.InstitutionCode;
                return _queryProcessor;
            }
        }

        public IQueryable<TUser> Users
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Insert a new TUser in the UserTable
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task CreateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            _userEngine.InstitutionCode = WebUtilities.InstitutionCode;
            _userEngine.Insert(user);

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Returns an TUser instance based on a userId query 
        /// </summary>
        /// <param name="userId">The user's Id</param>
        /// <returns></returns>
        public Task<TUser> FindByIdAsync(idT userId)
        {
            if (userId == null)
            {
                throw new ArgumentException("Null or empty argument: userId");
            }

            _userEngine.InstitutionCode = WebUtilities.InstitutionCode;
            TUser result = _userEngine.Retrieve(Convert.ToInt64(userId)) as TUser;
            if (result != null)
            {
                result.InstitutionCode = _userEngine.InstitutionCode;
                return Task.FromResult(result);
            }

            return Task.FromResult<TUser>(null);
        }

        /// <summary>
        /// Returns an TUser instance based on a userName query 
        /// </summary>
        /// <param name="userName">The user's name</param>
        /// <returns></returns>
        public Task<TUser> FindByNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException("userName");
            }

            _userEngine.InstitutionCode = WebUtilities.InstitutionCode;
            TUser user = _userEngine.RetrieveByUsername(userName) as TUser;

            if (user != null)
            {
                user.InstitutionCode = _userEngine.InstitutionCode;
                return Task.FromResult(user);
            }

            return Task.FromResult<TUser>(null);
        }

        /// <summary>
        /// Updates the UsersTable with the TUser instance values
        /// </summary>
        /// <param name="user">TUser to be updated</param>
        /// <returns></returns>
        public Task UpdateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (user.Id > 0)
            {
                _userEngine.InstitutionCode = WebUtilities.InstitutionCode;
                _userEngine.Update(user);
            }
            return Task.FromResult<object>(null);
        }

        public void Dispose()
        {

        }

        #region IUserClaimStore methods - TREATED
        /// <summary>
        /// Inserts a claim to the UserClaimsTable for the given user
        /// </summary>
        /// <param name="user">User to have claim added</param>
        /// <param name="claim">Claim to be added</param>
        /// <returns></returns>
        public Task AddClaimAsync(TUser user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("user");
            }

            var userClaim = new UserClaim(claim);
            if (userClaim == null)
            {
                throw new ArgumentException("Use or derive from MultiTenancyFramework.Core.UserClaim");
            }

            userClaim.UserId = user.Id;
            _userClaimsEngine.InstitutionCode = WebUtilities.InstitutionCode;
            _userClaimsEngine.Insert(userClaim);

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Returns all claims for a given user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            ClaimsIdentity identity = new ClaimsIdentity();
            var query = new GetUserClaimsByUserIdQuery
            {
                UserId = user.Id,
            };
            QueryProcessor.InstitutionCode = WebUtilities.InstitutionCode;
            var rows = QueryProcessor.Process(query);
            if (rows != null && rows.Count > 0)
            {
                foreach (var row in rows)
                {
                    identity.AddClaim(new Claim(row.ClaimType, row.ClaimValue, row.ClaimValueType));
                }
            }
            return Task.FromResult<IList<Claim>>(identity.Claims.ToList());
        }

        public Task RemoveClaimAsync(TUser user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            var userClaim = new UserClaim(claim);
            if (userClaim == null)
            {
                throw new ArgumentException("Use or derive from MultiTenancyFramework.Core.UserClaim");
            }
            if (userClaim.UserId != user.Id)
            {
                throw new InvalidOperationException("Claim does not belong to this user");
            }
            _userClaimsEngine.InstitutionCode = WebUtilities.InstitutionCode;
            _userClaimsEngine.Delete(userClaim);

            return Task.FromResult<object>(null);
        }
        #endregion

        #region IUserLoginStore methods - TREATED
        /// <summary>
        /// Inserts a Login in the UserLoginsTable for a given User
        /// </summary>
        /// <param name="user">User to have login added</param>
        /// <param name="login">Login to be added</param>
        /// <returns></returns>
        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            UserLogin userLogin = new UserLogin
            {
                UserId = user.Id,
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey,
            };

            _userLoginsEngine.InstitutionCode = WebUtilities.InstitutionCode;
            _userLoginsEngine.Insert(userLogin);

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Returns an TUser based on the Login info
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public Task<TUser> FindAsync(UserLoginInfo login)
        {
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            var query = new GetUserLoginByLoginProviderKeyAndUserIdQuery
            {
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey,
            };
            QueryProcessor.InstitutionCode = WebUtilities.InstitutionCode;
            var userLogin = QueryProcessor.Process(query);
            if (userLogin != null)
            {
                _userEngine.InstitutionCode = WebUtilities.InstitutionCode;
                TUser user = _userEngine.Retrieve(Convert.ToInt64(userLogin.UserId)) as TUser;
                if (user != null)
                {
                    return Task.FromResult(user);
                }
            }

            return Task.FromResult<TUser>(null);
        }

        /// <summary>
        /// Returns list of UserLoginInfo for a given TUser
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var query = new GetUserLoginsByUserIdQuery
            {
                UserId = user.Id,
            };
            QueryProcessor.InstitutionCode = WebUtilities.InstitutionCode;
            var userLogins = QueryProcessor.Process(query);
            if (userLogins != null && userLogins.Count > 0)
            {
                List<UserLoginInfo> logins = new List<UserLoginInfo>();
                foreach (var item in userLogins)
                {
                    logins.Add(new UserLoginInfo(item.LoginProvider, item.ProviderKey));
                }
                return Task.FromResult<IList<UserLoginInfo>>(logins);
            }

            return Task.FromResult<IList<UserLoginInfo>>(new List<UserLoginInfo>());
        }

        /// <summary>
        /// Deletes a login from UserLoginsTable for a given TUser
        /// </summary>
        /// <param name="user">User to have login removed</param>
        /// <param name="login">Login to be removed</param>
        /// <returns></returns>
        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            var query = new GetUserLoginByLoginProviderKeyAndUserIdQuery
            {
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey,
                UserID = user.Id,
            };
            QueryProcessor.InstitutionCode = WebUtilities.InstitutionCode;
            var userLogin = QueryProcessor.Process(query);
            if (userLogin != null)
            {
                _userLoginsEngine.InstitutionCode = WebUtilities.InstitutionCode;
                _userLoginsEngine.Delete(userLogin);
            }

            return Task.FromResult<object>(null);
        }
        #endregion

        #region IUserRoleStore methods - TREATED
        /// <summary>
        /// Inserts a entry in the UserRoles table. Nobody will actually use this
        /// </summary>
        /// <param name="user">User to have role added</param>
        /// <param name="roleName">Name of the role to be added to user</param>
        /// <returns></returns>
        public Task AddToRoleAsync(TUser user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("Argument cannot be null or empty: roleName.");
            }

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Returns the roles for a given TUser
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<IList<string>> GetRolesAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            HashSet<string> userRoles = new HashSet<string>();
            _userRoleEngine.InstitutionCode = WebUtilities.InstitutionCode;
            var theUserRoles = _userRoleEngine.RetrieveByIDs(user.UserRoleIDs.ToArray());

            if (theUserRoles != null && theUserRoles.Count > 0)
            {
                ActionAccessPrivilege privilege;
                var functionsInDb = DataCacheMVC.AllPrivileges;
                foreach (var userRole in theUserRoles)
                {
                    foreach (var priv in userRole.PrivilegeIDs)
                    {
                        if (functionsInDb.TryGetValue(priv, out privilege) && privilege != null)
                        {
                            userRoles.Add(privilege.Name);
                        }
                    }
                }
            }
            IList<string> result = userRoles.ToList();
            return Task.FromResult(result);
        }

        /// <summary>
        /// Verifies if a user is in a role
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public Task<bool> IsInRoleAsync(TUser user, string role)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrWhiteSpace(role))
            {
                throw new ArgumentNullException("role");
            }
            _userRoleEngine.InstitutionCode = WebUtilities.InstitutionCode;

            var theUserRoles = _userRoleEngine.RetrieveByIDs(user.UserRoleIDs.ToArray());

            if (theUserRoles != null && theUserRoles.Count > 0)
            {
                ActionAccessPrivilege privilege;
                var functionsInDb = DataCacheMVC.AllPrivileges;
                foreach (var userRole in theUserRoles)
                {
                    foreach (var priv in userRole.PrivilegeIDs)
                    {
                        if (functionsInDb.TryGetValue(priv, out privilege) && privilege != null && privilege.IsEnabled)
                        {
                            if (privilege.Name == role)
                            {
                                return Task.FromResult(true);
                            }
                        }
                    }
                }
            }
            return Task.FromResult(false);
        }

        /// <summary>
        /// Removes a user from a role
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public Task RemoveFromRoleAsync(TUser user, string role)
        {
            throw new NotImplementedException();
        }
        #endregion

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task DeleteAsync(TUser user)
        {
            if (user != null)
            {
                _userEngine.InstitutionCode = WebUtilities.InstitutionCode;
                _userEngine.Delete(user);
            }

            return Task.FromResult<object>(null);
        }

        #region IUserPasswordStore interfaces
        /// <summary>
        /// Returns the PasswordHash for a given TUser
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetPasswordHashAsync(TUser user)
        {
            return Task.FromResult(user.PasswordHash);
        }

        /// <summary>
        /// Verifies if user has password
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> HasPasswordAsync(TUser user)
        {
            return Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));
        }

        /// <summary>
        /// Sets the password hash for a given TUser
        /// </summary>
        /// <param name="user"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;

            return Task.FromResult<object>(null);
        }
        #endregion

        /// <summary>
        ///  Set security stamp
        /// </summary>
        /// <param name="user"></param>
        /// <param name="stamp"></param>
        /// <returns></returns>
        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            user.SecurityStamp = stamp;

            return Task.FromResult(0);

        }

        /// <summary>
        /// Get security stamp
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetSecurityStampAsync(TUser user)
        {
            return Task.FromResult(user.SecurityStamp);
        }

        /// <summary>
        /// Set email on user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task SetEmailAsync(TUser user, string email)
        {
            user.Email = email;
            if (user.Id > 0)
            {
                _userEngine.InstitutionCode = WebUtilities.InstitutionCode;
                _userEngine.Update(user);
            }
            return Task.FromResult(0);

        }

        /// <summary>
        /// Get email from user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetEmailAsync(TUser user)
        {
            return Task.FromResult(user.Email);
        }

        /// <summary>
        /// Get if user email is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        /// <summary>
        /// Set when user email is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <param name="confirmed"></param>
        /// <returns></returns>
        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            user.EmailConfirmed = confirmed;
            if (user.Id > 0)
            {
                _userEngine.InstitutionCode = WebUtilities.InstitutionCode;
                _userEngine.Update(user);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task<TUser> FindByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("email");
            }

            _userEngine.InstitutionCode = WebUtilities.InstitutionCode;
            TUser result = _userEngine.RetrieveByEmail(email) as TUser;
            if (result != null)
            {
                return Task.FromResult(result);
            }

            return Task.FromResult<TUser>(null);
        }

        #region IUserPhoneNumberStore interfaces - TREATED
        /// <summary>
        /// Set user phone number
        /// </summary>
        /// <param name="user"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            user.PhoneNumber = phoneNumber;
            if (user.Id > 0)
            {
                _userEngine.InstitutionCode = WebUtilities.InstitutionCode;
                _userEngine.Update(user);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Get user phone number
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetPhoneNumberAsync(TUser user)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        /// <summary>
        /// Get if user phone number is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        /// <summary>
        /// Set phone number if confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <param name="confirmed"></param>
        /// <returns></returns>
        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            user.PhoneNumberConfirmed = confirmed;
            if (user.Id > 0)
            {
                _userEngine.InstitutionCode = WebUtilities.InstitutionCode;
                _userEngine.Update(user);
            }
            return Task.FromResult(0);
        }
        #endregion


        #region IUserTwoFactorStore interface - TREATED
        /// <summary>
        /// Set two factor authentication is enabled on the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            user.TwoFactorEnabled = enabled;
            if (user.Id > 0)
            {
                _userEngine.InstitutionCode = WebUtilities.InstitutionCode;
                _userEngine.Update(user);
            }
            return Task.FromResult(0);
        }

        /// <summary>
        /// Get if two factor authentication is enabled on the user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }
        #endregion


        /// <summary>
        /// Get user lock out end date
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            return
                Task.FromResult(user.LockoutEndDateUtc.HasValue
                    ? new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc))
                    : new DateTimeOffset());
        }

        /// <summary>
        /// Set user lockout end date
        /// </summary>
        /// <param name="user"></param>
        /// <param name="lockoutEnd"></param>
        /// <returns></returns>
        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            user.LockoutEndDateUtc = lockoutEnd.UtcDateTime;
            if (user.Id > 0)
            {
                _userEngine.InstitutionCode = WebUtilities.InstitutionCode;
                _userEngine.Update(user);
            }
            return Task.FromResult(0);
        }

        /// <summary>
        /// Increment failed access count
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            user.AccessFailedCount++;
            if (user.Id > 0)
            {
                _userEngine.InstitutionCode = WebUtilities.InstitutionCode;
                _userEngine.Update(user);
            }
            return Task.FromResult(user.AccessFailedCount);
        }

        /// <summary>
        /// Reset failed access count
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task ResetAccessFailedCountAsync(TUser user)
        {
            user.AccessFailedCount = 0;
            if (user.Id > 0)
            {
                _userEngine.InstitutionCode = WebUtilities.InstitutionCode;
                _userEngine.Update(user);
            }
            return Task.FromResult(0);
        }

        /// <summary>
        /// Get failed access count
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        /// <summary>
        /// Get if lockout is enabled for the user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        /// <summary>
        /// Set lockout enabled for user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            user.LockoutEnabled = enabled;
            if (user.Id > 0)
            {
                _userEngine.InstitutionCode = WebUtilities.InstitutionCode;
                _userEngine.Update(user);
            }
            return Task.FromResult(0);
        }
    }
}
