using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using System.Web.Routing;

namespace MultiTenancyFramework.Mvc
{
    /// <summary>
    /// Our custom Authorize attribute
    /// </summary>
    public class GlobalExceptionFilterAttribute : FilterAttribute, IExceptionFilter // : HandleErrorAttribute
    {
        private readonly ILogger Logger;
        public GlobalExceptionFilterAttribute()
        {
            Logger = Utilities.Logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                filterContext.ExceptionHandled = true;
                var values = filterContext.RouteData.Values;
                string instCode = Convert.ToString(values["institution"]);

                // When view is not found, it usually throws 
                //Exception Details: System.InvalidOperationException: 
                // The view '~/Views/my-category/my-article-with-long-name.aspx' or its master could not be found. The following locations were searched: ~/Views/my-category/my-article-with-long-name.aspx
                if (filterContext.Exception is InvalidOperationException && filterContext.Exception.Message.Contains("The view '~/Views"))
                {
                    Logger.Log(filterContext.Exception.Message);
                    filterContext.Result = MvcUtility.GetPageResult("ViewNotFound", "Error", "", instCode);
                    return;
                }

                string area = Convert.ToString(values["area"]);
                string controller = Convert.ToString(values["controller"]);
                string action = Convert.ToString(values["action"]);
                var urlAccessed = string.Format("{0}/{1}/{2}/{3}", instCode, area, controller, action);
                Logger.Log(new GeneralException(string.Format("Crash from {0}", urlAccessed), filterContext.Exception));

                var genEx = filterContext.Exception as GeneralException;
                if (genEx != null)
                {
                    if (genEx.ExceptionType == ExceptionType.UnidentifiedInstitutionCode)
                    {
                        filterContext.Controller.TempData[ErrorMessageModel.ErrorMessageKey] = new ErrorMessageModel("Invalid Url. Please cross-check.", controller, action)
                        {
                            ErrorType = ExceptionType.UnidentifiedInstitutionCode,
                            AreaName = area,
                            FromUrl = urlAccessed,
                            ResponseCode = HttpStatusCode.NotFound,
                        };
                    }
                    else if (genEx.ExceptionType == ExceptionType.DatabaseRelated)
                    {
                        filterContext.Controller.TempData[ErrorMessageModel.ErrorMessageKey] = new ErrorMessageModel("A database error has occurred. Contact the administrator", controller, action)
                        {
                            ErrorType = ExceptionType.DatabaseRelated,
                            AreaName = area,
                            FromUrl = urlAccessed,
                            ResponseCode = HttpStatusCode.InternalServerError,
                        };
                    }
                    else
                    {
                        filterContext.Controller.TempData[ErrorMessageModel.ErrorMessageKey] = new ErrorMessageModel(genEx, controller, action)
                        {
                            AreaName = area,
                            FromUrl = urlAccessed
                        };
                    }
                    filterContext.Result = MvcUtility.GetPageResult("Index", "Error", "", instCode);
                    return;
                }

                bool doLogout = false;
                try
                {
                    instCode = WebUtilities.InstitutionCode ?? Utilities.INST_DEFAULT_CODE;
                }
                catch (LogOutUserException)
                {
                    doLogout = true;
                }
                if (doLogout || filterContext.Exception is LogOutUserException)
                {
                    WebUtilities.LogOut();
                    filterContext.Result = MvcUtility.GetLoginPageResult(instCode);
                }
                else
                {
                    var dbExType = typeof(System.Data.Common.DbException);
                    if (dbExType.IsAssignableFrom(filterContext.Exception.GetType())
                        || (filterContext.Exception.GetBaseException() != null && dbExType.IsAssignableFrom(filterContext.Exception.GetBaseException().GetType())))
                    {
                        filterContext.Controller.TempData[ErrorMessageModel.ErrorMessageKey] = new ErrorMessageModel("A database error has occurred. Contact the administrator", controller, action)
                        {
                            ErrorType = ExceptionType.DatabaseRelated,
                            AreaName = area,
                            FromUrl = urlAccessed,
                            ResponseCode = HttpStatusCode.InternalServerError,
                        };
                    }
                    else if (filterContext.Exception is HttpAntiForgeryException)
                    {
                        filterContext.Controller.TempData[ErrorMessageModel.ErrorMessageKey] = new ErrorMessageModel("Looks like this is a cross-site request forgery. We can't find the token.", controller, action)
                        {
                            AreaName = area,
                            FromUrl = urlAccessed
                        };
                    }
                    else
                    {
                        filterContext.Controller.TempData[ErrorMessageModel.ErrorMessageKey] = new ErrorMessageModel(filterContext.Exception, controller, action)
                        {
                            AreaName = area,
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
