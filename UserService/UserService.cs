using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BookStoreLib.Data;
using Microsoft.AspNetCore.Http.Features;
using UserService.Services;

namespace UserService;
public class UserService
{
    public static void Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);

            // Setting up CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            // Adding HTTP clients for external services
            builder.Services.AddHttpClient<AuthServiceClient>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:5000");
            });

            builder.Services.AddHttpClient("AuthTokenService", client =>
            {
                client.BaseAddress = new Uri("https://localhost:5000");
            });

            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 104857600;
            });

            // Database configuration using PostgreSQL
            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<UserDbContext>(options =>
                options.UseNpgsql(connectionString, b =>
                {
                    b.MigrationsAssembly("BookStoreLib");
                    b.MigrationsHistoryTable("__UserMigrationsHistory", "user");
                }));

            // JWT Authentication setup
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JWT:secret"]))
                };
            });

            builder.Services.AddAuthorization();
            builder.Services.AddControllers();
            builder.Services.AddScoped<BookManager>();
            builder.Services.AddScoped<AuthServiceClient>();
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Apply migrations
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();
                context.Database.Migrate();
            }

            // Middleware setup
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
        catch (Exception ex)
        {
            // Log exception to the console
            Console.WriteLine(ex.Message);
            Console.ReadKey();
        }
    }
}