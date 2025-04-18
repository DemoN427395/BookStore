using Microsoft.AspNetCore;

namespace GATEWAY;

public class Gateway
{
    public static async Task Main(string[] args)
    {
        Console.Title = "Ocelot API Gateway";
        var builder = WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
            })
            .UseStartup<Startup>()
            .UseUrls("http://*:4000");

        var host = builder.Build(); 
        await host.RunAsync();
    }
}