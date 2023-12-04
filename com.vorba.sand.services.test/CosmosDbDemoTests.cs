using Castle.Core.Logging;
using com.vorba.sand.services.CosmosDb.Demo;
using com.vorba.sand.services.ServiceBus;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace com.vorba.sand.services.test
{
    [TestClass]
    public class CosmosDbDemoTests
    {
        [TestMethod]
        public async Task ServiceTest()
        {
            var logger = new NullLogger<CosmosDbDemoServiceOptions>();
            var options = Options.Create<CosmosDbDemoServiceOptions>(new CosmosDbDemoServiceOptions
            {
                ApplicationName= "CosmosDBDotnetQuickstart",
                ContainerId = "Items",
                DatabaseId = "ToDoList",
                EndpointUri = "https://vorba1.documents.azure.com:443/",
                PrimaryKey = "JOYD //\\//\\//\\//\\//\\ ****** REDACTED ******* //\\//\\//\\//\\// ******* //\\// XQ==",
                ThroughputValueIncrement = 100,
            });
            var service = new CosmosDbDemoService(logger, options);
            await service.CreateDatabaseAsync("test");
            await service.DeleteDatabaseAndCleanupAsync();
        }
    }
}