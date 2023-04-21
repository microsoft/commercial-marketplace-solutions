# Sample Azure Managed Application with AKS

## Testing prior to publishing in marketplace

While developing and iterating, it is convenient to test the app without having to publish it in marketplace every time since push to preview may sometime take a few hours.

> When testing locally, we need to comment out the "delegatedManagedIdentityResourceId" lines in the role assignments since this property can be used only in cross-tenant scenarios such as deployment from Azure Marketplace.

Test the application using <http://github.com/Azure/arm-ttk> tool

```cmd
C:\Code\GitHub\Azure\arm-ttk\arm-ttk\Test-AzTemplate.cmd
```

Zip relevant files and upload to a storage container

```bash
tar -a -c -f ama-aks.zip createUiDefinition.json mainTemplate.json viewDefinition.json

azcopy copy ./ama-aks.zip "https://YOUR_STORAGE_ACCOUNT.blob.core.windows.net/YOUR_STORAGE_CONTAINER/ama-aks.zip?SHARED_ACCESS_SIGNATURE_WITH_WRITE_PERMISSION"
```

Create managed app definition for testing the deployment

```bash
az group create --name avamaaks --location eastus

az managedapp definition create --name "azure-managed-app-aks" --location eastus --resource-group avamaaks --lock-level ReadOnly --display-name "Azure Managed App AKS" --description "Azure Managed App AKS Example" --authorizations "YOUR_AAD_GROUP_PRINCIPAL_ID:b24988ac-6180-42a0-ab88-20f7382dd24c" --package-file-uri "https://YOUR_STORAGE_ACCOUNT.blob.core.windows.net/ama-aks/ama-aks.zip"
```

Deploy managed application from the definition created above using <https://portal.azure.com>

## Delay after AKS creation

> NOTE: The deployment will take about 30 minutes due to a purposeful sleep of 20+ minutes within the ARM template.

The sleep is there to introduce a delay after creation of the AKS cluster and its Node Resource Group so that AMA RP role assignments work properly for the Node Resource Group since it may take up to 30 minutes for the role assignments to reflect properly on the Node Resource Group in some cases.

Without this delay, the deployment will succeed most of the time by may fail intermittently with error messages like this:

> LinkAuthorizationFailed. The client 'xxxxx' with object id 'xxxxx' has permission to perform action 'Microsoft.Authorization/roleAssignments/write' on scope '/subscriptions/xxxxx/resourcegroups/mrg-xxxxx/providers/Microsoft.ContainerService/managedClusters/xxxxx/providers/Microsoft.Authorization/roleAssignments/xxxxx'; however, it does not have permission to perform action 'Microsoft.ManagedIdentity/userAssignedIdentities/write' on the linked scope(s) '/subscriptions/xxxxx/resourcegroups/mrg-xxxxx-aks-rg/providers/Microsoft.ManagedIdentity/userAssignedIdentities/xxxxx' or the linked scope(s) are invalid.

## Testing after publishing in marketplace

After the app seems to work, publish in your [Partner Center account](https://docs.microsoft.com/en-us/azure/azure-resource-manager/managed-applications/publish-marketplace-app) by uploading the ama-aks.zip file to the relevant plan's technical configuration.

After the app is available in preview, search for the app in <https://poral.azure.com> "Create a resource", and deploy using the UI.
