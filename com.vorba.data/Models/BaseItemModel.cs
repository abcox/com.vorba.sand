using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.vorba.data.Models
{
    public class BaseItemModel
    {
        //public BaseItemModel(dynamic data, string? id = null, string partitionKey = "partitionKey")
        //{
        //    Id = id ?? Guid.NewGuid().ToString();
        //    PartitionKey = partitionKey;
        //    Data = data;
        //}
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }
        [JsonProperty(PropertyName = "partitionKey")]
        public string? PartitionKey { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
