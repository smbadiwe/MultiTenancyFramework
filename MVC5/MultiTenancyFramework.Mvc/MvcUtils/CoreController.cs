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
    /// <summary>
    /// Provides core functionalities for the framework, especially authorization and exception handling
    /// </summary>
    public abstract class CoreController : Controller
    {
        private IDbQueryProcessor _queryProcessor;

        /// <summary>
        /// Query Processor to process queries
        /// </summary>
        protected IDbQueryProcessor QueryProcessor
        {
            get
            {
                if (_queryProcessor == null) _queryProcessor = Utilities.QueryProcessor;

                _queryProcessor.InstitutionCode = WebUtilities.InstitutionCode;
                return _queryProcessor;
            }
        }

        private ICommandProcessor _commandProcessor;

        /// <summary>
        /// Command Processor to execute commands
        /// </summary>
        protected ICommandProcessor CommandProcessor
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

        private ILogger _logger;
        /// <summary>
        /// Logger to log errors and/or messages
        /// </summary>
        protected ILogger Logger
        {
            get
            {
                if (_logger == null) _logger = Utilities.Logger;
                return _logger;
            }
        }

        /// <summary>
        /// The (logged in) user as maintained by the framework; distinct from (IPrincipal) User. It's shorthand for 
        /// <code>WebUtilities.GetCurrentlyLoggedInUser();</code>
        /// <para>If you need to set the value, call <code>WebUtilities.SetCurrentlyLoggedInUser(newValue);</code></para>
        /// </summary>
        protected IdentityUser IdentityUser { get { return WebUtilities.GetCurrentlyLoggedInUser(); } }

        /// <summary>
        /// Current datetime
        /// </summary>
        /// <returns></returns>
        protected DateTime Now()
        {
            return DateTime.Now.GetLocalTime();
        }

        private string _viewFolder;

        /// <summary>
        /// The name of the folder where the views for this controller are. This will usually be (and is defauletd to) the controller name
        /// </summary>
        public virtual string ViewFolder
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_viewFolder))
                {
                    _viewFolder = this.GetType().Name.Replace("Controller", "");
                }
                return _viewFolder;
            }
            set
            {
                _viewFolder = value;
            }
        }

        /// <summary>
        /// The area name. Override within an area and set with appropriate value
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

        /// <summary>
        /// Handy way to include <paramref name="institutionCode"/> in the traditional 'RedirectToAction' call
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="institutionCode"></param>
        /// <returns></returns>
        protected internal RedirectToRouteResult RedirectToAction(string actionName, string controllerName, string institutionCode)
        {
            return RedirectToAction(actionName, controllerName, "", institutionCode);
        }

        /// <summary>
        /// Handy way to include <paramref name="areaName"/> and <paramref name="institutionCode"/> in the traditional 'RedirectToAction' call
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="areaName"></param>
        /// <param name="institutionCode"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Redirects to \Error\DenyAccess
        /// </summary>
        /// <returns></returns>
        protected RedirectToRouteResult HttpAccessDenied()
        {
            return RedirectToAction("DenyAccess", "Error", InstitutionCode);
        }

        /// <summary>
        /// Redirects to \Error
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected ViewResult ErrorView(ErrorMessageModel model)
        {
            return View("Error", model);
        }

        protected void AlertSuccess(string message, bool dismissable = true, bool clearModel = true)
        {
            AddAlert(AlertStyles.Success, message, dismissable);
            if (clearModel) ModelState.Clear();
        }

        protected void AlertInformation(string message, bool dismissable = true)
        {
            AddAlert(AlertStyles.Information, message, dismissable);
        }

        protected void AlertWarning(string message, bool dismissable = false)
        {
            AddAlert(AlertStyles.Warning, message, dismissable);
        }

        protected void AlertFailure(string message = "Failure processing request", bool dismissable = false)
        {
            AddAlert(AlertStyles.Danger, message, dismissable);
        }

        protected void AlertFailureInvalidModel(bool dismissable = false)
        {
            AddAlert(AlertStyles.Danger, "One or more data items received is invalid", dismissable);
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
