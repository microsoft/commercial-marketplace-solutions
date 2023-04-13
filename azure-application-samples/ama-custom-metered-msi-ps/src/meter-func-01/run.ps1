using namespace System.Net

# $ErrorActionPreference = "Stop"

param($Request, $TriggerMetadata)

# Write to the Azure Functions log stream.
Write-Host "PowerShell HTTP trigger function processed a request."

# get body arguments
$DimensionId = $Request.Body.Dimension
$EffectiveStartTime = $Request.Body.EffectiveStartTime
$PlanId = $Request.Body.PlanId
$Quantity = $Request.Body.Quantity
$SubscriptionId = $Request.Body.SubscriptionId

# get environmental variables
$IdentityEndpoint = $Env:IDENTITY_ENDPOINT
$IdentityHeader = $Env:IDENTITY_HEADER
$ResouceGroupName = $Env:WEBSITE_RESOURCE_GROUP

# get the auth token response
Write-Host "Getting auth token"

$resourceURI = "https://management.azure.com/"
$tokenAuthURI = $IdentityEndpoint + "?resource=$resourceURI&api-version=2019-08-01"
$tokenResponse = Invoke-RestMethod -Method Get -Headers @{"X-IDENTITY-HEADER" = "$IdentityHeader" } -Uri $tokenAuthURI


# get the managed application
Write-Host "Getting the managed application id"

$ManagementUrl = "https://management.azure.com/subscriptions/" + $SubscriptionId + "/resourceGroups/" + $ResouceGroupName + "?api-version=2019-10-01"
$ResourceGroupInfo = Invoke-RestMethod -Headers $Headers -Uri $ManagementUrl 
$ManagedAppId = $ResourceGroupInfo.managedBy

# make the call to the metering API
Write-Host "Emitting metering call"

$headers = @{
    "Authorization" = $tokenResponse.token_type + ' ' + $tokenResponse.token
}

$body = @{
    "resourceUri" = $ManagedAppId
    "quantity" = $Quantity
    "dimension" = $DimensionId
    "effectiveStartTime" = $EffectiveStartTime
    "planId" = $PlanId
}


$meterCallResponse = Invoke-RestMethod 'https://marketplaceapi.microsoft.com/api/usageEvent?api-version=2018-08-31' -Method 'POST' -ContentType "application/json" -Headers $headers -Body $body -Verbose

Push-OutputBinding -Name Response -Value ([HttpResponseContext]@{
    StatusCode = [HttpStatusCode]::OK
    Body = ($meterCallResponse | ConvertTo-Json)
})