using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BookStoreLib.Models;
using BookStoreLib.Data;
using UserService.Constants;
using UserService.Services;
using Microsoft.AspNetCore.Identity;

namespace UserService
{
    public class UserService
    {
        public static void Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // Добавление политики CORS
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowAll", policy =>
                    {
                        policy.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
                });

                builder.Services.AddHttpClient<AuthServiceClient>(client =>
                {
                    client.BaseAddress = new Uri("https://localhost:5000"); // URL AuthTokenService
                });


                builder.Services.AddHttpClient("AuthTokenService", client =>
                {
                    client.BaseAddress = new Uri("https://localhost:5000"); // URL AuthTokenService
                });
                string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

                // // Используем UserDbContext из библиотеки
                // builder.Services.AddDbContext<UserDbContext>(options =>
                //     options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

                // UserDbContext
                builder.Services.AddDbContext<UserDbContext>(options =>
                    options.UseNpgsql(connectionString, b =>
                    {
                        b.MigrationsAssembly("BookStoreLib");
                        b.MigrationsHistoryTable("__UserMigrationsHistory", "user"); // История миграций в схеме user
                    }));


                // Настройка аутентификации JWT
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

                // Применение миграций
                using (var scope = app.Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();
                    context.Database.Migrate();
                }

                app.UseCors("AllowAll");
                app.UseAuthentication();
                app.UseAuthorization();
                app.MapControllers();
                app.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }
    }
}