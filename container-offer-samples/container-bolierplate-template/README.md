# azure-ToDo
The Azure ToDo application is a bolierplate for Container Offer technical requirment. The application consists of two pods, one running a nodeJs web form, and the second a mongodo instance for data storage. 

The structure under folder `src` contains the main structure and component to create container offer

## Helm Chart 
Helm is the package manager for Kubernetes. In other words, it is used to help you manage Kubernetes applications. Helm is the Kubernetes equivalent of yum or apt. Helm deploys charts, which you can think of as a packaged application.

Helm Chart contains 
- `values.yaml` file that drive and customize the deployment
- `templates` set of template deployment files that helm convert to K8s deployment file during the execution time.


## ARM Templates 
- `cluster-deployment.json` :This file defines the configuration for a Kubernetes deployment. Settings may be defined that specify details of creating pods, minimum pods in a cluster, scalability, and other settings.

- `createUIDefinition.json` file is used to customize the customer's interface during the installation process. It allows for various controls to be shown on the screen to collect values needed to install the application.

## mainfest
- `The manifest.yaml`: file brings together all elements of the deployment package by pointing at the other files you've been updating.

