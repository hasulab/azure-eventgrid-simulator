# azure-eventgrid-simulator
AzureEventGridSimulator Sample proejct
## Getting Started 

* Create a new ASP.NET Code empty Project
* Add latest [Hasulab.Azure.EventGrid.Simulator nuget library](https://www.nuget.org/packages/Hasulab.Azure.EventGrid.Simulator/) version reference to your proejct
* Add following lines to Progarm.cs
```
builder.Services.AddLogging();
builder.Services.AddSimulatorServices(builder.Configuration);
//Other codes
app.MapSimulatorEndpoint();

```
* Add following sample Kestrel settings to appsettings.json or appsettings.Development.json

```
  "Kestrel": {
    "Certificates": {
      "Default": {
        "Path": "localhost-TestPASS1234.pfx",
        "Password": "TestPASS1234"
      }
    }
  },
  "EventDeliverySettings": {
    "CheckUpdateTime": 1000,
    "ConcurrentEventsProcessing": 2 
  }
```
and events subscriptions  settings to appsettings.json or appsettings.Development.json
```
"topics": [
    {
      "name": "MyLocalAzureFunctionTopic",
      "port": 5002,
      "key": "TheLocal+DevelopmentKey=",
      "subscribers": [
        {
          "name": "LocalAzureFunctionSubscription",
          "destination": {
            "endpointType": "WebHook",
            "properties": {
              "endpoint": "http://localhost:7071/runtime/webhooks/EventGrid?functionName=ExampleFunction",
              "disableValidation": true
            }
          },
          "filter": {
            "includedEventTypes": [ "TestEvent" ]
          }
        }
    }
  ]
```
* Press F5 or run the proejct

## create local host certificate 

### Create a self-signed root certificate

```powershell
$params = @{
    Type = 'Custom'
    Subject = 'CN=MyLocalhostRootCert'
    KeySpec = 'Signature'
    KeyExportPolicy = 'Exportable'
    KeyUsage = 'CertSign'
    KeyUsageProperty = 'Sign'
    KeyLength = 2048
    HashAlgorithm = 'sha256'
    NotAfter = (Get-Date).AddMonths(24)
    CertStoreLocation = 'Cert:\CurrentUser\My'
}
$cert = New-SelfSignedCertificate @params
```

or find root certificate by name
```powershell
 $certs = Get-ChildItem -path Cert:\* -Recurse | where {$_.Subject –like '*MyLocalhostRootCert*'}

 $certs.Length
 $cert = $certs[0]
```

or by thumbprint
```powershell
 $certs = Get-ChildItem -Path "Cert:\*<THUMBPRINT>" -Recurse

 $certs.Length
 $cert = $certs[0]
```


#### Export CA certificate
    * goto `run` and type `certmgr.msc`
    * goto `Manage user certificates -> Certificates - Current Users` 
    * goto `Personal -> Certificates`
    * Right click on the root cetificate and follow the Wizard and  export with private key.

#### Imoprt root certificate to tursted root
    * goto `Manage user certificates -> Certificates - Current Users`
    * goto `Trusted Root Certification Authorities -> Certificates`
    * right click on import and follow the Wizard
    * seelct `Trusted Root Certification Authorities` where necessary.

### Generate a client certificate with localhost

```powershell
$params = @{
       Type = 'Custom'
       Subject = 'CN=localhost'
       DnsName = 'localhost'
       KeySpec = 'Signature'
       KeyExportPolicy = 'Exportable'
       KeyLength = 2048
       HashAlgorithm = 'sha256'
       NotAfter = (Get-Date).AddMonths(18)
       CertStoreLocation = 'Cert:\CurrentUser\My'
       Signer = $cert
       TextExtension = @(
        '2.5.29.37={text}1.3.6.1.5.5.7.3.1')
   }
   New-SelfSignedCertificate @params
```


```powershell
$params = @{
       Type = 'Custom'
       Subject = 'CN=myhost'
       DnsName = 'myhost'
       KeySpec = 'Signature'
       KeyExportPolicy = 'Exportable'
       KeyLength = 2048
       HashAlgorithm = 'sha256'
       NotAfter = (Get-Date).AddMonths(18)
       CertStoreLocation = 'Cert:\CurrentUser\My'
       Signer = $cert
       TextExtension = @(
        '2.5.29.37={text}1.3.6.1.5.5.7.3.1')
   }
   New-SelfSignedCertificate @params
```

#### Export client/dns certificate
    * goto `run` and type `certmgr.msc`
    * goto `Manage user certificates -> Certificates - Current Users` 
    * goto `Personal -> Certificates`
    * Right click on the client/dns cetificate and follow the Wizard and  export with private key.
    * Save as `localhost.pfx` in the `src\Local.ReverseProxy` folder and update the password in the ``appSettings.json`` file.


#### More info
    * How to Generate and export certificates for point-to-site using PowerShell](https://learn.microsoft.com/en-us/azure/vpn-gateway/vpn-gateway-certificates-point-to-site)
