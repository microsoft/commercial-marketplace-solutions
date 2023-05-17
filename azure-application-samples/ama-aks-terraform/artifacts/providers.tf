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
    resource_group_name  = ""
    storage_account_name = ""
    container_name       = ""
    key                  = "prod.tfstate"
    client_secret        = ""
    client_id            = ""
    subscription_id      = ""
    tenant_id            = ""
  }
}

provider "azurerm" {
  features {}
}