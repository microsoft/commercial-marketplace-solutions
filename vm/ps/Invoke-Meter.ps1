function Invoke-Meter() {

    Param (
        [Parameter(Mandatory)]
        [string]
        $DimensionId,

        [Parameter(Mandatory)]
        [string]
        $PlanId,

        [Parameter(Mandatory)]
        [string]
        $Quantity
    )

    # Get subscription and resource group
    $MetadataUrl = "http://169.254.169.254/metadata/instance?api-version=2019-06-01"
    $Metadata = Invoke-RestMethod -Headers @{ 'Metadata' = 'true' } -Uri $MetadataUrl
    
    # Get system identity access token
    # You will use this token for calling the management API
    $ManagementTokenUrl = "http://169.254.169.254/metadata/identity/oauth2/token?api-version=2018-02-01&resource=https%3A%2F%2Fmanagement.azure.com%2F"
    $AuthResponse = Invoke-RestMethod -Headers @{ "Metadata" = "true" } -Uri $ManagementTokenUrl 
    
    $Headers = @{}
    $Headers.Add("Authorization", "$($AuthResponse.token_type) " + " " + "$($AuthResponse.access_token)")

    # Make sure the system identity has at least reader permission on the resource group through the deployment template
    $ManagementUrl = "https://management.azure.com/subscriptions/" + $Metadata.compute.subscriptionId + "/resourceGroups/" + $Metadata.compute.resourceGroupName + "?api-version=2019-10-01"
    $ResourceGroupInfo = Invoke-RestMethod -Headers $Headers -Uri $ManagementUrl 
    $ManagedAppId = $ResourceGroupInfo.managedBy

    # Get the token for calling the metering API
    $MeteringApiTokenUrl = "http://169.254.169.254/metadata/identity/oauth2/token?api-version=2018-02-01&resource=20e940b3-4c77-4b0b-9a53-9e16a1b010a7"
    $AuthResponse = Invoke-RestMethod -Headers @{ "Metadata" = "true" } -Uri $MeteringApiTokenUrl 

    # Set to use TLS 1.2
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12;

    # Invoke meter for last 60m of transactions
    $StartTime = (Get-Date).AddMinutes(-60).ToString("yyyy-MM-ddTHH:mm:ssZ")

    $Body = @{ 
        "resourceUri"        = $ManagedAppId
        "quantity"           = $Quantity 
        "dimension"          = $DimensionId
        "effectiveStartTime" = $StartTime
        "planId"             = $PlanId
    } | ConvertTo-Json

    # Post the meter
    $MeterCallResponse = Invoke-RestMethod 'https://marketplaceapi.microsoft.com/api/usageEvent?api-version=2018-08-31' -Method 'POST' -ContentType "application/json" -Headers $Headers -Body $Body -Verbose

    Write-Host "-----------------------------------------------"
    Write-Host ($MeterCallResponse | ConvertTo-Json)
    Write-Host "-----------------------------------------------"
}