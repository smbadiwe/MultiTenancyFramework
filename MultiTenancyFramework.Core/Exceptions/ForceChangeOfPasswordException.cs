namespace MultiTenancyFramework
{
    public class ForceChangeOfPasswordException : GeneralException
    {
        public ForceChangeOfPasswordException() : base("You will need to change your password.")
        {

        }

        public ForceChangeOfPasswordException(string message) : base(message)
        {

        }
    }
}
