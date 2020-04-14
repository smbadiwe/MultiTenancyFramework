using System;

namespace MultiTenancyFramework.Mvc
{
    /// <summary>
    /// Throw this when view (.cshtml) file is not found
    /// </summary>
    /// <seealso cref="System.InvalidOperationException" />
    public class ViewNotFoundException : InvalidOperationException
    {
        public ViewNotFoundException(string virtualPath) : base(virtualPath + " could not be found")
        {

        }
    }
}
