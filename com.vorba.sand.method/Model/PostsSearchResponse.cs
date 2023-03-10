using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.vorba.sand.method.Model
{
    public class PostsSearchResponse : PagedResponse<List<Post>>
    {
        public PostsSearchResponse(List<Post> data, int? pageNumber, int? pageSize, int totalItems) : base(data, pageNumber, pageSize, totalItems)
        {
        }
    }
}
