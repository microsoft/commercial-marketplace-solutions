# Post an Azure Managed App Billing Event from a VM

A sample that will allow you to posting events to metered billing API in a Managed Application from a VM. The VM is deployed into the managed resource group.

## Installation from Partner Center Preview

1. Use the contents of the `/vm/arm` folder to create a ZIP file for your plan in Partner Center, or just use the `app.zip` file already in the repo.
1. Upload the ZIP file to the Technical Configuration page of the Azure Managed Application plan in Partner Center.
1. Publish the plan. It will take some time for the plan to reach Preview stage.
1. From Preview, purchase the plan. 
    - Do not purchase the plan from the same Azure Tenant as the one used to publish the plan. If you do so, the script referenced later will error.
    - Use the `East US` region using this example to ensure support for the VM SKU specified in the ARM template.

## Usage from the Managed Application VM

1. After deployment is complete, remote into the newly deployed VM.
1. Open Internet Explorer (IE).
    1. After opening IE, set the custom security settings to allow downloads.
1. Download this repository's code as a ZIP file [from this URL](https://github.com/dstarr/ama-cu/archive/refs/heads/main.zip).
1. In PowerShell, navigate to the `/vm/ps` folder on the VM.
    1. Run the command `notepad ./RunIt.ps1`.
    1. Open the [RunIt](./vm/ps/RunIt.ps1) file and see the arguments you need to pass to the Invoke-Meter function.
    1. Provide the correct values for the arguments.
    1. Run the command `Set-ExecutionPolicy Bypass`.
    1. Run the command `./RunIt.ps1`.
1. The results of the call will be show at the bottom of the terminal window after the script runs.
    1. When the meter has been posted successfully it will show the output of the reply JSON at the bottom of the screen.
    1. If there is an error - The meter did not get called or the meter is a duplicate of one sent earlier. The error message will indicate which.
