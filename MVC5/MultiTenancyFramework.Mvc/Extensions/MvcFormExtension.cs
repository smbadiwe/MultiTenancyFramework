using System.Collections.Generic;
using System.Web.Routing;

namespace System.Web.Mvc.Html
{
    public static class MvcFormExtension
    {
        /// <summary>Writes an opening &lt;form&gt; tag to the response. The form uses the POST method, and the request is processed by the action method for the view.</summary>
        /// <returns>An opening &lt;form&gt; tag. </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        public static MvcForm BeginFormWithXsrf(this HtmlHelper htmlHelper)
        {
            var mvcForm = htmlHelper.BeginForm();
            htmlHelper.ViewContext.Writer.Write(htmlHelper.AntiForgeryToken().ToHtmlString());
            return mvcForm;
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response. The form uses the GET method, and the request is processed by the action method for the view.</summary>
        /// <returns>An opening &lt;form&gt; tag. </returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        public static MvcForm BeginGETForm(this HtmlHelper htmlHelper)
        {
            RouteValueDictionary urlData = htmlHelper.ViewContext.HttpContext.Request.RequestContext.RouteData.Values;
            return htmlHelper.BeginForm(urlData["action"].ToString(), urlData["controller"].ToString(), urlData, FormMethod.Get);
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response and sets the action tag to the specified controller and action. The form uses the POST method.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        public static MvcForm BeginGETForm(this HtmlHelper htmlHelper, string actionName, string controllerName)
        {
            return htmlHelper.BeginForm(actionName, controllerName, new RouteValueDictionary(), FormMethod.Get, new RouteValueDictionary());
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response and includes the route values in the action attribute. The form uses the POST method, and the request is processed by the action method for the view.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.</param>
        public static MvcForm BeginFormWithXsrf(this HtmlHelper htmlHelper, object routeValues)
        {
            var mvcForm = htmlHelper.BeginForm(routeValues);
            htmlHelper.ViewContext.Writer.Write(htmlHelper.AntiForgeryToken().ToHtmlString());
            return mvcForm;
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response and includes the route values from the route value dictionary in the action attribute. The form uses the POST method, and the request is processed by the action method for the view.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        public static MvcForm BeginFormWithXsrf(this HtmlHelper htmlHelper, RouteValueDictionary routeValues)
        {
            return BeginFormWithXsrf(htmlHelper, null, null, routeValues, FormMethod.Post, new RouteValueDictionary());
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response and sets the action tag to the specified controller and action. The form uses the POST method.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        public static MvcForm BeginFormWithXsrf(this HtmlHelper htmlHelper, string actionName, string controllerName)
        {
            return BeginFormWithXsrf(htmlHelper, actionName, controllerName, new RouteValueDictionary(), FormMethod.Post, new RouteValueDictionary());
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response, and sets the action tag to the specified controller, action, and route values. The form uses the POST method.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.</param>
        public static MvcForm BeginFormWithXsrf(this HtmlHelper htmlHelper, string actionName, string controllerName, object routeValues)
        {
            var mvcForm = htmlHelper.BeginForm(actionName, controllerName, routeValues);
            htmlHelper.ViewContext.Writer.Write(htmlHelper.AntiForgeryToken().ToHtmlString());
            return mvcForm;
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response, and sets the action tag to the specified controller, action, and route values from the route value dictionary. The form uses the POST method.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        public static MvcForm BeginFormWithXsrf(this HtmlHelper htmlHelper, string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            return BeginFormWithXsrf(htmlHelper, actionName, controllerName, routeValues, FormMethod.Post, new RouteValueDictionary());
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response and sets the action tag to the specified controller and action. The form uses the specified HTTP method.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="method">The HTTP method for processing the form, either GET or POST.</param>
        public static MvcForm BeginFormWithXsrf(this HtmlHelper htmlHelper, string actionName, string controllerName, FormMethod method)
        {
            return BeginFormWithXsrf(htmlHelper, actionName, controllerName, new RouteValueDictionary(), method, new RouteValueDictionary());
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response and sets the action tag to the specified controller, action, and route values. The form uses the specified HTTP method.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.</param>
        /// <param name="method">The HTTP method for processing the form, either GET or POST.</param>
        public static MvcForm BeginFormWithXsrf(this HtmlHelper htmlHelper, string actionName, string controllerName, object routeValues, FormMethod method)
        {
            var mvcForm = htmlHelper.BeginForm(actionName, controllerName, routeValues, method);
            htmlHelper.ViewContext.Writer.Write(htmlHelper.AntiForgeryToken().ToHtmlString());
            return mvcForm;
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response, and sets the action tag to the specified controller, action, and route values from the route value dictionary. The form uses the specified HTTP method.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        /// <param name="method">The HTTP method for processing the form, either GET or POST.</param>
        public static MvcForm BeginFormWithXsrf(this HtmlHelper htmlHelper, string actionName, string controllerName, RouteValueDictionary routeValues, FormMethod method)
        {
            return BeginFormWithXsrf(htmlHelper, actionName, controllerName, routeValues, method, new RouteValueDictionary());
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response and sets the action tag to the specified controller and action. The form uses the specified HTTP method and includes the HTML attributes.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="method">The HTTP method for processing the form, either GET or POST.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        public static MvcForm BeginFormWithXsrf(this HtmlHelper htmlHelper, string actionName, string controllerName, FormMethod method, object htmlAttributes)
        {
            return BeginFormWithXsrf(htmlHelper, actionName, controllerName, new RouteValueDictionary(), method, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response and sets the action tag to the specified controller and action. The form uses the specified HTTP method and includes the HTML attributes from a dictionary.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="method">The HTTP method for processing the form, either GET or POST.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        public static MvcForm BeginFormWithXsrf(this HtmlHelper htmlHelper, string actionName, string controllerName, FormMethod method, IDictionary<string, object> htmlAttributes)
        {
            return BeginFormWithXsrf(htmlHelper, actionName, controllerName, new RouteValueDictionary(), method, htmlAttributes);
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response and sets the action tag to the specified controller, action, and route values. The form uses the specified HTTP method and includes the HTML attributes.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.</param>
        /// <param name="method">The HTTP method for processing the form, either GET or POST.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        public static MvcForm BeginFormWithXsrf(this HtmlHelper htmlHelper, string actionName, string controllerName, object routeValues, FormMethod method, object htmlAttributes)
        {
            var mvcForm = htmlHelper.BeginForm(actionName, controllerName, routeValues, method, htmlAttributes);
            htmlHelper.ViewContext.Writer.Write(htmlHelper.AntiForgeryToken().ToHtmlString());
            return mvcForm;
        }

        /// <summary>Writes an opening &lt;form&gt; tag to the response, and sets the action tag to the specified controller, action, and route values from the route value dictionary. The form uses the specified HTTP method, and includes the HTML attributes from the dictionary.</summary>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        /// <param name="method">The HTTP method for processing the form, either GET or POST.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        public static MvcForm BeginFormWithXsrf(this HtmlHelper htmlHelper, string actionName, string controllerName, RouteValueDictionary routeValues, FormMethod method, IDictionary<string, object> htmlAttributes)
        {
            var mvcForm = htmlHelper.BeginForm(actionName, controllerName, routeValues, method, htmlAttributes);
            htmlHelper.ViewContext.Writer.Write(htmlHelper.AntiForgeryToken().ToHtmlString());
            return mvcForm;
        }
    }
}
