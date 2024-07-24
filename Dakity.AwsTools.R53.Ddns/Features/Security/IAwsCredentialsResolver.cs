using System.Collections.Generic;
using Amazon.Runtime;

namespace Dakity.AwsTools.R53.Ddns.Features.Security;

/// <summary>
/// A contract to resolve and obtain.
/// </summary>
public interface IAwsCredentialsResolver
{
	/// <summary>
	/// Allows to resolve for AWS credentials in multiple ways.
	/// </summary>
	/// <param name="awsProfile">The AWS Profile name configured on the executing machine.</param>
	/// <param name="envVars">A collection of environment variables containing keys to create credentials.</param>
	/// <returns>An instance of <see cref="AWSCredentials"/></returns>
	public AWSCredentials ResolveCredentials(string awsProfile = null, Dictionary<string, string> envVars = null);

	/// <summary>
	/// Allows to refresh the expired credentials.
	/// </summary>
	/// <returns>An instance of <see cref="AWSCredentials"/></returns>
	public AWSCredentials RefreshCredentials();
}