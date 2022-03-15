# Deploy  Azure Marketplace Managed-App with built-in Custom billing Meter:

This demo shows how to deploy Managed App with Function app to  emit meter usage events to marketplace using **predefined dimensions and quantities** in code.

## Design
The following diagram shows the overall workflow for this demo
![Diagram](./images/Diagram.png)


## Custom billing Senario

This demo shows how to implement a **recurring fix fee billing** that happening once the managed resources are created using Azure function timer crobjob.


## Important Configuration
ARM template expects the following configuration

1. <b>Variable artifacts:</b> ARM template will deploy function from a Zip file. This variable is used to reference to the location of the zip file

1. <b>DIMENSION_CONFIG</b> predefined dimensions and quantities that the function will use to emit usage event to Azure marketplace

1. ARM Template will deploy function using `WEBSITE_RUN_FROM_PACKAGE` and expecting `functionpackage.zip` to place under `artifacts` folder.
![diagram](./images/Diagram4.png)


## Installation from Partner Center Preview

1. Use the contents of the `/function/ama-custom-billing-msi-timer/arm` folder to create a ZIP file for your plan in Partner Center.
1. Upload the ZIP file to the Technical Configuration page of the Azure Managed Application plan in Partner Center.
1. Publish the plan. It will take some time for the plan to reach Preview stage.
1. From Preview, purchase the plan. 
    - Do not purchase the plan from the same Azure Tenant as the one used to publish the plan. If you do so, the script referenced later will error.
    

## Usage from the Managed Application
1. This demo is using **hourly timer** so event will emitted on top of each hour
1. After deployment is complete, open **managed resource group** 
1. Open the created Application Insights resource
![diagram](./images/Diagram7.png)
1. From logs, Run the following query. Make sure to wait till the top of the hour to see the emitting events 
```
traces 
| where operation_Name contains "job" and message contains "emit"
```
![diagram](./images/Diagram6.png)

1. Confirm there are successful emitting events