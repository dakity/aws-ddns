using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dakity.AwsTools.R53.Ddns.Features.Route53;
using Dakity.AwsTools.R53.Ddns.IoC.Dto;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dakity.AwsTools.R53.Ddns;

public class Worker(ILogger<Worker> logger, AppSettings config, IDynamicDnsService dnsService) : BackgroundService
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
		ArgumentNullException.ThrowIfNull(config);

		if (!config.HostedZoneDomains.Any() || !config.AwsAccounts.Any())
		{
			var configErrorMessage = "Dynamic DNS Service stopping due to missing configuration. Please review that the correct `AwsAccounts` and `HostedZoneDomains` have been set.";
			logger.LogWarning(configErrorMessage);
			throw new ApplicationException(configErrorMessage);
		}

		foreach (var zoneDomain in config.HostedZoneDomains)
		{
			logger.LogInformation($"Running Dynamic DNS check for `{zoneDomain.Name}`.");

			var request = new UpdateDnsRequest
			{
				Domain = zoneDomain,
				AccountConfig = config.AwsAccounts.Single(x => x.Name == zoneDomain.AwsAccountKey)
			};

			await dnsService.UpdateDnsAsync(request, cancellationToken);
		}

		Console.WriteLine();
		logger.LogInformation("Dynamic DNS check completed at: {time}", DateTimeOffset.Now);
	}
}