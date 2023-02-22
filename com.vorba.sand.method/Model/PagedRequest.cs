using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.vorba.sand.method.Model
{
    public class PagedRequest
    {
        public static readonly int DefaultPageStart = 1;
        public static readonly int DefaultPageLimit = 10;
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public PagedRequest()
        {
            this.PageNumber = DefaultPageStart;
            this.PageSize = DefaultPageLimit;
        }
        public PagedRequest(int? limit, int? start)
        {
            this.PageNumber = (start ?? DefaultPageStart) < DefaultPageStart ? DefaultPageStart : start ?? DefaultPageStart;
            this.PageSize = (limit ?? DefaultPageLimit) < DefaultPageStart ? DefaultPageStart : limit ?? DefaultPageLimit;
        }
        //public PagedRequest(int pageNumber, int pageSize)
        //{
        //    this.PageNumber = pageNumber < 1 ? 1 : pageNumber;
        //    this.PageSize = pageSize > 10 ? 10 : pageSize;
        //}
    }
}
