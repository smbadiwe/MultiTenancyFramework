using System;
using System.Net;
using System.Web.Mvc;

namespace MultiTenancyFramework.Mvc
{
    /// <summary>
    /// Our custom Authorize attribute
    /// </summary>
    public class GlobalExceptionFilterAttribute : FilterAttribute, IExceptionFilter // : HandleErrorAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                filterContext.ExceptionHandled = true;
                bool isFatal = false;
                try
                {
                    bool doLogout = false;
                    var values = filterContext.RouteData.Values;
                    string instCode = Convert.ToString(values["institution"]);
                    try
                    {
                        var _instCode = WebUtilities.InstitutionCode ?? Utilities.INST_DEFAULT_CODE;
                        if (!instCode.Equals(_instCode, StringComparison.OrdinalIgnoreCase))
                        {
                            instCode = Utilities.INST_DEFAULT_CODE;
                            doLogout = true;
                        }
                    }
                    catch (LogOutUserException)
                    {
                        doLogout = true;
                    }
                    catch (Exception) //(GeneralException ex) when (ex.ExceptionType == ExceptionType.UnidentifiedInstitutionCode)
                    {
                        instCode = Utilities.INST_DEFAULT_CODE;
                    }

                    // When view is not found, it usually throws 
                    //Exception Details: System.InvalidOperationException: 
                    // The view '~/Views/my-category/my-article-with-long-name.aspx' or its master could not be found. The following locations were searched: ~/Views/my-category/my-article-with-long-name.aspx
                    if (filterContext.Exception is InvalidOperationException && filterContext.Exception.Message.Contains("The view '~/Views"))
                    {
                        isFatal = true;
                        filterContext.Result = MvcUtility.GetPageResult("ViewNotFound", "Error", "", instCode);
                        return;
                    }

                    var urlAccessed = filterContext.RequestContext.HttpContext.Request.RawUrl; // string.Format("/{0}{1}/{2}/{3}", instCode, string.IsNullOrWhiteSpace(area) ? "" : ("/" + area), controller, action);

                    string area = Convert.ToString(values["area"]);
                    string controller = Convert.ToString(values["controller"]);
                    string action = Convert.ToString(values["action"]);
                    var genEx = filterContext.Exception as GeneralException;
                    if (genEx != null)
                    {
                        if (genEx.ExceptionType == ExceptionType.UnidentifiedInstitutionCode)
                        {
                            instCode = Utilities.INST_DEFAULT_CODE;
                            filterContext.Controller.TempData[ErrorMessageModel.ErrorMessageKey] = new ErrorMessageModel("Invalid Url. Please cross-check.", controller, action)
                            {
                                ErrorType = ExceptionType.UnidentifiedInstitutionCode,
                                AreaName = area,
                                FromUrl = urlAccessed,
                                ResponseCode = HttpStatusCode.NotFound,
                                RenderErrorPageFully = true,
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
                            filterContext.Result = MvcUtility.GetPageResult("ReLogin", "Error", "", instCode);

                            //filterContext.Controller.TempData[ErrorMessageModel.ErrorMessageKey] = new ErrorMessageModel("Looks like this is a cross-site request forgery. We can't find the token.", controller, action)
                            //{
                            //    AreaName = area,
                            //    FromUrl = urlAccessed,
                            //    ErrorType = ExceptionType.Security,
                            //    ResponseCode = HttpStatusCode.PreconditionFailed,
                            //};
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
                }
                finally
                {
                    Utilities.Logger.Log(filterContext.Exception, isFatal);
                }
            }

        }

    }
}
