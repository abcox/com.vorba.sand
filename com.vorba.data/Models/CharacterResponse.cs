using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.vorba.data.Models
{
    public class CharacterResponse
    {
        public Character[]? results { get; set; }
        public ResponseMeta? info { get; set; }
    }
    public class ResponseMeta
    {
       public int count { get; set; }
       public int pages { get; set; }
       public string? next  { get; set; }
       public string? prev  { get; set; }
    }
}
