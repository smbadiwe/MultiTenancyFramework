using System;
using System.Collections.Specialized;

namespace MultiTenancyFramework
{
    /// <summary>
    /// Facilitates info retrieval from Web.config file
    /// </summary>
    public class ConfigurationHelper
    {
        public static NameValueCollection GetAssemblyConfig<T>()
        {
            return System.Configuration.ConfigurationManager.GetSection(typeof(T).Assembly.FullName.Split(',')[0]) as NameValueCollection;
        }

        public static NameValueCollection GetAssemblyConfig(string sectionName)
        {
            return System.Configuration.ConfigurationManager.GetSection(sectionName) as NameValueCollection;
        }

        public static R ConfigItem<T, R>(string key)
        {
            R result = default(R);
            if (string.IsNullOrWhiteSpace(key)) return result;

            var nvc = System.Configuration.ConfigurationManager.GetSection(typeof(T).Assembly.FullName.Split(',')[0]) as NameValueCollection;
            if (nvc != null)
            {
                try
                {
                    result = (R)Convert.ChangeType(nvc[key], typeof(R));
                }
                catch { }
            }
            return result;
        }

        /// <summary>
        /// Gets the App-Settings item with the given key.
        /// </summary>
        /// <typeparam name="TR">The type of the item value.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static TR AppSettingsItem<TR>(string key)
        {
            TR result = default(TR);
            if (string.IsNullOrWhiteSpace(key)) return result;

            var theValue = System.Configuration.ConfigurationManager.AppSettings[key];
            if (!string.IsNullOrWhiteSpace(theValue))
            {
                try
                {
                    result = (TR)Convert.ChangeType(theValue, typeof(TR));
                }
                catch { }
            }
            return result;
        }

        /// <summary>
        /// Gets the item with the given key from the given Section Item .
        /// </summary>
        /// <typeparam name="TR">The type of the item value.</typeparam>
        /// <param name="sectionName">The section name.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static TR SectionItem<TR>(string sectionName, string key)
        {
            TR result = default(TR);
            if (string.IsNullOrWhiteSpace(key)) return result;
            if (string.IsNullOrWhiteSpace(sectionName)) return result;

            var theSection = System.Configuration.ConfigurationManager.GetSection(sectionName) as NameValueCollection;
            if (theSection != null)
            {
                var theValue = theSection[key];
                if (string.IsNullOrWhiteSpace(theValue)) return result;

                try
                {
                    result = (TR)Convert.ChangeType(theValue, typeof(TR));
                }
                catch { }
            }
            return result;
        }

        public static string GetSiteUrl()
        {
            var url = AppSettingsItem<string>("SiteUrl") ?? string.Empty;
            if (!url.EndsWith("/"))
                url = url + "/";

            return url;
        }
    }
}
