# SaaS-Metered-Demo

This repo shows HOW-TO deploy  Azure Container Offer with SaaS offer as Bundle to provide Metered feature.

For more about Azure Container Offer please review the following resources

## Azure Container Offer Resources
You can learn more about Azure Container offer using the following resources
1. [Mastering The Marketplace](https://microsoft.github.io/Mastering-the-Marketplace/container/)
1. [Azure Container Offer overview](https://learn.microsoft.com/en-us/azure/marketplace/marketplace-containers)


## Azure SaaS Offer Resources
You can learn more about Azure Container offer using the following resources
1. [Mastering The Marketplace](https://microsoft.github.io/Mastering-the-Marketplace/saas/)
1. [Azure SaaS Offer overview](https://learn.microsoft.com/en-us/azure/marketplace/plan-saas-offer)


The following Diagram shows how the SaaS and Container offer will interact together to provide metered usage capability.
## Demo Diagram
![SaaS Metered](./images/diagram.png)

There is the expected workflow steps
1. ISV will publish SaaS and Continers offers
1. Customer will subscribe to SaaS Plan
1. Customer will capture the subscription GUID and plan information
1. Once Subscription is Activate it then Customer will subscribe to container offer and deploy it.
1. As part of deployment, UI definition may request Subscription GUID and Plan. In this demo this information needed. However, ISV can capture the subscription information by other means.
1. The container solution will send metered information to ISV.
1. In this implementation container solution is sending metered information Azure Function HTTP Trigger endpoint
1. Azure Function HTTP Trigger will store the information to CosmosDB
1. ISV will submit metered information to MP using Batch or PUT process.
1. In this implementation Azure Function Timer will run every hour and check the CosmosDB to collect the metered information and submit it to MP.



