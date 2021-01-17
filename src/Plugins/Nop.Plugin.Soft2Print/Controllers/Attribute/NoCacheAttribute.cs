using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Nop.Plugin.Soft2Print.Controllers.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class NoCacheAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Headers.Add("Cache-Control", "no-cache");
            base.OnResultExecuting(filterContext);
        }
    }
}
