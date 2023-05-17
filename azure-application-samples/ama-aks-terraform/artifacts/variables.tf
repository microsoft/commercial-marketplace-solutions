variable "agent_count" {
  default = 3
}

variable "cluster_name" {
  default = "k8stest"
}

variable "dns_prefix" {
  default = "k8stest"
}

variable "log_analytics_workspace_name" {
  default = "k8sLogAnalyticsWorkspaceName"
}

# Refer to https://azure.microsoft.com/pricing/details/monitor/ for Log Analytics pricing
variable "log_analytics_workspace_sku" {
  default = "PerGB2018"
}

variable "resource_group_location" {
  default     = "eastus"
  description = "Location of the resource group."
}

variable "resource_group_name" {
  default     = "demo-aks"
  description = "resource group name"
}

variable "ssh_public_key" {
  default = "ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAACAQDR3ELgvUalTKRVAPNoYEDyecX9QH4rTgPwWFuQ+b33/BBZDTGVwJtAZwCqASaGvc7Bmb3d4q/1Cyd7MHGV1rBD/UJvf4TfyVzQDk0q0DqV2oovlUI2CL9EOIGqN2c0Z4g6GvaPnMMwKSKpc2m4UiJgXPQcH2lwb7PDxrpTJTgP8F3iVX8yvH+zxw5KgN4K3K1D9K17OICo6vcaP2HB/cKRxQoDTEEBQAsglzeoXGzkP4yMwwV2uG4GhuG1urR8ZGKSfDohi67h2pgYLiRjQQ9LTRHTemCqreBIuyEOSPJNwrsIoS5xNqkdzeMzTr+1pL126c8cRgG8ENe7Qvrbqx3cVXK1Ezec4yYk1pCiGxu5x4P9WYmWcvjp/p1HZ3tuZyGYRAUx0KTqkm/TQJxxzWBoUTF6b6MNwIM6qW5N9NM7Q6nEghSWjZXaN/FXTN5OxVV5ygKUz2W5S5q8W3ySBKIec2Ee6M9kdFGzU6Qv9mGYpKU7/Ys2jBxYftQxw/fh2UoXRN69wwP+j4reAAbSJnlfFu6T5wUtlNEJAR5vYm6ek45FXeVzpU4nhzJqjdnrhsF7FSeSV/HXx1tl043me3X/qbB2nKF96T6kZLrAbzQUmecExahBIGJCLMbexMcFuii4nPz1wP0X2pT8EI2rfC8oSoxnf9SNHEcNHm6gB4RA5w== masalem@DESKTOP-S24FNT7"
}