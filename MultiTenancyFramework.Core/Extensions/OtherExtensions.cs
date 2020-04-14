namespace MultiTenancyFramework
{
    /// <summary>
    /// More in 'OtherExtensions' in MultiTenancyFramework.Utils
    /// </summary>
    public static class OtherExtensions
    {
        public static void SetNLogLogger(this ILogger logger, string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
                logger.SetLogger(NLog.LogManager.GetLogger(name));
            else
                logger.SetLogger(null);
        }
   }
}
