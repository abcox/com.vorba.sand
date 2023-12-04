using com.vorba.data.Interfaces;
using com.vorba.data.Models;
using com.vorba.sand.services2.CosmosDb.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;

namespace com.vorba.sand.services2.CosmosDb
{
    public class CosmosDbService: ICosmosDbService
    {
        private CosmosClient _cosmosClient;

        public CosmosDbService(
            //ILogger<CosmosDbService> log,
            //OpenApiSettings openapi,
            //IObjectDataService objectDataService
            IOptions<CosmosDbDemoServiceOptions> cosmosDbDemoServiceOptions
            )
        {
            //this._logger = log.ThrowIfNullOrDefault();
            //this._openapi = openapi.ThrowIfNullOrDefault();
            //this._objectDataService = objectDataService;

            _cosmosClient = GetCosmosClient(cosmosDbDemoServiceOptions?.Value);
        }

        public static CosmosClient GetCosmosClient(CosmosDbDemoServiceOptions options)
        {
            var clientOptions = new CosmosClientOptions
            {
                ApplicationName = options?.ApplicationName ?? "CosmosDBDotnetQuickstart",
                //SerializerOptions = new CosmosSerializationOptions()
                //{
                //    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
                //}
            };
            if (options?.ClientOptions?.SerializerOptions != null)
            {
                clientOptions.SerializerOptions = new CosmosSerializationOptions
                {
                    PropertyNamingPolicy = options.ClientOptions.SerializerOptions.PropertyNamingPolicy
                };
            }
            var endpointUri = options?.EndpointUri;
            var primaryKey = options?.PrimaryKey;
            return new CosmosClient(endpointUri, primaryKey, clientOptions);
        }

        public async Task<BaseResponse> CreateDatabaseAsync(string id)
        {
            //string name = req.Query["name"];
            //_logger.LogInformation($"{nameof(CreateDatabase)} name: '{name}'");

            //var response = await _objectDataService.CreateDatabaseAsync(name);
            BaseResponse result = new BaseResponse { Status = Status.Incomplete };
            try
            {
                DatabaseResponse databaseResponse = await _cosmosClient.CreateDatabaseAsync(id);
                result = new BaseResponse { Status = Status.Succeeded, Message = "Database created." };
            }
            catch (CosmosException cosmosException) when (cosmosException.StatusCode == HttpStatusCode.Conflict)
            {
                var reason = $"Failed to create database with id {id}. A database with that id already exists.";
                //_logger.LogError($"{reason}");
                //return new BadRequestObjectResult($"{reason}");
                result = new BaseResponse { Status = Status.Failed, Message = $"{reason}" };
            }
            catch (Exception exception)
            {
                //var reason = $"Failed to create database with id {name}";
                //_logger.LogError($"{reason}. ERROR: {ex.Message}");
                //return new BadRequestObjectResult($"{reason}.");
                result = new BaseResponse { Status = Status.Failed, Message = $"Unhandled exception message: {exception.Message}" };
            }
            return result;
        }

        public async Task<BaseResponse> DeleteDatabaseAsync(string id)
        {
            //string name = req.Query["name"];
            //_logger.LogInformation($"{nameof(CreateDatabase)} name: '{name}'");

            //var response = await _objectDataService.CreateDatabaseAsync(name);
            BaseResponse result = new BaseResponse { Status = Status.Incomplete };
            try
            {
                var database = _cosmosClient.GetDatabase(id); // no call - returns proxy
                DatabaseResponse databaseResponse = await database.DeleteAsync();
                result = new BaseResponse { Status = Status.Succeeded, Message = $"Database having Id '{id}' was deleted." };
            }
            catch (CosmosException cosmosException) when (cosmosException.StatusCode == HttpStatusCode.NotFound)
            {
                var reason = $"Failed to delete database with id {id}. A database with that id was not found.";
                //_logger.LogError($"{reason}");
                //return new BadRequestObjectResult($"{reason}");
                result = new BaseResponse { Status = Status.Failed, Message = $"{reason}" };
            }
            catch (Exception exception)
            {
                //var reason = $"Failed to create database with id {name}";
                //_logger.LogError($"{reason}. ERROR: {ex.Message}");
                //return new BadRequestObjectResult($"{reason}.");
                result = new BaseResponse { Status = Status.Failed, Message = $"exception: {exception.Message}" };
            }
            return result;
        }

        public async Task<BaseResponse> CreateDatabaseContainerAsync(string databaseId, string containerId, string partitionKeyPath)
        {
            BaseResponse result = new BaseResponse { Status = Status.Incomplete };
            try
            {
                var database = _cosmosClient.GetDatabase(databaseId); // no call - returns proxy
                ContainerProperties containerProperties = new ContainerProperties { Id = containerId, PartitionKeyPath = partitionKeyPath };
                ContainerResponse containerResponse = await database.CreateContainerAsync(containerProperties);
                result = new BaseResponse { Status = Status.Succeeded, Message = $"Container with Id '{containerResponse.Container.Id}' created in database with Id '{containerResponse.Container.Database.Id}'." };
            }
            catch (CosmosException cosmosException) when (cosmosException.StatusCode == HttpStatusCode.NotFound)
            {
                result = new BaseResponse { Status = Status.Failed, Message = $"cosmosException: {cosmosException.Message}" };
            }
            catch (Exception exception)
            {
                result = new BaseResponse { Status = Status.Failed, Message = $"exception: {exception.Message}" };
            }
            return result;
        }

        public async Task<BaseResponse> CreateCharacterAsync(Character character)
        {
            BaseResponse result = new BaseResponse { Status = Status.Incomplete };
            try
            {
                var database = _cosmosClient.GetDatabase("rickandmorty"); // no call - returns proxy
                //ContainerProperties containerProperties = new ContainerProperties { Id = containerId, PartitionKeyPath = partitionKeyPath };
                var container = database.GetContainer("character");
                //dynamic blob = new { };
                //blob.Id = Guid.NewGuid().ToString();
                //blob.PartitionKey = data.Name;
                ItemResponse<Character> response = await container.CreateItemAsync(character, new PartitionKey(character.Id));
                result = new BaseResponse { Status = Status.Succeeded, Message = $"Character added. ActivityId: {response.ActivityId}" };
            }
            catch (CosmosException cosmosException) when (cosmosException.StatusCode == HttpStatusCode.NotFound)
            {
                result = new BaseResponse { Status = Status.Failed, Message = $"cosmosException: {cosmosException.Message}" };
            }
            catch (Exception exception)
            {
                result = new BaseResponse { Status = Status.Failed, Message = $"exception: {exception.Message}" };
            }
            return result;
        }

        public async Task<BaseResponse> UpdateCharacterAsync(Character character)
        {
            BaseResponse result;
            try
            {
                var database = _cosmosClient.GetDatabase("rickandmorty"); // no call - returns proxy
                var container = database.GetContainer("character");
                ItemResponse<Character> itemResponse = await container.ReadItemAsync<Character>(character.Id, new PartitionKey(character.Id));
                itemResponse = await container.ReplaceItemAsync(character, character.Id, new PartitionKey(character.Id));
                result = new BaseResponse { Status = Status.Succeeded, Message = $"Character updated. ActivityId: {itemResponse.ActivityId}" };
            }
            catch (CosmosException cosmosException) when (cosmosException.StatusCode == HttpStatusCode.NotFound)
            {
                result = new BaseResponse { Status = Status.Failed, Message = $"cosmosException: {cosmosException.Message}" };
            }
            catch (Exception exception)
            {
                result = new BaseResponse { Status = Status.Failed, Message = $"exception: {exception.Message}" };
            }
            return result;
        }

        public async Task<BaseResponse> ReadCharacterAsync(string id)
        {
            BaseResponse result;
            try
            {
                var database = _cosmosClient.GetDatabase("rickandmorty"); // no call - returns proxy
                var container = database.GetContainer("character");
                //var q = container.GetItemLinqQueryable<Character>();
                //var item = q.Where(p => p.Id == id).Single();
                var itemResponse = await container.ReadItemAsync<Character>(id, new PartitionKey(id));
                if (itemResponse == null)
                {
                    result = new BaseResponse { Status = Status.NotFound, Message = "Character not found." };
                }
                else
                {
                    var character = itemResponse.Resource;
                    result = new BaseResponse { Data = new[] { character }, Status = Status.Succeeded, Message = "Character found." };
                }
            }
            catch (CosmosException cosmosException) when (cosmosException.StatusCode == HttpStatusCode.NotFound)
            {
                result = new BaseResponse { Status = Status.Failed, Message = $"cosmosException: {cosmosException.Message}" };
            }
            catch (Exception exception)
            {
                result = new BaseResponse { Status = Status.Failed, Message = $"exception: {exception.Message}" };
            }
            return result;
        }

        public async Task<BaseResponse> DeleteCharacterAsync(string id)
        {
            BaseResponse result = new BaseResponse { Status = Status.Incomplete };
            try
            {
                var database = _cosmosClient.GetDatabase("rickandmorty"); // no call - returns proxy
                var container = database.GetContainer("character");
                ItemResponse<Character> response = await container.DeleteItemAsync<Character>(id, new PartitionKey(id));
                result = new BaseResponse { Data = new[] { response.Resource }, Status = Status.Succeeded, Message = "Character deleted." };
            }
            catch (CosmosException cosmosException) when (cosmosException.StatusCode == HttpStatusCode.NotFound)
            {
                result = new BaseResponse { Status = Status.Failed, Message = $"cosmosException: {cosmosException.Message}" };
            }
            catch (Exception exception)
            {
                result = new BaseResponse { Status = Status.Failed, Message = $"exception: {exception.Message}" };
            }
            return result;
        }

        public async Task<BaseResponse> GetCharactersAsync(string[] ids)
        {
            BaseResponse result = new BaseResponse { Status = Status.Incomplete };
            try
            {
                var database = _cosmosClient.GetDatabase("rickandmorty"); // no call - returns proxy
                var container = database.GetContainer("character");
                var q = container.GetItemLinqQueryable<Character>();
                var iterator = q.Where(p => ids.Contains(p.Id)).ToFeedIterator();
                List<Character> items = new List<Character>();
                while (iterator.HasMoreResults)
                {
                    var results = await iterator.ReadNextAsync();
                    items.AddRange(results);
                }
                result = new BaseResponse { Data = items, Status = Status.Succeeded, Message = $"Characters retrieved." };
            }
            catch (CosmosException cosmosException) when (cosmosException.StatusCode == HttpStatusCode.NotFound)
            {
                result = new BaseResponse { Status = Status.Failed, Message = $"cosmosException: {cosmosException.Message}" };
            }
            catch (Exception exception)
            {
                result = new BaseResponse { Status = Status.Failed, Message = $"exception: {exception.Message}" };
            }
            return result;
        }

        public async Task<BaseResponse> GetDatabaseListAsync()
        {
            BaseResponse result = new BaseResponse { Status = Status.Incomplete };
            try
            {
                List<string> names = new List<string>();
                using (FeedIterator<DatabaseProperties> iterator = _cosmosClient.GetDatabaseQueryIterator<DatabaseProperties>())
                {
                    while (iterator.HasMoreResults)
                    {
                        FeedResponse<DatabaseProperties> nextFeedResponse = await iterator.ReadNextAsync();
                        List<string> databaseIdList = nextFeedResponse.Select(x=> x.Id).ToList();
                        names.AddRange(databaseIdList);
                    }
                }
                result = new BaseResponse { Status = Status.Succeeded, Message = $"List of databases retrieved.", Data = names };
            }
            catch (CosmosException cosmosException) when (cosmosException.StatusCode == HttpStatusCode.NotFound)
            {
                //var reason = $"Failed to create database with id {name}. A database with that id already exists.";
                //_logger.LogError($"{reason}");
                //return new BadRequestObjectResult($"{reason}");
                result = new BaseResponse { Status = Status.Failed, Message = $"cosmosException: {cosmosException.Message}" };
            }
            catch (Exception exception)
            {
                //var reason = $"Failed to create database with id {name}";
                //_logger.LogError($"{reason}. ERROR: {ex.Message}");
                //return new BadRequestObjectResult($"{reason}.");
                result = new BaseResponse { Status = Status.Failed, Message = $"exception: {exception.Message}" };
            }
            return result;
        }
    }
}
