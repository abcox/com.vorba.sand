using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Reflection.Metadata;
using System.Threading.Tasks;
//using AutoFixture;
using com.vorba.sand.method.Model;
//using com.vorba.sand.services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Extensions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace com.vorba.sand.method
{
    public class Functions1
    {
        private readonly ILogger<Functions1> _logger;
        private readonly OpenApiSettings _openapi;
        //private readonly IObjectDataService _objectDataService;
        private readonly string EndpointUri = "https://vorba1.documents.azure.com:443/";
        private readonly string PrimaryKey = "JOYD //\\//\\//\\//\\//\\ ****** REDACTED ******* //\\//\\//\\//\\// ******* //\\// XQ==";
        private CosmosClient cosmosClient;

        public Functions1(ILogger<Functions1> log, OpenApiSettings openapi
//            , IObjectDataService objectDataService
            )
        {
            this._logger = log.ThrowIfNullOrDefault();
            this._openapi = openapi.ThrowIfNullOrDefault();
            //this._objectDataService = objectDataService;
            var options = new CosmosClientOptions
            {
                ApplicationName = "CosmosDBDotnetQuickstart",
                SerializerOptions = new CosmosSerializationOptions()
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
                }
            };
            cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, options);
    }

        [FunctionName("Function1")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "deprecated" }, Deprecated = true)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "run")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
        
            string name = req.Query["name"];
        
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;
        
            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";
        
            return new OkObjectResult(responseMessage);
        }

        [FunctionName("Function2")]
        [OpenApiOperation(operationId: "Run2", tags: new[] { "deprecated" }, Deprecated = true)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run2(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "run2")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }

#region // vorba1/admin
        [FunctionName(nameof(CreateDatabase))]
        [OpenApiOperation(operationId: nameof(CreateDatabase), tags: new[] { "vorba1/admin" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        //[OpenApiRequestBody(contentType: MediaTypeNames.Application.Json, bodyType: typeof(Post), Description = "Create database", Required = true)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "Name (id) of database")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(string), Description = "Post created", Summary = "request successful")]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Description = "Success", Summary = "Database created")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = "bad request", Summary = "bad request")]
        public async Task<IActionResult> CreateDatabase(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "database/create")] HttpRequest req)
        {
            string name = req.Query["name"];
            _logger.LogInformation($"{nameof(CreateDatabase)} name: '{name}'");

            //var response = await _objectDataService.CreateDatabaseAsync(name);
            try
            {
                DatabaseResponse databaseResponse = await cosmosClient.CreateDatabaseAsync(name);
                return new OkObjectResult($"Database created. Id: {name}");
            }
            catch (CosmosException cosmosException) when (cosmosException.StatusCode == HttpStatusCode.Conflict)
            {
                var reason = $"Failed to create database with id {name}. A database with that id already exists.";
                _logger.LogError($"{reason}");
                return new BadRequestObjectResult($"{reason}");
            }
            catch (Exception ex)
            {
                var reason = $"Failed to create database with id {name}";
                _logger.LogError($"{reason}. ERROR: {ex.Message}");
                return new BadRequestObjectResult($"{reason}.");
            }
        }

        [FunctionName(nameof(DeleteDatabase))]
        [OpenApiOperation(operationId: nameof(DeleteDatabase), tags: new[] { "vorba1/admin" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        //[OpenApiRequestBody(contentType: MediaTypeNames.Application.Json, bodyType: typeof(Post), Description = "Create database", Required = true)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "Name (id) of database")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(string), Description = "Post created", Summary = "request successful")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: MediaTypeNames.Application.Json, bodyType: typeof(string), Description = "Database not deleted", Summary = "Database does not exist")]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Description = "Success", Summary = "Database created")]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = "bad request", Summary = "bad request")]
        public async Task<IActionResult> DeleteDatabase(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "database/delete")] HttpRequest req)
        {
            string name = req.Query["name"];
            _logger.LogInformation($"{nameof(DeleteDatabase)} name: '{name}'");

            //var response = await _objectDataService.CreateDatabaseAsync(name);
            try
            {
                var database = cosmosClient.GetDatabase(name); // no call - returns proxy
                DatabaseResponse databaseResponse = await database.DeleteAsync();
                return new OkObjectResult($"Database deleted. Id: {name}");
            }
            catch (CosmosException cosmosException) when (cosmosException.StatusCode == HttpStatusCode.NotFound)
            {
                var reason = $"Failed to delete database with id {name}. The database with that id was not found.";
                _logger.LogError($"{reason}");
                return new BadRequestObjectResult($"{reason}");
            }
            catch (Exception ex)
            {
                var reason = $"Failed to delete database with id {name}";
                _logger.LogError($"{reason}. ERROR: {ex.Message}");
                return new BadRequestObjectResult($"{reason}.");
            }
        }

        [FunctionName(nameof(CreateDatabaseContainer))]
        [OpenApiOperation(operationId: nameof(CreateDatabaseContainer), tags: new[] { "vorba1/admin" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        //[OpenApiRequestBody(contentType: MediaTypeNames.Application.Json, bodyType: typeof(Post), Description = "Create database", Required = true)]
        [OpenApiParameter(name: "containerId", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The id (name) of the container")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The id of an existing database to host container")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(string), Description = "Post created", Summary = "request successful")]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Description = "Success", Summary = "Database created")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = "bad request", Summary = "bad request")]
        public async Task<IActionResult> CreateDatabaseContainer(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "database/{id}/container/create")] HttpRequest req, string id)
        {
            string containerId = req.Query["containerId"];
            _logger.LogInformation($"{nameof(CreateDatabaseContainer)} database id: '{id}'");
            _logger.LogInformation($"{nameof(CreateDatabaseContainer)} container id: '{containerId}'");

            //var response = await _objectDataService.CreateDatabaseAsync(name);
            try
            {
                var database = cosmosClient.GetDatabase(id); // no call - returns proxy
                ContainerResponse containerResponse = await database.CreateContainerAsync(containerId, "/partitionKey");
                return new OkObjectResult($"Container created with id {containerId} in database with id {id}.");
            }
            catch (CosmosException cosmosException) when (cosmosException.StatusCode == HttpStatusCode.Conflict)
            {
                var reason = $"Failed to create container with id {id}. A container with that id already exists.";
                _logger.LogError($"{reason}");
                return new BadRequestObjectResult($"{reason}");
            }
            catch (Exception ex)
            {
                var reason = $"Failed to create database with id {id}";
                _logger.LogError($"{reason}. ERROR: {ex.Message}");
                return new BadRequestObjectResult($"{reason}.");
            }
        }
        #endregion // vorba1/admin

        #region admin/config



        #endregion admin/config


        [FunctionName(nameof(CreatePost2))]
        [OpenApiOperation(operationId: nameof(CreatePost2), tags: new[] { "vorba1" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: MediaTypeNames.Application.Json, bodyType: typeof(Post), Description = "Create entry in CosmosDb", Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(Post), Description = "Post created", Summary = "request successful")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = "bad request", Summary = "bad request")]
        public async Task<IActionResult> CreatePost2(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "create")] HttpRequest req)
        {
            _logger.LogInformation($"{nameof(CreatePost2)}");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Post post = JsonConvert.DeserializeObject<Post>(requestBody);
            using var db = new BloggingContext();
            var blog = db.Blogs.Find(post?.BlogId);

            if (blog == null)
                return new BadRequestObjectResult(new { post.BlogId });

            try
            {
                post.Blog = null;
                db.Posts.Add(post);
                db.SaveChanges();
                post.Blog = null; // resolve exception -> Newtonsoft.Json: Self referencing loop detected with type 'Post'. Path 'blog.posts'.

                return new OkObjectResult(post);
            }
            catch (Exception ex)
            {
                _logger.LogError("ERROR", ex);
                throw;
            }
        }
    }
}

