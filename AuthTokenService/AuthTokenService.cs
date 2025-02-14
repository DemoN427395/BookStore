using System.Text;
using AuthTokenService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using BookStoreLib.Data;
using BookStoreLib.Interfaces;
using BookStoreLib.Models;

namespace AuthTokenService
{
    public class AuthTokenService
    {
        public static void Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);
                builder.Services.AddControllers();

                // Добавление политики CORS, разрешающей все источники
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowAll", policy =>
                    {
                        policy.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
                });

                // string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

                builder.Services.AddHostedService<MigrationHostedService>();

                // builder.Services.AddDbContext<AuthDbContext>(options =>
                //     options.UseNpgsql(connectionString));
                builder.Services.AddDbContext<AuthDbContext>(options =>
                    options.UseNpgsql(
                        builder.Configuration.GetConnectionString("DefaultConnection"),
                        b => b.MigrationsAssembly("AuthTokenService")
                    )
                );

                builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<AuthDbContext>()
                    .AddDefaultTokenProviders();

                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                    .AddJwtBearer(options =>
                    {
                        var secret = builder.Configuration["JWT:secret"]
                                     ?? throw new ArgumentNullException("JWT:secret is not configured");

                        var validAudience = builder.Configuration["JWT:ValidAudience"]
                                            ?? throw new ArgumentNullException("JWT:ValidAudience is not configured");

                        var validIssuer = builder.Configuration["JWT:ValidIssuer"]
                                          ?? throw new ArgumentNullException("JWT:ValidIssuer is not configured");
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidAudience = validAudience,
                            ValidIssuer = validIssuer,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
                        };
                    });

                builder.Services.AddAuthorization();
                builder.Services.AddScoped<ITokenService, TokenService>();

                var app = builder.Build();

                // Применение миграций
                using (var scope = app.Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
                    context.Database.Migrate();
                }

                // Использование CORS middleware с политикой "AllowAll" (до аутентификации и авторизации)
                app.UseCors("AllowAll");

                app.UseAuthentication();
                app.UseAuthorization();

                // Регистрация контроллеров
                app.MapControllers();
                app.Run();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }
    }
}
