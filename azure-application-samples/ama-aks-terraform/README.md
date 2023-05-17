# azure-managed Deploy AKS with terraform
This is just a POC on HOW-TO use terraform with azure-managed 
1. This is a free code AS IT IS
1. This code will deploy an instance of AKS 
1. Terraform will use storage account as backend to store the state
1. Terraform will using a Azure Service Principle to deploy the AKS. The client Id and password for SP will be passed from [Publisher Key Vault](https://learn.microsoft.com/en-us/azure/azure-resource-manager/managed-applications/key-vault-access)

