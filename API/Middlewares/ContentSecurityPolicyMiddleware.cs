namespace RSOS.Middlewares;

public class ContentSecurityPolicyMiddleware
{
    private readonly RequestDelegate _next;

    public ContentSecurityPolicyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; " +
                                                                   "style-src 'self' 'unsafe-inline'; " +
                                                                   "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
                                                                   "font-src 'self'; " +
                                                                   "img-src 'self' data:; ");

        await _next(context);
    }
}

public static class ContentSecurityPolicyMiddlewareExtensions
{
    public static IApplicationBuilder UseContentSecurityPolicy(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ContentSecurityPolicyMiddleware>();
    }
}