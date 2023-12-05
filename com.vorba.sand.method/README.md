# com.vorba.sand.method

## Clients
1. Generate for Typescript + Angular:
```
# openapi-generator-cli generate -g typescript-angular -i http://localhost:7193/api/swagger.json -o com.vorba.sand.method\clients\typescript-angular -p npmName=vorba-blog-api
openapi-generator-cli generate -g typescript-angular -i http://localhost:7193/api/swagger.json -o com.vorba.sand.method\clients\com.vorba.blog\vorba-blog\src\app\core\api\v1 -p npmName=vorba-blog-api
```

## Open Api Standard (swagger)
- https://comvorbasandmethod20230102115619.azurewebsites.net/api/swagger/ui

## Angular Api Client
- [strictTemplates enabled](https://angular.io/guide/angular-compiler-options#stricttemplates)

## Publishing to Azure
- right-click project named 'com.vorba.sand.method' and select publish

## Security
1. To authenticate the OpenAPI (swagger) UI:
   - copy the default apiKey
   - click the button labelled Authorize, and
   - paste value field labelled function_key (apiKey)

## Release Notes
1. Add WebApplication1 as new REST Web API
2. Add Services2 to support CosmosDb use by WebApplication1
3. Add com.vorba.data to support models/view-models, etc.

## Azure Resources

### Key vault

1. Access policy options: [RBAC](https://learn.microsoft.com/en-us/azure/role-based-access-control/overview?WT.mc_id=Portal-Microsoft_Azure_KeyVault), or [legacy](https://learn.microsoft.com/en-us/azure/key-vault/general/assign-access-policy?WT.mc_id=Portal-Microsoft_Azure_KeyVault&tabs=azure-portal)
2. Grant 'Key Vault Secrets User' role to the App Svc via 'Identity' / Azure Role Assignment / Add
3. Review [vorba-sand-kv-2 role assignments](https://portal.azure.com/#@Vorba.onmicrosoft.com/resource/subscriptions/236217f7-0ad4-4dd6-8553-dc4b574fd2c5/resourceGroups/vorba-sand/providers/Microsoft.KeyVault/vaults/vorba-sand-kv-2/users)
4. Note: resolve Visual Studio Azure authority issues with cmd like `az login`

