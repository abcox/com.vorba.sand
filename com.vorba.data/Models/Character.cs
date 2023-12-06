using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.vorba.data.Models
{
    // https://products.codeporting.app/convert/ai/typescript-to-csharp/

    [JsonConverter(typeof(IdConverter))]
    public record BaseId(string Value);
    public class Character//: BaseObject
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }
        public string? Species { get; set; }
        public string? Type { get; set; }
        public string? Gender { get; set; }
        public Dictionary<string, string>? Origin { get; set; }
        public Dictionary<string, string>? Location { get; set; }
        public string? Image { get; set; }
        public List<string>? Episode { get; set; }
        public string? Url { get; set; }
        public DateTime? Created { get; set; }

        public static CharacterDto toDto(Character item)
        {
            return new() {
                Id = item.Id.ToString(),
                Name = item.Name,
                Status = item.Status,
                Species = item.Species,
                Type = item.Type,
                Gender = item.Gender,
                Origin = item.Origin,
                Location = item.Location,
                Image = item.Image,
                Episode = item.Episode,
                Url = item.Url,
                Created = item.Created,
            };
        }
    }

    //[JsonConverter(typeof(IdConverter))]
    //public record BaseId(string Value);
    public class IdConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var id = (BaseId)value;
            serializer.Serialize(writer, id.Value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = serializer.Deserialize<string>(reader);
            return new BaseId(value);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(BaseId);
        }
    }

    public class CharacterDto //: Character, IBaseObject<string>
    {
        //public string? Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }
        public string? Species { get; set; }
        public string? Type { get; set; }
        public string? Gender { get; set; }
        public Dictionary<string, string>? Origin { get; set; }
        public Dictionary<string, string>? Location { get; set; }
        public string? Image { get; set; }
        public List<string>? Episode { get; set; }
        public string? Url { get; set; }
        public DateTime? Created { get; set; }
    }

    public interface IBaseObject<T>
    {
        [JsonProperty(PropertyName = "id")]
        public T? Id { get; set; }
    }
}
