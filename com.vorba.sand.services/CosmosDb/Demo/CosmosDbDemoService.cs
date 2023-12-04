using com.vorba.sand.services.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using System.Net;

namespace com.vorba.sand.services.CosmosDb.Demo
{
    public class CosmosDbDemoService: IObjectDataService
    {
        public static readonly int ThroughputValueIncrementDefault = 100;

        private readonly ILogger<CosmosDbDemoServiceOptions> logger;
        private readonly CosmosDbDemoServiceOptions options;
        private CosmosClient cosmosClient;
        private Database? database;
        private Container? container;

        public CosmosDbDemoService(
            ILogger<CosmosDbDemoServiceOptions> logger,
            IOptions<CosmosDbDemoServiceOptions> options
            )
        {
            this.logger = logger;
            this.options = options.Value;

            this.cosmosClient = new CosmosClient(this.options.EndpointUri, this.options.PrimaryKey, new CosmosClientOptions() { ApplicationName = "CosmosDBDotnetQuickstart" });

            //Task.Run(() => init().Wait());
        }

        //private CosmosClient Client =>
        //    cosmosClient ??= new CosmosClient(
        //        options.EndpointUri, options.PrimaryKey,
        //        new CosmosClientOptions() { ApplicationName = options.ApplicationName }
        //        );

        private CosmosClient Client
        {
            get
            {
                try
                {
                    //cosmosClient ??= new CosmosClient(
                    //    options.EndpointUri, options.PrimaryKey,
                    //    new CosmosClientOptions() { ApplicationName = options.ApplicationName }
                    //    );
                    this.cosmosClient = new CosmosClient(options.EndpointUri, options.PrimaryKey, new CosmosClientOptions() { ApplicationName = "CosmosDBDotnetQuickstart" });

                }
                catch ( Exception ex )
                {
                    logger.LogError("Client create failed: {exceptionMessage}", ex.Message);
                }
                return cosmosClient;
            }
        }


        //private async Task init()
        //{
        //    await CreateDatabaseAsync();
        //    await CreateContainerAsync();
        //    await ScaleContainerAsync();
        //    await AddItemsToContainerAsync();
        //}

        public async Task<DatabaseResponse> CreateDatabaseAsync(string name)
        {
            try
            {
                var result = await cosmosClient.CreateDatabaseIfNotExistsAsync(id: name);
                logger.LogInformation("Database created (id: {databaseId})", database.Id);
                return result;
            }
            catch(Exception ex)
            {
                logger.LogError("Database create failed: {exceptionMessage}", ex.Message);
            }
            return null;
        }

        private async Task CreateContainerAsync()
        {
            if (database == null)
            {
                logger.LogError("Database required");
                return;
            }
            container = await database.CreateContainerIfNotExistsAsync(options.ContainerId, "/partitionKey");
            logger.LogInformation("Container created (id: {containerId})", container.Id);
        }

        private async Task ScaleContainerAsync()
        {
            // Read the current throughput
            try
            {
                if (container == null)
                {
                    logger.LogError("Container required");
                    return;
                }
                int? throughput = await container.ReadThroughputAsync();
                if (throughput.HasValue)
                {
                    logger.LogInformation("Current provisioned throughput: {throughputValue}", throughput.Value);
                    int newThroughputValue = throughput.Value + options.ThroughputValueIncrement ?? ThroughputValueIncrementDefault;
                    // Update throughput
                    await container.ReplaceThroughputAsync(newThroughputValue);
                    logger.LogInformation("New provisioned throughput: {newThroughputValue}", newThroughputValue);
                }
            }
            catch (CosmosException cosmosException) when (cosmosException.StatusCode == HttpStatusCode.BadRequest)
            {
                logger.LogError("Cannot read container throuthput. Exception: {cosmosExceptionResponseBody}", cosmosException.ResponseBody);
            }
        }

        private async Task AddItemsToContainerAsync()
        {
            if (container == null)
            {
                logger.LogError("Container required");
                return;
            }

            // Create a family object for the Andersen family
            Family andersenFamily = new Family
            {
                Id = "Andersen.1",
                PartitionKey = "Andersen",
                LastName = "Andersen",
                Parents = new Parent[]
                {
                    new Parent { FirstName = "Thomas" },
                    new Parent { FirstName = "Mary Kay" }
                },
                Children = new Child[]
                {
                    new Child
                    {
                        FirstName = "Henriette Thaulow",
                        Gender = "female",
                        Grade = 5,
                        Pets = new Pet[]
                        {
                            new Pet { GivenName = "Fluffy" }
                        }
                    }
                },
                Address = new Address { State = "WA", County = "King", City = "Seattle" },
                IsRegistered = false
            };

            try
            {
                // Read the item to see if it exists.  
                ItemResponse<Family> andersenFamilyResponse = await this.container.ReadItemAsync<Family>(andersenFamily.Id, new PartitionKey(andersenFamily.PartitionKey));
                logger.LogInformation("Item in database with id: {andersenFamilyResponseResourceId} already exists", andersenFamilyResponse.Resource.Id);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen"
                ItemResponse<Family> andersenFamilyResponse = await this.container.CreateItemAsync<Family>(andersenFamily, new PartitionKey(andersenFamily.PartitionKey));

                // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
                logger.LogError("Created item in database with id: {andersenFamilyResponseResourceId} Operation consumed {andersenFamilyResponseRequestCharge} RUs.\n", andersenFamilyResponse.Resource.Id, andersenFamilyResponse.RequestCharge);
            }

            // Create a family object for the Wakefield family
            Family wakefieldFamily = new Family
            {
                Id = "Wakefield.7",
                PartitionKey = "Wakefield",
                LastName = "Wakefield",
                Parents = new Parent[]
                {
                    new Parent { FamilyName = "Wakefield", FirstName = "Robin" },
                    new Parent { FamilyName = "Miller", FirstName = "Ben" }
                },
                Children = new Child[]
                {
                    new Child
                    {
                        FamilyName = "Merriam",
                        FirstName = "Jesse",
                        Gender = "female",
                        Grade = 8,
                        Pets = new Pet[]
                        {
                            new Pet { GivenName = "Goofy" },
                            new Pet { GivenName = "Shadow" }
                        }
                    },
                    new Child
                    {
                        FamilyName = "Miller",
                        FirstName = "Lisa",
                        Gender = "female",
                        Grade = 1
                    }
                },
                Address = new Address { State = "NY", County = "Manhattan", City = "NY" },
                IsRegistered = true
            };

            try
            {
                // Read the item to see if it exists
                ItemResponse<Family> wakefieldFamilyResponse = await this.container.ReadItemAsync<Family>(wakefieldFamily.Id, new PartitionKey(wakefieldFamily.PartitionKey));
                logger.LogInformation("Item in database with id: {wakefieldFamilyResponseResourceId} already exists", wakefieldFamilyResponse.Resource.Id);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // Create an item in the container representing the Wakefield family. Note we provide the value of the partition key for this item, which is "Wakefield"
                ItemResponse<Family> wakefieldFamilyResponse = await this.container.CreateItemAsync<Family>(wakefieldFamily, new PartitionKey(wakefieldFamily.PartitionKey));

                // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
                logger.LogError("Created item in database with id: {wakefieldFamilyResponseResourceId} Operation consumed {wakefieldFamilyResponseRequestCharge} RUs.", wakefieldFamilyResponse.Resource.Id, wakefieldFamilyResponse.RequestCharge);
            }
        }

        public async Task DeleteDatabaseAndCleanupAsync()
        {
            if (database != null)
            {
                DatabaseResponse databaseResourceResponse = await database.DeleteAsync();
                // Also valid: await this.cosmosClient.Databases["FamilyDatabase"].DeleteAsync();

                logger.LogInformation("Database deleted (id: {databaseId})", options.DatabaseId);
            }
            else
                logger.LogWarning("Database not found where id = {databaseId}", options.DatabaseId);

            //Dispose of CosmosClient
            if (cosmosClient != null)
            {
                cosmosClient.Dispose();
                logger.LogInformation("Client disposed", options.DatabaseId);
            }
            else
                logger.LogWarning("Client already disposed", options.DatabaseId);
        }
    }
}
