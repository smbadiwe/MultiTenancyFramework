using MultiTenancyFramework.Entities;

namespace MultiTenancyFramework.NHibernate.Maps
{
    public class SystemSettingMap : EntityMap<SystemSetting>
    {
        public SystemSettingMap()
        {
            Component(x => x.EmailAndSmtpSetting, y =>
            {
                y.Map(z => z.DefaultEmailSender);
                y.Map(z => z.DefaultEmailReceiver);
                y.Map(z => z.DefaultEmailSubject);
                y.Map(z => z.DefaultSenderDisplayName);
                y.Map(z => z.SmtpUsername);
                y.Map(z => z.SmtpPassword);
                y.Map(z => z.SmtpHost);
                y.Map(z => z.SmtpPort);
                y.Map(z => z.EnableSSL);
            });
            Component(x => x.UsernameAndPasswordRule, y =>
            {
                y.Map(z => z.AllowOnlyAlphanumericUserNames);
                y.Map(z => z.DefaultAccountLockoutTimeSpanInMinutes);
                y.Map(z => z.MaxFailedAccessAttemptsBeforeLockout);
                y.Map(z => z.PasswordRequireDigit);
                y.Map(z => z.PasswordRequiredLength);
                y.Map(z => z.PasswordRequireLowercase);
                y.Map(z => z.PasswordRequireNonLetterOrDigit);
                y.Map(z => z.PasswordRequireUppercase);
                y.Map(z => z.RequireUniqueEmail);
                y.Map(z => z.UserLockoutEnabledByDefault);
            });
        }
    }
}
