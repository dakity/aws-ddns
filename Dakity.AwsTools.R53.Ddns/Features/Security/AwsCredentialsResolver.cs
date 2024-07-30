using System;
using System.Collections.Generic;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;

namespace Dakity.AwsTools.R53.Ddns.Features.Security;

/// <summary>
/// An implementation of <see cref="IAwsCredentialsResolver"/>.
/// </summary>
/// <param name="profile">The optional AWS profile name running on the executing computer.</param>
/// <param name="vars">Environment variables containing credential values.</param>
public class AwsCredentialsResolver(string profile = null, Dictionary<string, string> vars = null) : IAwsCredentialsResolver
{
	private readonly Dictionary<int, AWSCredentials> _awsCredentialsCache = new();

	/// <inheritdoc cref="IAwsCredentialsResolver"/>
	public AWSCredentials ResolveCredentials(string awsProfile = null, Dictionary<string, string> envVars = null)
	{
		profile = awsProfile;
		vars = envVars;

		var hash = new HashCode();
		hash.Add(awsProfile);
		hash.Add(envVars);

		var hashcode = hash.ToHashCode();

		if (_awsCredentialsCache.TryGetValue(hashcode, out var awsCredentials)) return awsCredentials;


		var chain = new CredentialProfileStoreChain();

		if (!string.IsNullOrWhiteSpace(awsProfile) && chain.TryGetAWSCredentials(awsProfile, out awsCredentials))
		{
			_awsCredentialsCache.Add(hashcode, awsCredentials);
			return awsCredentials;
		}

		if (envVars != null && !string.IsNullOrEmpty(envVars["AWS_SESSION_TOKEN"]))
		{
			awsCredentials = new SessionAWSCredentials(envVars["AWS_ACCESS_KEY_ID"], envVars["AWS_SECRET_ACCESS_KEY"], envVars["AWS_SESSION_TOKEN"]);
			_awsCredentialsCache.Add(hashcode, awsCredentials);
			return awsCredentials;
		}

		if (envVars != null)
		{
			awsCredentials = new BasicAWSCredentials(envVars["AWS_ACCESS_KEY_ID"], envVars["AWS_SECRET_ACCESS_KEY"]);
			_awsCredentialsCache.Add(hashcode, awsCredentials);
			return awsCredentials;
		}

		return null;
	}

	/// <inheritdoc cref="IAwsCredentialsResolver"/>
	public AWSCredentials RefreshCredentials()
	{
		_awsCredentialsCache.Clear();
		return ResolveCredentials(profile, vars);
	}
}