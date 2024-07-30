using System.Threading;
using System.Threading.Tasks;

namespace Dakity.AwsTools.R53.Ddns.Features.Route53;

/// <summary>
/// A contract to perform DNS updates dynamically.
/// </summary>
public interface IDynamicDnsService
{
	/// <summary>
	/// Allows to update DNS records.
	/// </summary>
	/// <param name="request">An instance of <see cref="UpdateDnsRequest"/>.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns></returns>
	Task UpdateDnsAsync(UpdateDnsRequest request, CancellationToken cancellationToken);
}