using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.vorba.sand.method.Model
{
    public class Response<T>
    {
        public Response() { }
        public Response(T data)
        {
            Data = data;
            Errors = null;
            Message = string.Empty;
            Succeeded = true;
        }
        public T Data { get; set; }
        public string[] Errors { get; set; }
        public string Message { get; set; }
        public bool Succeeded { get; set; }
    }
}
