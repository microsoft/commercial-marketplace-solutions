terraform {
  required_version = ">=1.0"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>3.0"
    }
    random = {
      source  = "hashicorp/random"
      version = "~>3.0"
    }
  }
  backend "azurerm" {
    resource_group_name  = "msalem-aks"
    storage_account_name = "msalemstorage"
    container_name       = "terraform"
    key                  = "prod.tfstate"
    client_secret        = "L9P8Q~2U73KvPfThtytwtp5iFXQUPtXMuKF5ea.H"
    client_id            = "0d995ae2-15e6-4e61-b4b3-f8dccf29f4aa"
    subscription_id      = "c4a29eac-da9a-40d8-80ac-0dc69cdd51ca"
    tenant_id            = "970d1b35-db9b-48b2-b163-32c001b79bf8"
  }
}

provider "azurerm" {
  features {}
}