# Azure Marketplace Managed Apps metered billing examples

This repository contains several reference implementations of solutions to emit metered billing usage events from within an Azure Managed Application. 

These examples are for reference when building your own solution.

## PowerShell script

Post a metered billing event from a VM deployed inside a managed resource group.

1. [Virtual Machine Example](./vm/README.md)


## Azure Functions

Post a metered billing event from an Azure function. 
1. Post a metered billing event [from a Azure Function HTTP Trigger](./function/ama-custom-billing-msi-trigger/README.md) using a C#.
1. Post Recurring metered billing event [from a Azure Function Timer](./function/ama-custom-billing-msi-timer/README.md) using a C#.

<br>

# Post a Metered Billing Event Using  Notification webhook

1. Post a metered billing event on creation [from Managed Application Notification webhook](./function/ama-custom-billing-notification-webhook/README.md) using a C#.
