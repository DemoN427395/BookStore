// using UserService.Models;
// using Microsoft.EntityFrameworkCore;
//
// namespace UserService.Services;
//
// public class MigrationHostedService : IHostedService
// {
//     private readonly IServiceProvider _serviceProvider;
//     public MigrationHostedService(IServiceProvider serviceProvider)
//     {
//         _serviceProvider = serviceProvider;
//     }
//     public async Task StartAsync(CancellationToken cancellationToken)
//     {
//         using var scope = _serviceProvider.CreateScope();
//         var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//         await context.Database.MigrateAsync(cancellationToken);
//     }
//     public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
// }