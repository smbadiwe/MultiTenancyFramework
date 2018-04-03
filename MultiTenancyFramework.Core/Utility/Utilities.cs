using MultiTenancyFramework.Caching;
using MultiTenancyFramework.Commands;
using MultiTenancyFramework.Data;
using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using System;

namespace MultiTenancyFramework
{
    public class Utilities
    {
        /// <summary>
        /// Default Institution Code
        /// </summary>
        public const string INST_DEFAULT_CODE = "core";

        public static string[] Alphabets = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J","K", "L", "M", "N", "O","P", "Q", "R", "S", "T",
                    "U", "V", "W", "X", "Y","Z","AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ","AK", "AL", "AM", "AN", "AO","AP", "AQ",
                    "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY","AZ"};

        private const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        private static Random random = new Random();
        public static string GenerateRandomAlphanumericText(int length)
        {
            var stringChars = new char[length];

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(stringChars);
        }

        /// <summary>
        /// The property used to mark objects as deleted
        /// </summary>
        public const string SoftDeleteParamName = "deleted";
        public const string SoftDeletePropertyName = "IsDeleted";
        public const string InstitutionFilterName = "InstitutionFilter";
        public const string SoftDeleteFilterName = "SoftDeleteFilter";
        public const string InstitutionCodePropertyName = "InstitutionCode";
        public const string InstitutionCodeQueryParamName = "instCode";

        /// <summary>
        /// The default value is "Password@1". To change it, add key: 'DefaultPassword' to your appSettings in your config file.
        /// <para>When changing, stick to the password rules setup</para>para>
        /// </summary>
        public static string DefaultPassword
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["DefaultPassword"] ?? "Password@1";
            }
        }

        /// <summary>
        /// To set it, add key: 'EntityAssemblies' to your appSettings in your config file. If value is more than one items, use comma-separated list
        /// </summary>
        public static string[] EntityAssemblies
        {
            get
            {
                var items = ConfigurationHelper.AppSettingsItem<string>("EntityAssemblies")?.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (items == null) return new string[0];

                return items;
            }
        }

        public static ILogger Logger
        {
            get
            {
                return MyServiceLocator.GetInstance<ILogger>();
            }
        }

        public static ICommandProcessor CommandProcessor { get { return MyServiceLocator.GetInstance<ICommandProcessor>(); } }
        public static IDbQueryProcessor QueryProcessor { get { return MyServiceLocator.GetInstance<IDbQueryProcessor>(); } }


        private const string SS_SYS_SETTINGS = "::SystemSettings::";

        public static SystemSetting SystemSettings
        {
            get
            {
                var cache = MyServiceLocator.GetInstance<ICacheManager>();

                var item = cache.Get<SystemSetting>(SS_SYS_SETTINGS);
                if (item == null)
                {
                    var dao = MyServiceLocator.GetInstance<ICoreDAO<SystemSetting>>();
                    try
                    {
                        item = dao.RetrieveOne();
                        if (item == null) return new SystemSetting();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                        return new SystemSetting();
                    }
                    cache.Set(SS_SYS_SETTINGS, item, 1440); // 24 hrs
                }
                return item;
            }
            set
            {
                var cache = MyServiceLocator.GetInstance<ICacheManager>();

                cache.Set(SS_SYS_SETTINGS, value, 1440);  // 24 hrs
            }
        }

        public static string GetMimeType(string filename)
        {
            var extension = System.IO.Path.GetExtension(filename)?.ToLower();
            string contentType;
            switch (extension)
            {
                case ".pdf":
                    contentType = "application/pdf";
                    break;
                case ".jpeg":
                case ".jpg":
                    contentType = "image/jpeg";
                    break;
                case ".png":
                    contentType = "image/png";
                    break;
                case ".txt":
                    contentType = "text/plain";
                    break;
                case ".csv":
                    contentType = "text/csv";
                    break;
                case ".docx":
                    contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;
                case ".pptx":
                    contentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                    break;
                case ".xlsx":
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
                case ".zip":
                    contentType = "application/zip";
                    break;
                case ".rar":
                    contentType = "application/x-rar";
                    break;
                case ".json":
                    contentType = "application/json";
                    break;
                case ".xml":
                    contentType = "application/xml";
                    break;
                default:
                    contentType = "application/octet-stream";
                    //contentType = MimeMapping.GetMimeMapping(filename);
                    break;
            }

            return contentType;
        }
    }
}
