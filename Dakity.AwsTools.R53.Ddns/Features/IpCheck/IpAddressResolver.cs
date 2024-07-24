using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Dakity.AwsTools.R53.Ddns.Features.IpCheck;

/// <summary>
/// An implementation of <see cref="IIpAddressResolver"/>.
/// </summary>
/// <param name="logger">An instance of <see cref="ILogger{IpAddressResolver}"/>.</param>
/// <param name="client">An instance of the <see cref="HttpClient"/>.</param>
public class IpAddressResolver(ILogger<IpAddressResolver> logger, HttpClient client) : IIpAddressResolver
{
	private const string IpAddressProviderEndpoint = "http://checkip.amazonaws.com";

	/// <inheritdoc cref="IIpAddressResolver"/>
	public async Task<IPAddress[]> GetMachineIpAddressAsync()
	{
		return await Dns.GetHostAddressesAsync(Dns.GetHostName());
	}

	/// <inheritdoc cref="IIpAddressResolver"/>
	public async Task<string> GetExternalIpAddressAsync()
	{
		try
		{
			client.DefaultRequestHeaders.Clear();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			var path = new Uri(IpAddressProviderEndpoint);

			var response = await client.GetAsync(path);
			response.EnsureSuccessStatusCode();
			var responseContent = await response.Content.ReadAsStringAsync();

			if (Regex.IsMatch(responseContent, "^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$"))
			{
				var externalIpAddress = responseContent.TrimEnd(Environment.NewLine.ToCharArray());

				Console.WriteLine();
				logger.LogInformation($"New IP: {externalIpAddress}");
				Console.WriteLine();

				return externalIpAddress;
			}

			throw new InvalidIpAddressException();
		}
		catch (Exception ex) when (ex is HttpRequestException)
		{
			// Do some logging
			logger.LogError(ex, ex.Message);
			return "0.0.0.0";
		}
	}
}