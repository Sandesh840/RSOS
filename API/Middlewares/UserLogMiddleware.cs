using System.Globalization;
using Common.Utilities;
using Serilog.Context;
using UAParser;

namespace RSOS.Middlewares;

public class UserLogMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UserLogMiddleware> _logger;

    public UserLogMiddleware(RequestDelegate next, ILogger<UserLogMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public Task Invoke(HttpContext context)
    {
        var userAgent = context.Request.Headers.UserAgent;
        var uaParser = Parser.GetDefault();
        var clientInfo = uaParser.Parse(userAgent);
        
        if (context.Session.GetInt32("UserId") != null)
        {
            LogContext.PushProperty("SessionIdentifier", context.Session.Id);
            LogContext.PushProperty("UserIdentifier", context.Session.GetInt32("UserId"));
            
            _logger.LogInformation("Session Identifier: {SessionId}", context.Session.Id);
            _logger.LogInformation("User Identifier: {UserId}", context.Session.GetInt32("UserId"));
        }
        
        LogContext.PushProperty("IPAddress", ExtensionMethods.GetIpAddress());
        LogContext.PushProperty("MacAddress", ExtensionMethods.GetMacAddress());
        LogContext.PushProperty("ProcessIdentifier", Environment.ProcessId);
        LogContext.PushProperty("Country", RegionInfo.CurrentRegion.DisplayName);
        LogContext.PushProperty("UserAgent", clientInfo.UA.Family + " " + clientInfo.UA.Major +"." + clientInfo.UA.Minor + " Family " + clientInfo.OS.Family);

        _logger.LogInformation("IP Address: {IpAddress}", ExtensionMethods.GetIpAddress());
        _logger.LogInformation("MAC Address: {MacAddress}", ExtensionMethods.GetMacAddress());
        _logger.LogInformation("Process Identifier: {ProcessId}", Environment.ProcessId);
        _logger.LogInformation("User Agent: {UserAgent}", clientInfo.UA.Family + " " + clientInfo.UA.Major +"." + clientInfo.UA.Minor + " Family " + clientInfo.OS.Family);

        return _next(context);
    }
}