# Deploy Azure Managed App with built-in Custom billing Meter

This demo shows how to deploy Managed App with Function app to emit meter usage events to marketplace using http trigger and  **predefined dimensions and quantities** in code.

## Design

The following diagram shows the overall workflow for this demo.

![Diagram](./images/Diagram.png)

## Custom billing Senario

This demo shows how to implement a custom billing using Azure http trigger function where usage information will be passed on the request header query

For this demo, we setup plan information as following
![diagram](./images/Diagram5.png)

## Important Configuration

ARM template expects the following configuration

1. **Variable artifacts** - ARM template will deploy function from a Zip file. This variable is used to reference to the location of the zip file
2. ARM using resource type `Microsoft.Resources/deployments`
    ![diagram](./images/Diagram2.png)

    In order to pass the Partner center validation you need to update the resource name with the Customer PID Guid from Partner Center.

    ![diagram](./images/Diagram3.png)

3. ARM Template will deploy function using `WEBSITE_RUN_FROM_PACKAGE` and expecting `functionpackage.zip` to place under `artifacts` folder.
![diagram](./images/Diagram4.png)

## Installation from Partner Center Preview

1. Use the contents of the `/function/ama-custom-billing-msi-trigger/arm` folder to create a ZIP file for your plan in Partner Center.
1. Upload the ZIP file to the Technical Configuration page of the Azure Managed Application plan in Partner Center.
1. Publish the plan. It will take some time for the plan to reach Preview stage.
1. From Preview, purchase the plan.
    - Do not purchase the plan from the same Azure Tenant as the one used to publish the plan. If you do so, the script referenced later will error.

## Usage from the Managed Application

1. After deployment is complete, open **managed resource group**
1. Open function app and Click **Functions**
1. Click Webhook  and Click **Code + Test** to then click **Test/Run**

    ![diagram](./images/Diagram7.png)

1. In the Testing Panel, Under **Query** Section, Click **Add parameter** and enter

    ```text
    Name = dimension
    Value = key
    ```

1. Click **Add parameter** and enter the following

    ```text
    Name = quantity
    Value = 1
    ```

    ![diagram](./images/Diagram8.png)

1. Click **Run** and you should see the following successful emitting event.

    ![diagram](./images/Diagram9.png)
