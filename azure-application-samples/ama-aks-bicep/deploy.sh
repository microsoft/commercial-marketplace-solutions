#!/bin/bash

az login --service-principal -u $ARM_CLIENT_ID  -p $ARM_CLIENT_SECRET --tenant $ARM_TENANT_ID


unzip mainbicep.zip -d . 

az deployment group create --subscription $ARM_SUBSCRIPTION --resource-group $MG_RESOURCE_GROUP_NAME \
 --template-file main.bicep  \
 --parameters storageAccountName=$STORAGE_NAME \
   storageAccountType=$STORAGE_TYPE 


