namespace TentRentalSaaS.Api.Middleware
{
    /// <summary>
    /// Middleware to add security headers to all HTTP responses
    /// </summary>
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Remove server information headers to prevent information disclosure
            context.Response.Headers.Remove("Server");
            context.Response.Headers.Remove("X-Powered-By");
            context.Response.Headers.Remove("X-AspNet-Version");
            context.Response.Headers.Remove("X-AspNetMvc-Version");
            
            // Prevent MIME type sniffing
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            
            // Prevent clickjacking attacks
            context.Response.Headers.Add("X-Frame-Options", "DENY");
            
            // Enable XSS filtering in older browsers
            context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
            
            // Control referrer information
            context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
            
            // Prevent browser caching of sensitive data
            context.Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate");
            context.Response.Headers.Add("Pragma", "no-cache");
            
            // HSTS (HTTP Strict Transport Security) - only when using HTTPS
            if (context.Request.IsHttps)
            {
                context.Response.Headers.Add("Strict-Transport-Security", 
                    "max-age=31536000; includeSubDomains; preload");
            }
            
            // Content Security Policy - allow Stripe for payment processing
            var csp = "default-src 'self'; " +
                     "script-src 'self' https://js.stripe.com; " +
                     "style-src 'self' 'unsafe-inline'; " +
                     "img-src 'self' data: https:; " +
                     "font-src 'self'; " +
                     "connect-src 'self' https://api.stripe.com; " +
                     "frame-src https://js.stripe.com https://hooks.stripe.com; " +
                     "form-action 'self'; " +
                     "base-uri 'self'; " +
                     "object-src 'none';";
            
            context.Response.Headers.Add("Content-Security-Policy", csp);
            
            // Permissions Policy (formerly Feature Policy)
            context.Response.Headers.Add("Permissions-Policy", 
                "geolocation=(), microphone=(), camera=(), payment=(self)");
            
            await _next(context);
        }
    }
    
    /// <summary>
    /// Extension methods for SecurityHeadersMiddleware
    /// </summary>
    public static class SecurityHeadersMiddlewareExtensions
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SecurityHeadersMiddleware>();
        }
    }
}
