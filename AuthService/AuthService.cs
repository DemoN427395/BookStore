
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace AuthService
{
    public class AuthService
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthentication()
                .AddJwtBearer(options =>
                {
                    options.Authority = "http://localhost:5074";
                    options.TokenValidationParameters.ValidateAudience = false;
                    options.TokenValidationParameters.ValidateIssuer = false;
                    options.RequireHttpsMetadata = false;
                });
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "api-scope");
                });
            });


            // Add authorization services
            builder.Services.AddAuthorization();

            var app = builder.Build();
            app.UseAuthentication(); // Ensure authentication middleware is added before authorization
            app.UseAuthorization();

            app.MapGet("identity", (ClaimsPrincipal user) => user.Claims.Select(c => new { c.Type, c.Value }))
                .RequireAuthorization("ApiScope");

            app.Run();
        }
    }
}
