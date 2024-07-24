using Dakity.AwsTools.R53.Ddns;
using Dakity.AwsTools.R53.Ddns.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(config =>
    {
        config.AddEnvironmentVariables();

        if (args != null)
        {
            // Environment from command line
            // e.g.: dotnet run --environment "Staging"
            config.AddCommandLine(args);
        }
    })
    .ConfigureAppConfiguration((context, config) =>
    {
        config
            .AddJsonFile("appsettings.json", false, true)
            .AddUserSecrets<Program>()
            .AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationServices(context.Configuration);
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();