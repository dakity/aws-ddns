using Amazon.Route53;

namespace Dakity.AwsTools.R53.Ddns.Features.Route53;

public interface IAmazonRoute53ClientFactory
{
	/// <summary>
	///     Creates a <see cref="AmazonRoute53Client" /> using the AWS Profile credentials.
	/// </summary>
	/// <param name="awsProfileName">The profile name.</param>
	/// <returns>An instance of <see cref="AmazonRoute53Client" /></returns>
	AmazonRoute53Client BuildClient(string awsProfileName);

	/// <summary>
	///     Creates a <see cref="AmazonRoute53Client" /> using the AWS access key and secret credentials.
	/// </summary>
	/// <param name="accessKey">The access key value.</param>
	/// <param name="secret">The secret value.</param>
	/// <returns>An instance of <see cref="AmazonRoute53Client" /></returns>
	AmazonRoute53Client BuildClient(string accessKey, string secret);

	/// <summary>
	///     Creates a <see cref="AmazonRoute53Client" /> using the AWS access key, secret and session token.
	/// </summary>
	/// <param name="accessKey">The access key value.</param>
	/// <param name="secret">The secret value.</param>
	/// <param name="sessionToken">The session token.</param>
	/// <returns>An instance of <see cref="AmazonRoute53Client" /></returns>
	AmazonRoute53Client BuildClient(string accessKey, string secret, string sessionToken);
}