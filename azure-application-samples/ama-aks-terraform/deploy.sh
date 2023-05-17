#!/bin/bash

az login --service-principal -u $ARM_CLIENT_ID  -p $ARM_CLIENT_SECRET --tenant $ARM_TENANT_ID
az storage account create -n "$TF_STORAGE_NAME" -g "$MG_RESOURCE_GROUP_NAME" --location $LOCATION --sku Standard_LRS
export AZURE_STORAGE_KEY="$(az storage account keys list -g "$MG_RESOURCE_GROUP_NAME" -n "$TF_STORAGE_NAME" --query '[0].value' -o tsv)"
az storage container create -n "terraformstate" --account-key $AZURE_STORAGE_KEY --account-name $TF_STORAGE_NAME

unzip terraformfiles.zip -d . 
chmod +x ./terraform 
./terraform init \
    -backend-config=storage_account_name=$TF_STORAGE_NAME \
    -backend-config=container_name=terraformstate \
    -backend-config=key=prod.tfstate \
    -backend-config=resource_group_name=$MG_RESOURCE_GROUP_NAME\
    -backend-config=subscription_id=$ARM_SUBSCRIPTION_ID \
    -backend-config=tenant_id=$ARM_TENANT_ID \
    -backend-config=client_id=$ARM_CLIENT_ID \
    -backend-config=client_secret=$ARM_CLIENT_SECRET

./terraform import azurerm_resource_group.rg $MG_RESOURCE_GROUP_ID

./terraform apply -input=false -auto-approve \
            -var resource_group_name=$MG_RESOURCE_GROUP_NAME \
            -var cluster_name=$CLUSTER_NAME \
            -var dns_prefix=$DNS_PREFIX  \
            -var resource_group_location=$LOCATION 

