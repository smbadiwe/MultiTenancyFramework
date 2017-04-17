using System.Web.Mvc;
using System.Linq;
using System;

namespace MultiTenancyFramework.Mvc {
    /// <summary>
    /// Our custom Authorize attribute
    /// </summary>
    public class GlobalExceptionFilterAttribute : FilterAttribute, IExceptionFilter // : HandleErrorAttribute
    {
        private readonly ILogger Logger;
        public GlobalExceptionFilterAttribute() {
            Logger = Utilities.Logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnException(ExceptionContext filterContext) {
            if (!filterContext.ExceptionHandled) {
                filterContext.ExceptionHandled = true;
                var genEx = filterContext.Exception as GeneralException;
                var values = filterContext.RouteData.Values;
                string instCode = Convert.ToString(values["institution"]);
                Logger.Log(new GeneralException(string.Format("Crash from {0}/{1}/{2}", instCode, values["controller"], values["action"]), filterContext.Exception));

                if (genEx != null && genEx.ExceptionType == ExceptionType.UnidentifiedInstitutionCode) {
                    filterContext.Result = MvcUtility.GetPageResult("InvalidUrl", "Error", "", instCode);
                    genEx = null;
                    return;
                }

                bool doLogout = false;
                try {
                    instCode = WebUtilities.InstitutionCode ?? Utilities.INST_DEFAULT_CODE;
                } catch (LogOutUserException) {
                    doLogout = true;
                }
                if (doLogout || filterContext.Exception is LogOutUserException) {
                    WebUtilities.LogOut();
                    filterContext.Result = MvcUtility.GetLoginPageResult(instCode);
                } else {
                    if (filterContext.Exception is System.Data.Common.DbException) {
                        filterContext.Controller.TempData[ErrorMessageModel.ErrorMessageKey] = new ErrorMessageModel("A database error has occurred. Contact the administrator", Convert.ToString(values["controller"]), Convert.ToString(values["action"])) {
                            AreaName = Convert.ToString(values["area"])
                        };
                    } else if (filterContext.Exception is HttpAntiForgeryException) {
                        filterContext.Controller.TempData[ErrorMessageModel.ErrorMessageKey] = new ErrorMessageModel(filterContext.Exception.Message, Convert.ToString(values["controller"]), Convert.ToString(values["action"])) {
                            AreaName = Convert.ToString(values["area"])
                        };
                    } else {
                        filterContext.Controller.TempData[ErrorMessageModel.ErrorMessageKey] = new ErrorMessageModel(filterContext.Exception, Convert.ToString(values["controller"]), Convert.ToString(values["action"])) {
                            AreaName = Convert.ToString(values["area"])
                        };
                    }
                    filterContext.Result = MvcUtility.GetPageResult("Index", "Error", "", instCode);
                }
                return;
            }

        }

    }

}
