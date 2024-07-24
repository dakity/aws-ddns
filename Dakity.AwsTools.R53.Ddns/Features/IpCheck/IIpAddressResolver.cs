using System.Net;
using System.Threading.Tasks;

namespace Dakity.AwsTools.R53.Ddns.Features.IpCheck;

/// <summary>
/// A contract to perform lookup operation operations for IP addresses.
/// </summary>
public interface IIpAddressResolver
{
	/// <summary>
	/// Allows to look up the local network gateway IP address.
	/// </summary>
	/// <returns>The external IP address.</returns>
	Task<string> GetExternalIpAddressAsync();

	/// <summary>
	/// Allows to retrieve the local machine IP address.
	/// </summary>
	/// <returns>A collection of <see cref="IPAddress"/> containing the local machine IP addresses.</returns>
	Task<IPAddress[]> GetMachineIpAddressAsync();
}