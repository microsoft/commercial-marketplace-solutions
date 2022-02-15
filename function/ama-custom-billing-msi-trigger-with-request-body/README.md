# Deploy  Azure Marketplace Managed-App with built-in Custom billing Meter:

This demo shows how to deploy Managed App with Function app to  emit meter usage events to marketplace using http trigger.

## Design
The following diagrm shows the overall workflow for this demo
![Diagram](./images/Diagram.png)


## Custom billing Senario

This demo shows how to implement a custom billing using Azure  http trigger function where usage information will be passed on request body as following.
```
{
    "dimension" : "test",
    "quantity"  : 1
}
```


## Important Configuration
ARM template expects the following configuration

1. <b>_artifactsLocation</b> ARM template will deploy function from a Zip file so it is expected to pass the location of the zip file using this `_artifactsLocation` parameter

1. <b>_artifactsLocationSasToken</b> It is optional and only required if accessing the function zip file require a token

1. <b>DIMENSION_CONFIG</b> predefined dimensions and quantities that the function will use to emit usage event to Azure marketplace


1. ARM using resource type `Microsoft.Resources/deployments` 
![diagram](./images/Diagram2.png)
In order to pass the Partner center validation you need to update the resource name with the Custerom PID Guid from PartnerCenter
![diagram](./images/Diagram3.png)

1. ARM Template will deploy function using `WEBSITE_RUN_FROM_PACKAGE` and expecting `functionpackage.zip` to place under `artifacts` folder.
![diagram](./images/Diagram4.png)



## Installation from Partner Center Preview

1. Use the contents of the `/function/ama-custom-billing-msi-trigger-with-request-body/arm` folder to create a ZIP file for your plan in Partner Center.
1. Upload the ZIP file to the Technical Configuration page of the Azure Managed Application plan in Partner Center.
1. Publish the plan. It will take some time for the plan to reach Preview stage.
1. From Preview, purchase the plan. 
    - Do not purchase the plan from the same Azure Tenant as the one used to publish the plan. If you do so, the script referenced later will error.
    

## Usage from the Managed Application

1. After deployment is complete, open **managed resouce group** 
1. Open function app and Click **Functions**
1. Click Webhook  and Click **Code + Test ** to get Fuction URL
![diagram](./images/Diagram7.png)
1. Open Cloud Shell and run the curl command 
```
curl  -X Post <webhook function url> -d '{"dimension":"<Enter Dimension Name>","quantity":"Enter Quantity"}'
```
1. Open ApplicationInsight and  run the following query 
```
traces 
| where operation_Name contains "Webhook" and message contains "emit"
```
![diagram](./images/Diagram6.png)

1. Confirm there are successful emitting events