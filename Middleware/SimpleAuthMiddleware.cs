using Microsoft.AspNetCore.Authorization;

namespace SimpleDrive.Middleware
{
    public class SimpleAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _token;

        public SimpleAuthMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _token = config["Auth:BearerToken"];
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var skipAuth = endpoint?.Metadata?.GetMetadata<AllowAnonymousAttribute>() != null;

            if (!skipAuth)
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();
                if (authHeader != $"Bearer {_token}")
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }
            }

            await _next(context);
        }
    }

}
