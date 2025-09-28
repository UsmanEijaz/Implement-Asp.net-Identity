using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using User_Management.Model;

namespace User_Management.Middlewares
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ApplicationDbContext db)
        {
            var path = context.Request.Path;

            if (path.StartsWithSegments("/login") || path.StartsWithSegments("/register"))
            {
                await _next(context);
                return;
            }

            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                var handler = new JwtSecurityTokenHandler();

                try
                {
                    var jwtToken = handler.ReadJwtToken(token);
                    var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

                    var isRevoked = await db.RefreshTokens.AnyAsync(r => r.Token == jti && r.IsRevoked);
                    if (isRevoked)
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Token revoked. Please log in again.");
                        return;
                    }
                }
                catch (Exception)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Invalid token.");
                    return;
                }
            }
            await _next(context);
        }
    }
}
