using Dakity.AwsTools.R53.Ddns.Features.IpCheck;
using Dakity.AwsTools.R53.Ddns.Features.Route53;
using Dakity.AwsTools.R53.Ddns.Features.Security;
using Dakity.AwsTools.R53.Ddns.IoC.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Dakity.AwsTools.R53.Ddns.IoC;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();

        var settings = configuration.Get<AppSettings>();
        services.TryAddSingleton(settings);

        services.TryAddSingleton<IAwsCredentialsResolver, AwsCredentialsResolver>();
        services.TryAddSingleton<IAmazonRoute53ClientFactory, AmazonRoute53ClientFactory>();
        services.TryAddSingleton<IDynamicDnsService, DynamicDnsService>();
        services.AddHttpClient<IIpAddressResolver, IpAddressResolver>();

        return services;
    }
}