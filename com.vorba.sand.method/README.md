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
