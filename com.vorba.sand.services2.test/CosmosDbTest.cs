using com.vorba.data.Models;
using com.vorba.sand.services2.CosmosDb;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace com.vorba.sand.services2.test
{
    [TestClass]
    public class CosmosDbTest
    {
        public static readonly string baseAddress = "https://rickandmortyapi.com/api/character";

        public CosmosDbTest()
        {

        }

        public static SocketsHttpHandler socketsHttpHandler => new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(15) // Recreate every 15 minutes
        };
        // HttpClient lifecycle management best practices:
        // https://learn.microsoft.com/dotnet/fundamentals/networking/http/httpclient-guidelines#recommended-use
        private static HttpClient sharedClient = new(socketsHttpHandler)
        {
            //BaseAddress = new Uri(baseAddress), // "https://jsonplaceholder.typicode.com"
        };

        [TestMethod]
        public async Task CreateTextData()
        {
            var handler = new SocketsHttpHandler
            {                
                PooledConnectionLifetime = TimeSpan.FromMinutes(15) // Recreate every 15 minutes
            };
            var name = "Rick";
            var status = "";
            var page = 0;
            var requestUrl = $"{baseAddress}?name={name}&status={status}&page={page + 1}";
            //var sharedClient = new HttpClient(handler);
            List<Character> characters = new List<Character>();
            HttpResponseMessage? response = sharedClient.GetAsync("https://rickandmortyapi.com/api/character?name=Rick").Result;
            if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                CharacterResponse? content = response.Content.ReadFromJsonAsync<CharacterResponse>().Result;
                var info = content?.info;
                var next = info?.next;
                var results = content?.results;
                if (results.Length > 0)
                {
                    characters.AddRange(results);
                }
                for (var i = 0; i < info?.pages && !string.IsNullOrEmpty(next); i++)
                {
                    response = sharedClient.GetAsync(next).Result;
                    if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        content = response.Content.ReadFromJsonAsync<CharacterResponse>().Result;
                        info = content?.info;
                        next = info?.next;
                        results = content?.results;
                        if (results.Length > 0)
                        {
                            characters.AddRange(results);
                        }
                    }
                }
            }

            // PrimaryKey at https://portal.azure.com/#@Vorba.onmicrosoft.com/resource/subscriptions/236217f7-0ad4-4dd6-8553-dc4b574fd2c5/resourceGroups/vorba-sand/providers/Microsoft.DocumentDb/databaseAccounts/vorba1/Connection%20strings
            var logger = new NullLogger<CosmosDbDemoServiceOptions>();
            var options = Options.Create<CosmosDbDemoServiceOptions>(new CosmosDbDemoServiceOptions
            {
                ApplicationName = "CosmosDBDotnetQuickstart",
                ContainerId = "Characters",
                DatabaseId = "RickAndMortyTest2",
                EndpointUri = "https://vorba1.documents.azure.com:443/",
                PrimaryKey = "",
                ThroughputValueIncrement = 100,
                CreateIfNotExists = true,
            });
            //var client = CosmosDbService.GetCosmosClient(options.Value);
            //var result = service.CreateDatabaseAsync("test").Result;
            //await service.DeleteDatabaseAndCleanupAsync();
            var container = await CosmosDbService.GetContainer(options.Value);

            // https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/tutorial-dotnet-bulk-import
            List<Task> tasks = new List<Task>(characters.Count);
            var errors = new Dictionary<int, string>();
            foreach (Character item in characters)
            {
                //var item = characters[0];
                tasks.Add(container.CreateItemAsync(Character.toDto(item), new PartitionKey(item.Id.ToString()))
                    .ContinueWith(itemResponse =>
                    {
                        if (!itemResponse.IsCompletedSuccessfully)
                        {
                            AggregateException innerExceptions = itemResponse.Exception.Flatten();
                            if (innerExceptions.InnerExceptions.FirstOrDefault(innerEx => innerEx is CosmosException) is CosmosException cosmosException)
                            {
                                //Console.WriteLine($"Received {cosmosException.StatusCode} ({cosmosException.Message}).");
                                errors.Add(item.Id, $"Received {cosmosException.StatusCode} ({cosmosException.Message}).");
                            }
                            else
                            {
                                //Console.WriteLine($"Exception {innerExceptions.InnerExceptions.FirstOrDefault()}.");
                                errors.Add(item.Id, $"Exception {innerExceptions.InnerExceptions.FirstOrDefault()}.");
                            }
                        }
                    }));
            }
            // Wait until all are done
            //await Task.WhenAll(tasks);
            await Task.WhenAll(tasks);
        }
    }
}