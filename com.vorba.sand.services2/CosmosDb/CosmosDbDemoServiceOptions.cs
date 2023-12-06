using Microsoft.Azure.Cosmos;

namespace com.vorba.sand.services2
{
    public class CosmosDbDemoServiceOptions
    {
        public string? ApplicationName { get; set; }
        public string? DatabaseId { get; set; }
        public string? ContainerId { get; set; }
        public string? EndpointUri { get; set; }
        public string? PrimaryKey { get; set; }
        public int? ThroughputValueIncrement { get; set; }
        public CosmosClientOptions? ClientOptions { get; set; }
        public bool CreateIfNotExists { get; set; }
    }
}
