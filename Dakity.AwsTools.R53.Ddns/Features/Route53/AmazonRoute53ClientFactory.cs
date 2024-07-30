using System.Collections.Generic;
using Amazon;
using Amazon.Route53;
using Dakity.AwsTools.R53.Ddns.Features.Security;

namespace Dakity.AwsTools.R53.Ddns.Features.Route53;

/// <summary>
/// An implementation of <see cref="IAmazonRoute53ClientFactory"/>
/// </summary>
/// <param name="credentialsResolver">An instance of <see cref="IAwsCredentialsResolver"/>.</param>
public class AmazonRoute53ClientFactory(IAwsCredentialsResolver credentialsResolver) : IAmazonRoute53ClientFactory
{
	/// <inheritdoc cref="IAmazonRoute53ClientFactory" />
	public AmazonRoute53Client BuildClient(string awsProfileName)
	{
		var credentials = credentialsResolver.ResolveCredentials(awsProfileName);

		return new AmazonRoute53Client(credentials);
	}

	/// <inheritdoc cref="IAmazonRoute53ClientFactory" />
	public AmazonRoute53Client BuildClient(string accessKey, string secret)
	{
		var credentials = credentialsResolver.ResolveCredentials(null, new Dictionary<string, string>
		{
			{ "AWS_ACCESS_KEY_ID", accessKey },
			{ "AWS_SECRET_ACCESS_KEY", secret },
			{ "AWS_SESSION_TOKEN", null }
		});

		return new AmazonRoute53Client(credentials, RegionEndpoint.USEast1);
	}

	/// <inheritdoc cref="IAmazonRoute53ClientFactory" />
	public AmazonRoute53Client BuildClient(string accessKey, string secret, string sessionToken)
	{
		var credentials = credentialsResolver.ResolveCredentials(null, new Dictionary<string, string>
		{
			{ "AWS_ACCESS_KEY_ID", accessKey },
			{ "AWS_SECRET_ACCESS_KEY", secret },
			{ "AWS_SESSION_TOKEN", sessionToken }
		});

		return new AmazonRoute53Client(credentials, RegionEndpoint.USEast1);
	}
}