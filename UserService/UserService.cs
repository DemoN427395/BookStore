using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserService.Constants;
using UserService.Models;
using UserService.Services;

namespace UserService
{
    public class UserService
    {
        public static void Main(string[] args)
        {
            try {
                var builder = WebApplication.CreateBuilder(args);

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

                builder.Services.AddHttpClient<AuthServiceClient>(client =>
                {
                    client.BaseAddress = new Uri(AuthUri.HttpUri);
                });

                builder.Services.AddDbContext<UserDataContext>(options =>
                    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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

                var app = builder.Build();

                // Применение миграций
                using (var scope = app.Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<UserDataContext>();
                    context.Database.Migrate();
                }

                app.UseCors("AllowAll");

                app.UseAuthentication();
                app.UseAuthorization();

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
