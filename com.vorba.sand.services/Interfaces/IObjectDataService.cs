using Microsoft.Azure.Cosmos;

namespace com.vorba.sand.services.Interfaces
{
    public interface IObjectDataService
    {
        Task<DatabaseResponse> CreateDatabaseAsync(string databaseName);
    }
}
