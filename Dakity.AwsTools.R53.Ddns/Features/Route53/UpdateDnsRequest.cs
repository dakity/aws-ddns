using Dakity.AwsTools.R53.Ddns.IoC.Dto;

namespace Dakity.AwsTools.R53.Ddns.Features.Route53;

public class UpdateDnsRequest
{
	public AwsAccountConfig AccountConfig { get; set; }
	public HostedZoneDomain Domain { get; set; }
	public string ExternalIpAddress { get; set; }
}