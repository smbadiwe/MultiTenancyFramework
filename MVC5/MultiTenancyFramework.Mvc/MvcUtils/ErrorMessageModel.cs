using System;
using System.Net;

namespace MultiTenancyFramework.Mvc
{
    public class ErrorMessageModel
    {
        public const string ErrorMessageKey = "ErrorMessageKey";
        public string ErrorMessage { get; private set; }
        public string StackTrace { get; private set; }
        public Type ExceptionType { get; private set; }
        public ExceptionType? ErrorType { get; set; }
        public bool RenderErrorPageFully { get; set; }
        public ErrorMessageModel(bool renderErrorPageFully = false) : this(string.Empty, renderErrorPageFully)
        {
        }

        public ErrorMessageModel(string errorMsg, bool renderErrorPageFully = false)
        {
            if (string.IsNullOrWhiteSpace(errorMsg)) errorMsg = "An error occurred while processing your request.";
            ErrorMessage = WebUtility.HtmlDecode(errorMsg);
            RenderErrorPageFully = renderErrorPageFully;
            ErrorType = MultiTenancyFramework.ExceptionType.InvalidUserActionOrInput;
        }

        public ErrorMessageModel(Exception ex, bool renderErrorPageFully = false)
        {
            ErrorMessage = ex.GetFullExceptionMessage();
            StackTrace = ex.StackTrace;
            RenderErrorPageFully = renderErrorPageFully;
            ExceptionType = ex.GetType();
            if (ExceptionType == typeof(GeneralException))
            {
                ErrorType = (ex as GeneralException).ExceptionType;
            }
        }

    }

}
