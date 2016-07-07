namespace MultiTenancyFramework.Data
{
    public interface IDataInitializer
    {
        /// <summary>
        /// This picks session factory parameters from the web.config file
        /// </summary>
        /// <param name="isWeb"></param>
        void Init(bool isWeb = true);

        void Terminate(bool isWeb = true);
    }
}
