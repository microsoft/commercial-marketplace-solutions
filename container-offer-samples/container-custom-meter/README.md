# Azure Compute with Container Custom Meter

The Azure Compute application is a simple sample used Azure Cognitive Serive with store the transaction in mongoDb. The application consists of three pods, one running a nodeJs web form, and the second a mongodo instance for storage. Third pod is meterEngine to submit data to Marketplace API

## Helm Chart

Helm is the package manager for Kubernetes. In other words, it is used to help you manage Kubernetes applications. Helm is the Kubernetes equivalent of yum or apt. Helm deploys charts, which you can think of as a packaged application.

The Helm Chart contains the following.

- `values.yaml` file that drive and customize the deployment
- `templates` set of template deployment files that helm convert to K8s deployment file during the execution time.

## ARM Templates 

- The `cluster-deployment.json` defines the configuration for a Kubernetes deployment. Settings may be defined that specify details of creating pods, minimum pods in a cluster, scalability, and other settings.
- The `createUIDefinition.json` file is used to customize the customer's interface during the installation process. It allows for various controls to be shown on the screen to collect values needed to install the application.

## manifest.yaml

- The `manifest.yaml`: file brings together all elements of the deployment package by pointing at the other files you've been updating.

# How to use the application
1. Compile the `src` code. 
1. Use `Dockerfile` to build image and push it to ACR
1. update Helm chart `value.yaml` under `AzureCompute` with image Digest and ACR
1. Update `manifest` file with ACR information
1. Follow [MTM Container Labs: Create CNAB Bundle](https://microsoft.github.io/Mastering-the-Marketplace/container/Labs/lab2-create-cnab-bundle-package/) material to build CNAB bundle
1. Use the CNAB to complete container offer