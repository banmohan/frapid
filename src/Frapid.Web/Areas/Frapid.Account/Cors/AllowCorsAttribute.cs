using System.Web.Mvc;

namespace Frapid.Account.Cors
{
    public class AllowCorsAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!string.IsNullOrWhiteSpace(filterContext.RequestContext.HttpContext.Request.Headers["Origin"]) 
                && string.IsNullOrWhiteSpace(filterContext.RequestContext.HttpContext.Response.Headers["Access-Control-Allow-Origin"]))
            {
                filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "*");
                filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Headers", "*");
                filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Credentials", "true");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}