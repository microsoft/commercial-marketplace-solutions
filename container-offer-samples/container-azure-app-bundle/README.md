# Azure Application: HOW-TO Deploy Container Offer with AKS

This repo demonstrates how to deploy Container offer as part of AKS cluster as Azure Managed Application.

## How To Deploy AKS as Azure Application

Please refer to the following repo for details creating Azure Managed Application (AMA) ARM templates to deploy AKS in the AMA managed resource group.

> [Azure Managed Application and AKS with Managed Identity](https://github.com/arsenvlad/azure-managed-app-aks-managed-identity)

## Challenges to deploy Container Offer as part of the ARM template

In order to deploy Container Offer as Part of the Managed App. The following information must be provided as part of the ARM chart.

```json
   "clusterExtensionTypeName": " Enter the Extension Type",
   "plan-name": "Enter the Plan Name",
   "plan-offerID": " Enter Offer ID",
   "plan-publisher": "Enter the publisher ID",
   "releaseTrain": "stable"
```

 Here is example for AzureVote Demo information.

 ```json
   "clusterExtensionTypeName": "Contoso.AzureVoteKubernetesAppTest",
   "plan-name": "testplan",
   "plan-offerID": "kubernetest_apps_demo_offer",
   "plan-publisher": "test_test_mix3pptest0011614206850774",
   "releaseTrain": "stable"
 ```

## Azure Application Offer Resources

You can learn more about Azure Managed Application offers using the following resources.

1. [Mastering The Marketplace](https://microsoft.github.io/Mastering-the-Marketplace/ama/) on-demand video course.
1. [Azure Managed Applications overview](https://learn.microsoft.com/en-us/azure/azure-resource-manager/managed-applications/overview) documentation.

## Azure Container Offer Resources

You can learn more about Azure Container offer using the following resources.

1. [Mastering The Marketplace](https://microsoft.github.io/Mastering-the-Marketplace/container/) on-demand video course.
1. [Azure Container Offer overview](https://learn.microsoft.com/en-us/azure/marketplace/marketplace-containers) documentation.
