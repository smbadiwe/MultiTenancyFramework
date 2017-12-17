using System.Collections.Generic;
using System.Web.Routing;

namespace System.Web.Mvc.Html
{
    public static class MvcFormExtension
    {
        /// <summary>Writes an opening &lt;form&gt; tag to the response. The form uses the GET method, and the request is processed by the action method for the view.</summary>
        /// <returns>An opening &lt;form&gt; tag. </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        public static MvcForm BeginGETForm(this HtmlHelper htmlHelper)
        {
            return BeginFormGeneral(htmlHelper, null, null, null, FormMethod.Get, null);
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response. The form uses the GET method, and the request is processed by the action method for the view.</summary>
        /// <returns>An opening &lt;form&gt; tag. </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        public static MvcForm BeginGETForm(this HtmlHelper htmlHelper, object htmlAttributes)
        {
            return BeginFormGeneral(htmlHelper, null, null, null, FormMethod.Get, htmlAttributes);
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response. The form uses the GET method, and the request is processed by the action method for the view.</summary>
        /// <returns>An opening &lt;form&gt; tag. </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method.</param>
        public static MvcForm BeginGETForm(this HtmlHelper htmlHelper, string actionName)
        {
            return BeginFormGeneral(htmlHelper, actionName, null, null, FormMethod.Get, null);
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response and sets the action tag to the specified controller and action. The form uses the POST method.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        public static MvcForm BeginGETForm(this HtmlHelper htmlHelper, string actionName, string controllerName)
        {
            return BeginFormGeneral(htmlHelper, actionName, controllerName, null, FormMethod.Get, null);
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response. The form uses the POST method, and the request is processed by the action method for the view.</summary>
        /// <returns>An opening &lt;form&gt; tag. </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        public static MvcForm BeginFormWithXsrf(this HtmlHelper htmlHelper)
        {
            var mvcForm = htmlHelper.BeginForm();
            htmlHelper.ViewContext.Writer.Write(htmlHelper.AntiForgeryToken().ToHtmlString());
            return mvcForm;
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response and sets the action tag to the specified action. The form uses the POST method.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method.</param>
        public static MvcForm BeginFormWithXsrf(this HtmlHelper htmlHelper, string actionName)
        {
            return BeginFormGeneral(htmlHelper, actionName, null, null, FormMethod.Post, null);
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response and sets the action tag to the specified action. The form uses the POST method.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        public static MvcForm BeginFormWithXsrf(this HtmlHelper htmlHelper, string actionName, object htmlAttributes)
        {
            return BeginFormGeneral(htmlHelper, actionName, null, null, FormMethod.Post, htmlAttributes);
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response and includes the route values from the route value dictionary in the action attribute. The form uses the POST method, and the request is processed by the action method for the view.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        public static MvcForm BeginFormWithXsrf(this HtmlHelper htmlHelper, RouteValueDictionary routeValues)
        {
            return BeginFormGeneral(htmlHelper, null, null, routeValues, FormMethod.Post, new RouteValueDictionary());
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response and sets the action tag to the specified controller and action. The form uses the POST method.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        public static MvcForm BeginFormWithXsrf(this HtmlHelper htmlHelper, string actionName, string controllerName)
        {
            return BeginFormGeneral(htmlHelper, actionName, controllerName, null, FormMethod.Post, null);
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response, and sets the action tag to the specified controller, action, and route values. The form uses the POST method.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.</param>
        public static MvcForm BeginFormWithXsrf(this HtmlHelper htmlHelper, string actionName, string controllerName, object routeValues)
        {
            return BeginFormGeneral(htmlHelper, actionName, controllerName, new RouteValueDictionary(routeValues), FormMethod.Post, null);
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response, and sets the action tag to the specified controller, action, and route values from the route value dictionary. The form uses the POST method.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        public static MvcForm BeginFormWithXsrf(this HtmlHelper htmlHelper, string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            return BeginFormGeneral(htmlHelper, actionName, controllerName, routeValues, FormMethod.Post, null);
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response and sets the action tag to the specified controller and action. The form uses the specified HTTP method.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="method">The HTTP method for processing the form, either GET or POST.</param>
        public static MvcForm BeginFormGeneral(this HtmlHelper htmlHelper, string actionName, string controllerName, FormMethod method)
        {
            return BeginFormGeneral(htmlHelper, actionName, controllerName, null, method, null);
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response and sets the action tag to the specified controller, action, and route values. The form uses the specified HTTP method.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.</param>
        /// <param name="method">The HTTP method for processing the form, either GET or POST.</param>
        public static MvcForm BeginFormGeneral(this HtmlHelper htmlHelper, string actionName, string controllerName, object routeValues, FormMethod method)
        {
            return BeginFormGeneral(htmlHelper, actionName, controllerName, new RouteValueDictionary(routeValues), method, null);
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response, and sets the action tag to the specified controller, action, and route values from the route value dictionary. The form uses the specified HTTP method.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        /// <param name="method">The HTTP method for processing the form, either GET or POST.</param>
        public static MvcForm BeginFormGeneral(this HtmlHelper htmlHelper, string actionName, string controllerName, RouteValueDictionary routeValues, FormMethod method)
        {
            return BeginFormGeneral(htmlHelper, actionName, controllerName, routeValues, method, new RouteValueDictionary());
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response and sets the action tag to the specified controller and action. The form uses the specified HTTP method and includes the HTML attributes.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="method">The HTTP method for processing the form, either GET or POST.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        public static MvcForm BeginFormGeneral(this HtmlHelper htmlHelper, string actionName, string controllerName, FormMethod method, object htmlAttributes)
        {
            return BeginFormGeneral(htmlHelper, actionName, controllerName, new RouteValueDictionary(), method, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response and sets the action tag to the specified controller and action. The form uses the specified HTTP method and includes the HTML attributes from a dictionary.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="method">The HTTP method for processing the form, either GET or POST.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        public static MvcForm BeginFormGeneral(this HtmlHelper htmlHelper, string actionName, string controllerName, FormMethod method, IDictionary<string, object> htmlAttributes)
        {
            return BeginFormGeneral(htmlHelper, actionName, controllerName, null, method, htmlAttributes);
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response and sets the action tag to the specified controller, action, and route values. The form uses the specified HTTP method and includes the HTML attributes.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.</param>
        /// <param name="method">The HTTP method for processing the form, either GET or POST.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        public static MvcForm BeginFormGeneral(this HtmlHelper htmlHelper, string actionName, string controllerName, object routeValues, FormMethod method, object htmlAttributes)
        {
            return BeginFormGeneral(htmlHelper, actionName, controllerName, new RouteValueDictionary(routeValues), method, new RouteValueDictionary(htmlAttributes));
        }
        
        /// <summary>Writes an opening &lt;form&gt; tag to the response, and sets the action tag to the specified controller, action, and route values from the route value dictionary. The form uses the specified HTTP method, and includes the HTML attributes from the dictionary. If method is not GET, then anti-forgery token will be generated.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        /// <param name="method">The HTTP method for processing the form, either GET or POST.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        public static MvcForm BeginFormGeneral(this HtmlHelper htmlHelper, string actionName, string controllerName, RouteValueDictionary routeValues, FormMethod method, IDictionary<string, object> htmlAttributes)
        {
            var routes = GetRoutes(actionName, controllerName, routeValues);
            var mvcForm = htmlHelper.BeginForm(Convert.ToString(routes["action"]), Convert.ToString(routes["controller"]), routes, method, htmlAttributes);
            if (method != FormMethod.Get)
            {
                htmlHelper.ViewContext.Writer.Write(htmlHelper.AntiForgeryToken().ToHtmlString());
            }
            return mvcForm;
        }

        private static RouteValueDictionary GetRoutes(string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            RouteValueDictionary routes;
            if (routeValues == null || routeValues.Count == 0)
            {
                routes = new RouteValueDictionary();
            }
            else
            {
                routes = new RouteValueDictionary(routeValues);
            }

            var routeVals = HttpContext.Current.Request.RequestContext.RouteData.Values;
            if (!routes.ContainsKey("institution"))
            {
                routes["institution"] = routeVals["institution"];
            }
            if (!routes.ContainsKey("area"))
            {
                routes["area"] = routeVals["area"];
            }
            if (string.IsNullOrWhiteSpace(actionName))
            {
                actionName = Convert.ToString(routeVals["action"]);
            }
            routes["action"] = actionName;
            if (string.IsNullOrWhiteSpace(controllerName))
            {
                controllerName = Convert.ToString(routeVals["controller"]);
            }
            routes["controller"] = controllerName;
            return routes;
        }
    }
}
