using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.NHibernate.Maps
{
    public sealed class AppUserMap : AppUserMap<AppUser>
    {
    }

    public class AppUserMap<T> : PersonMap<T> where T : AppUser
    {
        public AppUserMap()
        {
            Table(ConfigurationHelper.AppSettingsItem<bool>("UseLowercaseTableNames") ? "users" : "Users");
            Map(x => x.UserName).Index("ind_username");
            Map(x => x.PasswordHash);
            Map(x => x.UserRoles);

            Map(x => x.ForceChangeOfPassword);

            Map(x => x.LockoutEnabled);
            Map(x => x.LockoutEndDateUtc);
            Map(x => x.AccessFailedCount);
            Map(x => x.PhoneNumberConfirmed);
            Map(x => x.TwoFactorEnabled);
            Map(x => x.SecurityStamp);
            Map(x => x.EmailConfirmed);
        }
    }
}
