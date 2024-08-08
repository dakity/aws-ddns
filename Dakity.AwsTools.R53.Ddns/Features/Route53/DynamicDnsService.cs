using System;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Route53;
using Amazon.Route53.Model;
using Microsoft.Extensions.Logging;

namespace Dakity.AwsTools.R53.Ddns.Features.Route53;

public class DynamicDnsService(ILogger<DynamicDnsService> logger, IAmazonRoute53ClientFactory route53ClientFactory) : IDynamicDnsService
{
	public async Task UpdateDnsAsync(UpdateDnsRequest request, CancellationToken cancellationToken)
	{
		try
		{
			IAmazonRoute53 client = route53ClientFactory.BuildClient(request.AccountConfig.Credentials.Key, request.AccountConfig.Credentials.Secret);

			var hostedZonesResponse = await client.ListHostedZonesAsync(cancellationToken);

			if (!hostedZonesResponse.HostedZones.Any()) return;

			var domainHostedZones = hostedZonesResponse.HostedZones.Where(x => request.Domain.Name.Contains(x.Name)).ToImmutableList();

			if (!domainHostedZones.Any())
			{
				logger.LogWarning("\n= No hosted zones were found for domain {0} \n", request.Domain.Name);
				return;
			}

			foreach (var hostedZone in domainHostedZones)
			{
				var targetHostedZone = request.Domain;

				if (targetHostedZone == null) continue;

				logger.LogInformation("Processing Zone {0}", targetHostedZone.Name);

				var listResourceRecordSetsResponse = await client.ListResourceRecordSetsAsync(new ListResourceRecordSetsRequest(hostedZone.Id), cancellationToken);

				if (listResourceRecordSetsResponse.HttpStatusCode != HttpStatusCode.OK)
					throw new ApplicationException($"Unable to retrieve the list of record sets for {targetHostedZone.Name}");


				var changeBatch = new ChangeBatch();
				foreach (var domainName in targetHostedZone.RecordNames)
				{
					logger.LogInformation("Updating {0}", domainName);

					ResourceRecordSet domainRecordSet;
					try
					{
						domainRecordSet = listResourceRecordSetsResponse.ResourceRecordSets.Single(x => x.Name == domainName && x.Type == "A");
					}
					catch
					{
						logger.LogWarning("\n{0} not configured in Route53. \n", domainName);
						continue;
					}

					// If DNS record IP matches current external IP, exit loop.
					if (domainRecordSet.ResourceRecords.Any(x => x.Value.Contains(request.ExternalIpAddress))) continue;

					// IP is different, add update record IP and add it to the changes batch collection.
					domainRecordSet.ResourceRecords = [new ResourceRecord { Value = request.ExternalIpAddress }];

					changeBatch.Changes.Add(new Change
					{
						Action = ChangeAction.UPSERT,
						ResourceRecordSet = domainRecordSet
					});
				}

				if (!changeBatch.Changes.Any())
				{
					// Nothing to update.
					logger.LogInformation("All DNS records are up to date.");

					continue;
				}

				// Process update.
				var recordSetsResponse = await client.ChangeResourceRecordSetsAsync(new ChangeResourceRecordSetsRequest(hostedZone.Id, changeBatch), cancellationToken);

				// Monitor the change status
				var changeRequest = new GetChangeRequest
				{
					Id = recordSetsResponse.ChangeInfo.Id
				};

				ChangeStatus status;
				do
				{
					var change = await client.GetChangeAsync(changeRequest, cancellationToken);
					status = change.ChangeInfo.Status;

					logger.LogInformation("Change is pending ...");
					await Task.Delay(15000, cancellationToken);
				} while (status == ChangeStatus.PENDING);


				logger.LogInformation("Hosted Zone {0} updated successfully. \n================================================================== DONE\n", targetHostedZone.Name);
			}
		}
		catch (Exception ex)
		{
			logger.LogError(ex, ex.Message);
			throw;
		}
	}
}