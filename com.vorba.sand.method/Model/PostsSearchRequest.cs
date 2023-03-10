using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.vorba.sand.method.Model
{
    public class PostsSearchRequest : PagedRequest
    {
        public int? blogId { get; set; }
        public string? q { get; set; }
    }
}
