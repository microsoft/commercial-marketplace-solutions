# VM running as user assigned managed identity having access to managed app

The template defines a user assigned managed identity resource (of type 'Microsoft.ManagedIdentity/userAssignedIdentities').

The VM runs under this managed identity. This identity assumes the reader role for the managed resource group and specifies the cross tenant access though '**delegatedManagedIdentityResourceId**' property.

The template then defines a nested template to be run after this deployment complete. Important note is to use 'incremental' mode on this template. The template defines a role assignment resource that assigns the reader role for the user assigned managed identity to the managed application created in the main template. Please notice the 'scope' property for the role assignment.

The additional nested template named 'outputsForDeployent' is used for debugging.
