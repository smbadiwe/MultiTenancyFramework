using MultiTenancyFramework.Commands;
using MultiTenancyFramework.Data.Queries;
using System;

namespace MultiTenancyFramework
{
    public class Utilities
    {
        /// <summary>
        /// Default Institution Code
        /// </summary>
        public const string INST_DEFAULT_CODE = "Core";

        public static string[] Alphabets = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J","K", "L", "M", "N", "O","P", "Q", "R", "S", "T",
                    "U", "V", "W", "X", "Y","Z","AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ","AK", "AL", "AM", "AN", "AO","AP", "AQ",
                    "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY","AZ"};

        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
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
        public const string SoftDeletePropertyName = "IsDeleted";
        public const string InstitutionFilterName = "InstitutionFilter";
        public const string SoftDeleteFilterName = "SoftDeleteFilter";
        public const string InstitutionCodePropertyName = "InstitutionCode";
        public const string InstitutionCodeQueryParamName = "instCode";

        public const string SS_SYS_SETTINGS = "::SystemSettings::";
        
        public static string DefaultPassword
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["DefaultPassword"] ?? "Password@1";
            }
        }

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
    }
}
