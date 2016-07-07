using System;

namespace MultiTenancyFramework
{
    /// <summary>
    /// TODO: Figure out what other special thing to do with this exception.
    /// Maybe throw an Error ID and a nicer message. Think through later.
    /// </summary>
    public class GeneralException : Exception
    {
        public override string Message
        {
            get
            {
                return base.Message;
            }
        }

        public ExceptionType ExceptionType { get; set; }
        public GeneralException(string message, ExceptionType exceptionType = ExceptionType.InvalidUserActionOrInput) : base(message)
        {
            ExceptionType = exceptionType;
        }

        public GeneralException(string message, Exception innerException, ExceptionType exceptionType = ExceptionType.InvalidUserActionOrInput) : base(message, innerException)
        {
            ExceptionType = exceptionType;
        }
    }

    public enum ExceptionType
    {
        InvalidUserActionOrInput,
        NoMoreDbForSchools,
        UnidentifiedInstitutionCode,
        DatabaseRelated,
    }
}
