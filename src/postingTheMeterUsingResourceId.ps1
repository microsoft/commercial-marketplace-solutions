# Get system identity access token
# You will use this token for calling the management API
$managementTokenUrl = "http://169.254.169.254/metadata/identity/oauth2/token?api-version=2018-02-01&resource=https%3A%2F%2Fmanagement.azure.com%2F"
$Token = Invoke-RestMethod -Headers @{"Metadata" = "true"} -Uri $managementTokenUrl 
$Headers = @{}
$Headers.Add("Authorization","$($Token.token_type) "+ " " + "$($Token.access_token)")


# Get subscription and resource group
$metadataUrl = "http://169.254.169.254/metadata/instance?api-version=2019-06-01"
$metadata = Invoke-RestMethod -Headers @{'Metadata'='true'} -Uri $metadataUrl

# Make sure the system identity has at least reader permission on the resource group through the deployment template
$managementUrl = "https://management.azure.com/subscriptions/" + $metadata.compute.subscriptionId + "/resourceGroups/" + $metadata.compute.resourceGroupName + "?api-version=2019-10-01"
$resourceGroupInfo = Invoke-RestMethod -Headers $Headers -Uri $managementUrl 
$managedappId = $resourceGroupInfo.managedBy

# Get resourceUsageId from the managed app
# A nested deployment within the template is used to give read access to the managed identity.
$managedAppUrl = "https://management.azure.com" + $managedappId + "\?api-version=2019-07-01"
$managedApp = Invoke-RestMethod -Headers $Headers -Uri $managedAppUrl  

$resourceUsageId = $ManagedApp.properties.billingDetails.resourceUsageId



# Get the token for calling the metering API
$meteringApiTokenUrl = "http://169.254.169.254/metadata/identity/oauth2/token?api-version=2018-02-01&resource=20e940b3-4c77-4b0b-9a53-9e16a1b010a7"
$Token = Invoke-RestMethod -Headers @{"Metadata" = "true"} -Uri $meteringApiTokenUrl 


# Set to use TLS 1.2
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls11, [Net.SecurityProtocolType]::Tls12;


$lastHourMinusFiveMinutes = (Get-Date).AddMinutes(-65).ToString("yyyy-MM-ddTHH:mm:ssZ")

$body = @{ 'resourceId' = $resourceUsageId; 'quantity' = 15; 'dimension' = 'dim1'; 'effectiveStartTime' = $lastHourMinusFiveMinutes; 'planId' = 'userassigned'} | ConvertTo-Json

$Headers = @{}
$Headers.Add("Authorization","$($Token.token_type) "+ " " + "$($Token.access_token)")

# Post the meter
$response = Invoke-RestMethod 'https://marketplaceapi.microsoft.com/api/usageEvent?api-version=2018-08-31' -Method 'POST' -ContentType "application/json" -Headers $Headers -Body $body -Verbose




