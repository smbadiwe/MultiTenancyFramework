using MultiTenancyFramework.Commands;
using MultiTenancyFramework.Data.Queries;
using MultiTenancyFramework.Entities;
using MultiTenancyFramework.Mvc.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MultiTenancyFramework.Mvc
{
    public abstract class CoreController : Controller
    {
        private IDbQueryProcessor _queryProcessor;

        public IDbQueryProcessor QueryProcessor
        {
            get
            {
                if (_queryProcessor == null) _queryProcessor = Utilities.QueryProcessor;

                _queryProcessor.InstitutionCode = WebUtilities.InstitutionCode;
                return _queryProcessor;
            }
        }

        private ICommandProcessor _commandProcessor;

        public ICommandProcessor CommandProcessor
        {
            get
            {
                if (_commandProcessor == null) _commandProcessor = Utilities.CommandProcessor;

                return _commandProcessor;
            }
        }

        /// <summary>
        /// = RouteData.Values["institution"]. It's appearing in too many controllers now.
        /// BTW, there is also WebUtilities.InstitutionCode which essentially same, just that that one
        /// will force logout if Session has expired, unlike this one. So, use with caution
        /// </summary>
        protected string InstitutionCode
        {
            get
            {
                return Convert.ToString(RouteData.Values["institution"]);
            }
        }

        protected readonly ILogger Logger;

        /// <summary>
        /// The (logged in) user as maintained by the framework; distinct from (IPrincipal) User
        /// </summary>
        protected IdentityUser IdentityUser { get; set; }

        public CoreController()
        {
            Logger = Utilities.Logger;
        }

        /// <summary>
        /// Current datetime
        /// </summary>
        /// <returns></returns>
        protected DateTime Now()
        {
            return DateTime.Now.GetLocalTime();
        }

        /// <summary>
        /// The name of the folder where the views for this controller are. This will usually be the controller name
        /// </summary>
        public virtual string ViewFolder { get { return string.Empty; } }

        /// <summary>
        /// The area name. Set to empty string if not applicable
        /// </summary>
        public virtual string AreaName { get { return string.Empty; } }

        /// <summary>
        /// This gets to direct view filename, stopping the unnecesary work MVC does to find the views in expected folders.
        /// <para>We know where the views are, and there's a convention - ~/Views/{ViewFolder}/{viewName}.cshtml OR ~/Areas/{AreaName}/Views/{ViewFolder}/{viewName}.cshtml</para>
        /// </summary>
        /// <param name="viewName"></param>
        protected virtual string GetViewName(string viewName = "Index")
        {
            if (string.IsNullOrWhiteSpace(ViewFolder)) return viewName;

            if (string.IsNullOrWhiteSpace(AreaName))
            {
                return $"~/Views/{ViewFolder}/{viewName}.cshtml";
            }
            return $"~/Areas/{AreaName}/Views/{ViewFolder}/{viewName}.cshtml";
        }

        protected virtual void AlertSuccess(string message, bool dismissable = true, bool clearModel = true)
        {
            AddAlert(AlertStyles.Success, message, dismissable);
            if (clearModel) ModelState.Clear();
        }

        protected internal RedirectToRouteResult RedirectToAction(string actionName, string controllerName, string institutionCode)
        {
            return RedirectToAction(actionName, controllerName, "", institutionCode);
        }

        protected internal RedirectToRouteResult RedirectToAction(string actionName, string controllerName, string areaName, string institutionCode)
        {
            if (string.IsNullOrWhiteSpace(actionName)) throw new ArgumentNullException("action");
            var routes = new System.Web.Routing.RouteValueDictionary();
            if (!string.IsNullOrWhiteSpace(controllerName))
            {
                routes.Add("controller", controllerName);
            }
            if (!string.IsNullOrWhiteSpace(areaName))
            {
                routes.Add("area", areaName);
            }
            else
            {
                if (controllerName == "Error")
                {
                    routes.Add("area", "");
                }
            }
            if (!string.IsNullOrWhiteSpace(institutionCode))
            {
                routes.Add("institution", institutionCode);
            }
            return RedirectToAction(actionName, routes);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                filterContext.ExceptionHandled = true;
                var genEx = filterContext.Exception as GeneralException;
                var values = filterContext.RouteData.Values;
                if (genEx != null && genEx.ExceptionType == ExceptionType.UnidentifiedInstitutionCode)
                {
                    filterContext.Result = RedirectToAction("InvalidUrl", "Error", Convert.ToString(values["institution"]));
                    genEx = null;
                    return;
                }

                string instCode = Utilities.INST_DEFAULT_CODE;
                bool doLogout = false;
                try
                {
                    instCode = WebUtilities.InstitutionCode ?? Utilities.INST_DEFAULT_CODE;
                }
                catch (LogOutUserException)
                {
                    doLogout = true;
                }
                Logger.Log(new GeneralException(string.Format("Crash from {0}/{1}/{2}", instCode, values["controller"], values["action"]), filterContext.Exception));
                if (doLogout || filterContext.Exception is LogOutUserException)
                {
                    WebUtilities.LogOut();
                    filterContext.Result = MvcUtility.GetLoginPageResult(instCode);
                }
                else
                {
                    if (filterContext.Exception is HttpAntiForgeryException)
                    {
                        TempData[ErrorMessageModel.ErrorMessageKey] = new ErrorMessageModel(filterContext.Exception.Message);
                    }
                    else
                    {
                        TempData[ErrorMessageModel.ErrorMessageKey] = new ErrorMessageModel(filterContext.Exception);
                    }
                    filterContext.Result = RedirectToAction("Index", "Error", instCode);
                }
                return;
            }
            base.OnException(filterContext);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var actionDescriptor = filterContext.ActionDescriptor;

            #region Check whether it's an anonymous action
            
            // check if AllowAnonymous is on the controller
            var anonymous = actionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true)
                    .Cast<AllowAnonymousAttribute>();
            if (anonymous.Any())
            {
                //Allow Anonymous
                anonymous = null;
                base.OnActionExecuting(filterContext);
                return;
            }

            // It's not; so check if AllowAnonymous is on the action
            anonymous = actionDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true)
                    .Cast<AllowAnonymousAttribute>();
            if (anonymous.Any())
            {
                //Allow Anonymous
                anonymous = null;
                base.OnActionExecuting(filterContext);
                return;
            }
            anonymous = null;

            #endregion

            // If user is not logged in (authenticated) yet, 
            IdentityUser = WebUtilities.GetCurrentlyLoggedInUser();
            if (IdentityUser == null)
            {
                // It's not anonymous, so force user to login
                WebUtilities.LogOut();
                filterContext.Result = MvcUtility.GetLoginPageResult(InstitutionCode);
                return;
            }

            // At this point, we have established that we have a logged-in user. So...
            #region Authorize at Privilege level

            var userPrivList = WebUtilities.LoggedInUsersPrivilegesDict;
            //This should never be true under normal circumstances, 'cos a properly logged-in user
            // should have at least one user privllege
            if (userPrivList == null)
            {
                WebUtilities.LogOut();
                filterContext.Result = MvcUtility.GetLoginPageResult(InstitutionCode);
                return;
            }

            // OK. So the user has some privileges. So...
            string privilegeName = string.Format("{0}-{1}-{2}",
                   actionDescriptor.ActionName,
                   actionDescriptor.ControllerDescriptor.ControllerName,
                   filterContext.RouteData.Values["area"]);

            if (!userPrivList.ContainsKey(privilegeName))
            {
                //Normally, I use a convention where I have a 'GetData' action for every 
                // 'Index' action that is used to view a list of stuffs (entities).
                if ("GetData" == actionDescriptor.ActionName &&
                    userPrivList.ContainsKey(string.Format("{0}-{1}-{2}",
                            "Index", actionDescriptor.ControllerDescriptor.ControllerName, filterContext.RouteData.Values["area"])))
                {
                    base.OnActionExecuting(filterContext);
                    return;
                }

                //The generalized case of the above 'GetData' trick 
                var point = actionDescriptor.GetCustomAttributes(typeof(ValidateUsingPrivilegeForActionAttribute), true)
                        .Cast<ValidateUsingPrivilegeForActionAttribute>().FirstOrDefault();
                if (point != null)
                {
                    foreach (var actionName in point.ActionNames)
                    {
                        if (userPrivList.ContainsKey(string.Format("{0}-{1}-{2}",
                            actionName, actionDescriptor.ControllerDescriptor.ControllerName, filterContext.RouteData.Values["area"])))
                        {
                            base.OnActionExecuting(filterContext);
                            return;
                        }
                    }
                }
                filterContext.Result = HttpAccessDenied();
                return;
            }

            #endregion

            // fall back to base
            base.OnActionExecuting(filterContext);
        }

        protected virtual RedirectToRouteResult HttpAccessDenied()
        {
            return RedirectToAction("DenyAccess", "Error", InstitutionCode);
        }

        protected virtual ViewResult ErrorView(ErrorMessageModel model)
        {
            return View("Error", model);
        }

        protected virtual void AlertInformation(string message, bool dismissable = true)
        {
            AddAlert(AlertStyles.Information, message, dismissable);
        }

        protected virtual void AlertWarning(string message, bool dismissable = false)
        {
            AddAlert(AlertStyles.Warning, message, dismissable);
        }

        protected virtual void AlertFailure(string message = "Failure processing request", bool dismissable = false)
        {
            AddAlert(AlertStyles.Danger, message, dismissable);
        }

        protected virtual void AlertFailureInvalidModel(bool dismissable = false)
        {
            AddAlert(AlertStyles.Danger, "Invalid data POSTed", dismissable);
        }

        private void AddAlert(string alertStyle, string message, bool dismissable)
        {
            var alerts = TempData.ContainsKey(Alert.TempDataKey)
                ? (List<Alert>)TempData[Alert.TempDataKey]
                : new List<Alert>();

            alerts.Add(new Alert
            {
                AlertStyle = alertStyle,
                Message = message,
                Dismissable = dismissable
            });

            TempData[Alert.TempDataKey] = alerts;
        }

    }
}
