using System.Security.Claims;
using Duende.IdentityServer.Models;
using IdentityService.Database;
using IdentityService.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService
{
    public class IdentityService
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication(IdentityConstants.BearerScheme).AddCookie(IdentityConstants.ApplicationScheme)
                .AddBearerToken(IdentityConstants.BearerScheme);

            builder.Services.AddIdentityCore<User>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddApiEndpoints();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            WebApplication app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

                app.ApplyMigrations();
            }

            app.MapGet("users/me", async (ClaimsPrincipal claims, ApplicationDbContext context) =>
            {
                string userId = claims.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

                return await context.Users.FindAsync(userId);
            }).RequireAuthorization();

            app.UseHttpsRedirection();

            app.MapIdentityApi<User>();

            app.Run();

        }
    }
}
