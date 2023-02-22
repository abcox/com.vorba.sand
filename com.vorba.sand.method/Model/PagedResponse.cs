using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.vorba.sand.method.Model
{
    internal class PagedResponse<T> : Response<T>
    {
        public PagedResponse(T data, int pageNumber, int pageSize, int totalItems)
        {
            this.Data = data;
            this.Errors = null;
            this.Message = null;
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
            this.Succeeded = true;
            this.TotalItems = totalItems;
            this.TotalPages = totalItems / pageSize;
        }
        public Uri FirstPage { get; set; }
        public Uri LastPage { get; set; }
        public Uri NextPage { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public Uri PreviousPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}
