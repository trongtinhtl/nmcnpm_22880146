using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using ITConferences.Enums;
using ITConferences.Providers;

namespace ITConferences.Models
{
    public class SessionAuthorizeAttribute: ActionFilterAttribute
    {
        private readonly Role _role;

        public SessionAuthorizeAttribute(Role role)
        {
            _role = role;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;

            if (httpContext.Session.GetString("UserName") == null)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
                return;
            }

            if (_role == Role.Admin && new UserProvider().IsAdmin(httpContext.Session.GetString("UserName")) == false)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
