using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dakity.AwsTools.R53.Ddns.Features.IpCheck;
using Dakity.AwsTools.R53.Ddns.Features.Route53;
using Dakity.AwsTools.R53.Ddns.IoC.Dto;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dakity.AwsTools.R53.Ddns;

public class Worker(ILogger<Worker> logger, AppSettings config, IDynamicDnsService dnsService, IIpAddressResolver ipAddressResolver) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		logger.LogInformation("Dynamic DNS Service running.");

		// When the timer should have no due-time, then do the work once now.
		await DoWork(stoppingToken);

		using PeriodicTimer timer = new(TimeSpan.FromMinutes(config.ExecutionIntervalInMinutes));

		try
		{
			while (await timer.WaitForNextTickAsync(stoppingToken))
			{
				await DoWork(stoppingToken);
			}
		}
		catch (OperationCanceledException)
		{
			logger.LogInformation("Dynamic DNS Service stopping.");
		}
	}

	private async Task DoWork(CancellationToken cancellationToken)
	{
		logger.LogInformation("= Dynamic DNS check starting");

		ArgumentNullException.ThrowIfNull(config);

		string externalIpAddress = await ipAddressResolver.GetExternalIpAddressAsync();

		if (!config.HostedZoneDomains.Any() || !config.AwsAccounts.Any())
		{
			var configErrorMessage = "Dynamic DNS Service stopping due to missing configuration. Please review that the correct `AwsAccounts` and `HostedZoneDomains` have been set.";
			logger.LogWarning(configErrorMessage);
			throw new ApplicationException(configErrorMessage);
		}

		foreach (var zoneDomain in config.HostedZoneDomains)
		{
			logger.LogInformation("\n= Running Dynamic DNS check for `{0}` \n", zoneDomain.Name);

			var accountConfig = config.AwsAccounts.SingleOrDefault(x => x.Name == zoneDomain.AwsAccountKey);

			if (accountConfig == null)
			{
				logger.LogError("Unable to locate account given key {0}", zoneDomain.AwsAccountKey);
				continue;
			}

			var request = new UpdateDnsRequest
			{
				Domain = zoneDomain,
				AccountConfig = accountConfig,
				ExternalIpAddress = externalIpAddress
			};

			await dnsService.UpdateDnsAsync(request, cancellationToken);
		}

		Console.WriteLine();
		logger.LogInformation("= Dynamic DNS check completed at: {time} \n================================================================== DONE\n\n", DateTimeOffset.Now);
	}
}