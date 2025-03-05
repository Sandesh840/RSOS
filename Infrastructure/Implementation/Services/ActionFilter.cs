using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace RSOS.Helper
{
    public class ActionNameLoggingFilter : IActionFilter
    {
        private readonly ILogger<ActionNameLoggingFilter> _logger;

        public ActionNameLoggingFilter(ILogger<ActionNameLoggingFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var actionName = context.ActionDescriptor.RouteValues["action"];
            _logger.LogError($"{actionName} Executing {DateTime.UtcNow}");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var actionName = context.ActionDescriptor.RouteValues["action"];
            _logger.LogError($"{actionName} Executed {DateTime.UtcNow}");
        }
    }

}
