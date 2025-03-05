using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utilities;

public class Authentication : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        if (filterContext.HttpContext.Session.GetInt32("UserId") == null)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary {
                    { "Controller", "Home" },
                    { "Action", "Login" }
                });
        }
    }
}