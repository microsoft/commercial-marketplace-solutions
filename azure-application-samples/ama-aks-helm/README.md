# Azure Managed Application and AKS with Helm
This repo demonestrate how to deploy AKS cluster and Microservices solution as Azure Managed Application. The demo will provision AKS cluster and deploy Azure To-DO app using Helm Chart.


## How To Deploy AKS as Managed App
Please refer to the following repo for detail about how create Managed Application ARM template to deploy AKS

 [Azure Managed Application and AKS with Managed Identity](https://github.com/arsenvlad/azure-managed-app-aks-managed-identity)



 ## Challenges to deploy Helm chart using ARM template

 In order to deploy helm chart after AKS deployment there are few consideration before going forward with deployment.

 ### Managed Application will allow only traffic to ISV ACR to pull the images. 
 ISV can create ACR and push all the required container images to ACR. ISV will include ACR information and reference in the `mainTemplate.json`

 ### Managed Application will block any traffic to download package like Helm or Kubectl
 ISV can download helm package and also zip helm chart and add them to `artifacts` folder
 
 ### ARM template does not support Helm or Kubectl out of the box
 ISV can use `deployment Script` to execute helm command agains the AKS cluster to deploy helm chart.


 ## deploy Helm chart as part of Managed Application ARM template
1. As ISV create ACR and publish all needed containers to ACR. Please create one using the following [doc](https://docs.microsoft.com/en-us/azure/container-registry/container-registry-get-started-portal)
1. In this demo there are 2 containers. `mongo:latest` and `docker.io/magdysalem/nodejstodo:v3`. Please pull both locally then push to your ACR
1. Edit `mainTemplate.json` and update `ImportContainerImages` script with ISV ACR information.
1. Compress the content of ama-aks and publish it as Managed application
1. publish in your [Partner Center account](https://docs.microsoft.com/en-us/azure/azure-resource-manager/managed-applications/publish-marketplace-app) by uploading the ama-aks.zip file to the relevant plan's technical configuration.
1. After the app is available in preview, search for the app in <https://poral.azure.com> "Create a resource", and deploy using the UI. 







