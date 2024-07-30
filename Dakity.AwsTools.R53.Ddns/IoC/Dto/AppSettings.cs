using System.Collections.Generic;

namespace Dakity.AwsTools.R53.Ddns.IoC.Dto;

public class AppSettings
{
    /// <summary>
    ///     The frequency on which to check for an IP change.
    /// </summary>
    public int ExecutionIntervalInMinutes { get; set; }

    /// <summary>
    ///     A collection of AWS accounts used to support the hosted zone domains.
    /// </summary>
    public List<AwsAccountConfig> AwsAccounts { get; set; } = new();

    /// <summary>
    ///     A list of <see cref="HostedZoneDomain" /> containing the hosted zones domain names and the collection of records in
    ///     that zone.
    /// </summary>
    public List<HostedZoneDomain> HostedZoneDomains { get; set; } = new();
}