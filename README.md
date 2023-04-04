# Azure Marketplace Managed Apps Accelerator ToolKit

This repository contains several reference implementations of solutions to emit metered billing usage events from within an Azure Managed Application. 

## PowerShell script
Post a metered billing event from a VM deployed inside a managed resource group.
1. [Virtual Machine Example](./vm/README.md)
## Azure Functions
Post a metered billing event from an Azure function. 
1. Post a metered billing event [from a Azure Function HTTP Trigger using header request](./function/ama-custom-billing-msi-trigger) using a C#.
1. Post a metered billing event [from a Azure Function HTTP Trigger using body request](./function/ama-custom-billing-msi-trigger-with-request-body) using a C#.
1. Post Recurring metered billing event [from a Azure Function Timer](./function/ama-custom-billing-msi-timer) using a C#.
<br/>

## Post a Metered Billing Event Using  Notification webhook
1. Post a metered billing event on creation [from Managed Application Notification webhook](./function/ama-custom-billing-notification-webhook) using a C#.

<br/>

## Azure DevOps CI/CD 
This is reference implementation on HOW-TO **Publishers** can [implement CI/CD process](./azure-devops-cicd/README.md) to update their managed application instance in customers environment. The implementation highlists HOW-TO
1. Implement managed app ARM Delta 
1. Update PaaS resources code like WebApp or Azure Function code.
