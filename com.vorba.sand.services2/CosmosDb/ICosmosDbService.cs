using com.vorba.data.Interfaces;
using com.vorba.data.Models;
using com.vorba.sand.services2.CosmosDb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.vorba.sand.services2.CosmosDb
{
    public interface ICosmosDbService
    {
        Task<BaseResponse> CreateDatabaseAsync(string id);
        Task<BaseResponse> DeleteDatabaseAsync(string id);
        Task<BaseResponse> CreateDatabaseContainerAsync(string databaseId, string containerId, string partitionKeyPath);
        Task<BaseResponse> GetDatabaseListAsync();

        Task<BaseResponse> ReadCharacterAsync(string id);
        Task<BaseResponse> CreateCharacterAsync(Character character);
        Task<BaseResponse> UpdateCharacterAsync(Character character);
        Task<BaseResponse> DeleteCharacterAsync(string id);
    }
}
