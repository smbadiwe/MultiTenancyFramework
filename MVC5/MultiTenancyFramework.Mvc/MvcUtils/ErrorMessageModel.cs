using System;
using System.Net;
using System.Web.Mvc;

namespace MultiTenancyFramework.Mvc
{
    public class ErrorMessageModel : HandleErrorInfo
    {
        public const string ErrorMessageKey = "ErrorMessageKey";
        /// <summary>
        /// The (MVC) area name
        /// </summary>
        public string AreaName { get; set; }
        public string ErrorMessage { get; private set; }
        public string StackTrace { get; private set; }
        /// <summary>
        /// typeof(Exception): The exception's type.
        /// </summary>
        public Type ExceptionType { get; private set; }
        /// <summary>
        /// Error type
        /// </summary>
        public ExceptionType? ErrorType { get; set; }
        /// <summary>
        /// Whether or not to render the resulting page fully or within the section
        /// </summary>
        public bool RenderErrorPageFully { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="renderErrorPageFully"></param>
        public ErrorMessageModel(bool renderErrorPageFully = false) : this(string.Empty, renderErrorPageFully)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="renderErrorPageFully"></param>
        public ErrorMessageModel(string errorMsg, bool renderErrorPageFully = false)
            : this(errorMsg, "", "", renderErrorPageFully)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <param name="renderErrorPageFully"></param>
        public ErrorMessageModel(string errorMsg, string controllerName, string actionName, bool renderErrorPageFully = false)
            : base(new GeneralException("", MultiTenancyFramework.ExceptionType.DoNothing), controllerName, actionName)
        {
            if (string.IsNullOrWhiteSpace(errorMsg)) errorMsg = "An error occurred while processing your request.";
            ErrorMessage = WebUtility.HtmlDecode(errorMsg);
            RenderErrorPageFully = renderErrorPageFully;
            ErrorType = MultiTenancyFramework.ExceptionType.InvalidUserActionOrInput;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="renderErrorPageFully"></param>
        public ErrorMessageModel(Exception ex, bool renderErrorPageFully = false) 
            : base(ex, "", "")
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <param name="renderErrorPageFully"></param>
        public ErrorMessageModel(Exception exception, string controllerName, string actionName, bool renderErrorPageFully = false)
            : base(exception, controllerName, actionName)
        {

        }
    }

}
