# Azure Managed Application: HOW-TO Deploy Container Offer with AKS
This repo demonestrate how to deploy Container offer as part of AKS cluster as Azure Managed Application. 


## How To Deploy AKS as Managed App
Please refer to the following repo for detail about how create Managed Application ARM template to deploy AKS
 [Azure Managed Application and AKS with Managed Identity](https://github.com/arsenvlad/azure-managed-app-aks-managed-identity)

 ## Challenges to deploy Container Offer as part of the ARM template

 In order to deploy Container Offer as Part of the Managed App. The following information must be provided as part of the ARM chart.

 ```
    "clusterExtensionTypeName": " Enter the Extension Type",
    "plan-name": "Enter the Plan Name",
    "plan-offerID": " Enter Offer ID",
    "plan-publisher": "Enter the publisher ID",
    "releaseTrain": "stable"
 ```

 Here is example for AzureVote Demo information
 ```
     "clusterExtensionTypeName": "Contoso.AzureVoteKubernetesAppTest",
    "plan-name": "testplan",
    "plan-offerID": "kubernetest_apps_demo_offer",
    "plan-publisher": "test_test_mix3pptest0011614206850774",
    "releaseTrain": "stable"
 ```

## Azure Managed Application Offer Resources
You can learn more about Azure Managed Application offer using the following resources
1. [Mastering The Marketplace](https://microsoft.github.io/Mastering-the-Marketplace/ama/)
1. [Azure Managed Applications overview](https://learn.microsoft.com/en-us/azure/azure-resource-manager/managed-applications/overview)

## Azure Container Offer Resources
You can learn more about Azure Container offer using the following resources
1. [Mastering The Marketplace](https://microsoft.github.io/Mastering-the-Marketplace/container/)
1. [Azure Container Offer overview](https://learn.microsoft.com/en-us/azure/marketplace/marketplace-containers)
