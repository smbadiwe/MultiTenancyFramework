using System;
using System.Web.Mvc;

namespace MultiTenancyFramework.Mvc
{
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
                var values = filterContext.RouteData.Values;
                string instCode = Convert.ToString(values["institution"]);
                var urlAccessed = string.Format("{0}/{1}/{2}/{3}", instCode, values["area"], values["controller"], values["action"]);
                Logger.Log(new GeneralException(string.Format("Crash from {0}", urlAccessed), filterContext.Exception));

                var genEx = filterContext.Exception as GeneralException;
                if (genEx != null) {
                    if (genEx.ExceptionType == ExceptionType.UnidentifiedInstitutionCode) {
                        filterContext.Controller.TempData[ErrorMessageModel.ErrorMessageKey] = new ErrorMessageModel("Invalid Url. Please cross-check.", Convert.ToString(values["controller"]), Convert.ToString(values["action"])) {
                            AreaName = Convert.ToString(values["area"]),
                            FromUrl = urlAccessed
                        };
                    } else if (genEx.ExceptionType == ExceptionType.DatabaseRelated) {
                        filterContext.Controller.TempData[ErrorMessageModel.ErrorMessageKey] = new ErrorMessageModel("A database error has occurred. Contact the administrator", Convert.ToString(values["controller"]), Convert.ToString(values["action"])) {
                            AreaName = Convert.ToString(values["area"]),
                            FromUrl = urlAccessed
                        };
                    }
                    filterContext.Result = MvcUtility.GetPageResult("Index", "Error", "", instCode);
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
                            AreaName = Convert.ToString(values["area"]),
                            FromUrl = urlAccessed
                        };
                    } else if (filterContext.Exception is HttpAntiForgeryException) {
                        filterContext.Controller.TempData[ErrorMessageModel.ErrorMessageKey] = new ErrorMessageModel(filterContext.Exception.Message, Convert.ToString(values["controller"]), Convert.ToString(values["action"])) {
                            AreaName = Convert.ToString(values["area"]),
                            FromUrl = urlAccessed
                        };
                    } else {
                        filterContext.Controller.TempData[ErrorMessageModel.ErrorMessageKey] = new ErrorMessageModel(filterContext.Exception, Convert.ToString(values["controller"]), Convert.ToString(values["action"])) {
                            AreaName = Convert.ToString(values["area"]),
                            FromUrl = urlAccessed
                        };
                    }
                    filterContext.Result = MvcUtility.GetPageResult("Index", "Error", "", instCode);
                }
                return;
            }

        }

    }

}
