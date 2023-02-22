# com.vorba.sand.method

## Clients
1. Generate for Typescript + Angular:
```
# openapi-generator-cli generate -g typescript-angular -i http://localhost:7193/api/swagger.json -o com.vorba.sand.method\clients\typescript-angular -p npmName=vorba-blog-api
openapi-generator-cli generate -g typescript-angular -i http://localhost:7193/api/swagger.json -o com.vorba.sand.method\clients\com.vorba.blog\vorba-blog\src\app\core\api\v1 -p npmName=vorba-blog-api
```

## Angular Api Client
- [strictTemplates enabled](https://angular.io/guide/angular-compiler-options#stricttemplates)
- 