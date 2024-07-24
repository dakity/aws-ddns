# Introduction

**Dakity AWS Dynamic DNS** is a console application written in .NET C# that allows to maintain AWS Route53 domain records mapped to a dynamically changing network IP address.

## How does it work?

It leverage the Amazon <http://checkip.amazonaws.com> endpoint (similar service could be used) to determine if local network IP changed and when so it updates the Route53 A record pointing to the local network.

## Potential uses

This app is ideal when running home labs and/or doing self-hosting or when exposing applications like media servers to the internet usings custom domains.

## Configuration

```json
{
    "ExecutionIntervalInMinutes": "30", 
    "AwsAccounts": [
        {
            "Name": "DOMAIN-NAME",
            "Credentials": {
            "Key": "IAM-ROLE-KEY",
            "Secret": "IAM-ROLE-SECRET"
            }
        }
    ],
    "HostedZoneDomains": [
    {
      "Name": "domain-name.com.",
      "RecordNames": [
        "domain-name.com.",
        "api.domain-name.com.",
      ],
      "AwsAccountKey": "DOMAIN-NAME"
    }
  ],
}
```

**ExecutionIntervalInMinutes**

How often should it check for IP change.

**AwsAccounts**

This is an array that support running the tool against multiple AWS accounts. The recommendation is to create a dedicated IM role applying the least privilege principle with permissions to only do the records update.

- Name - is use as key to read the data. The recommendation is to use the domain name.
- Credentials - an object containing the IAM role key and secret.
  - key - the IAM key
  - Secret - the IAM secret value

> IMPORTANT: ALWAYS STORE YOUR SECRETS IN A SAFE PLACE.

For development the recommendation is to use the VS User Secrets to avoid committing your secrets.
For production the recommendation is to read secrets from SSM or similar technology. Never store secrets in your CICD configuration.

**HostedZoneDomains**

This if the domain zone and the A records pointing to the local network IP.

- Name - The Route53 hosted zone name.
- RecordNames - A collection of A records under the hosted zone.
- AwsAccountKey - The key linking to the `AwsAccounts.Name`, usually the domain name.

## Deployment

This app comes with a docker file that allows you to deploy it as container. It can also be run as stand alone dotnet app in any supporting machine.

> The application needs to be deployed inside the local network so its external/gateway IP is the one being evaluated.
