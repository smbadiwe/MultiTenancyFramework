
namespace MultiTenancyFramework
{
    public class LogOutUserException : GeneralException
    {
        public LogOutUserException() : base("Log user out. Redirect to login page")
        {
        }
        
        public LogOutUserException(string message) : base(message)
        {
        }
    }
}
