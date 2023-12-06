using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.vorba.sand.services2.CosmosDb.Models
{
    public class BaseResponse
    {
        public BaseResponse() { }

        public BaseResponse(string message) { }

        public string Message { get; set; }

        public Status Status { get; set; }

        public dynamic Data { get; set; }

        public int Count { get; set; }

    }
        public enum Status
        {
        Failed,
        Incomplete,
        NotFound,
        Succeeded,
    }
}
