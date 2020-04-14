using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Routing;

namespace System.Web.Mvc
{
    public static class AreaRegistrationContextExtensions
    {
        /// <summary>
        /// If you need your urls to render as lowercase, use this; otherwise, use <code>.MapRoute</code> extension.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapRouteLowerCase(this AreaRegistrationContext context, string name, string url)
        {
            return MapRouteLowerCase(context, name, url, (object)null /* defaults */);
        }

        /// <summary>
        /// If you need your urls to render as lowercase, use this; otherwise, use <code>.MapRoute</code> extension.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="defaults"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapRouteLowerCase(this AreaRegistrationContext context, string name, string url, object defaults)
        {
            return MapRouteLowerCase(context, name, url, defaults, (object)null /* constraints */);
        }

        /// <summary>
        /// If you need your urls to render as lowercase, use this; otherwise, use <code>.MapRoute</code> extension.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="defaults"></param>
        /// <param name="constraints"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapRouteLowerCase(this AreaRegistrationContext context, string name, string url, object defaults, object constraints)
        {
            return MapRouteLowerCase(context, name, url, defaults, constraints, null /* namespaces */);
        }

        /// <summary>
        /// If you need your urls to render as lowercase, use this; otherwise, use <code>.MapRoute</code> extension.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="namespaces"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapRouteLowerCase(this AreaRegistrationContext context, string name, string url, string[] namespaces)
        {
            return MapRouteLowerCase(context, name, url, (object)null /* defaults */, namespaces);
        }

        /// <summary>
        /// If you need your urls to render as lowercase, use this; otherwise, use <code>.MapRoute</code> extension.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="defaults"></param>
        /// <param name="namespaces"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapRouteLowerCase(this AreaRegistrationContext context, string name, string url, object defaults, string[] namespaces)
        {
            return MapRouteLowerCase(context, name, url, defaults, null /* constraints */, namespaces);
        }

        /// <summary>
        /// If you need your urls to render as lowercase, use this; otherwise, use <code>.MapRoute</code> extension.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="defaults"></param>
        /// <param name="constraints"></param>
        /// <param name="namespaces"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "This is not a regular URL as it may contain special routing characters.")]
        public static Route MapRouteLowerCase(this AreaRegistrationContext context, string name, string url, object defaults, object constraints, string[] namespaces)
        {
            if (namespaces == null && context.Namespaces != null)
            {
                namespaces = context.Namespaces.ToArray();
            }

            Route route = context.Routes.MapRouteLowerCase(name, url, defaults, constraints, namespaces);
            route.DataTokens["area"] = context.AreaName;

            // disabling the namespace lookup fallback mechanism keeps this areas from accidentally picking up
            // controllers belonging to other areas
            bool useNamespaceFallback = (namespaces == null || namespaces.Length == 0);
            route.DataTokens["UseNamespaceFallback"] = useNamespaceFallback;

            return route;
        }
    }
}
