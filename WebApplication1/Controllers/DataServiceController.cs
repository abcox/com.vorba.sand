using com.vorba.data.Models;
using com.vorba.sand.services2;
using com.vorba.sand.services2.CosmosDb;
using com.vorba.sand.services2.CosmosDb.Models;
using com.vorba.sand.services2.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    //[Route("api/[controller]")]
    [Route("api")]
    [ApiController]
    public class DataServiceController : ControllerBase
    {
        private readonly ICosmosDbService _cosmosDbService;
        private readonly IQueueServices? _queueServices;

        public DataServiceController(ICosmosDbService cosmosDbService, IQueueServices queueServices)
        {
            _cosmosDbService = cosmosDbService;
            _queueServices = queueServices;
        }

        // NOTE: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling?view=aspnetcore-8.0
        // https://learn.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger?view=aspnetcore-8.0

        //[HttpGet("DataService/database/create")]
        ////[OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "Name (id) of database")]
        //
        ////[ProducesResponseType<BaseResponse>(StatusCodes.Status200OK)]
        //public async Task<IActionResult> CreateDatabase([FromQuery] string id)
        //{
        //    return new OkObjectResult(await _cosmosDbService.CreateDatabaseAsync(id));
        //}
        //
        //[HttpGet("DataService/database/{databaseId}/delete")]
        ////[OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "Name (id) of database")]
        //
        ////[ProducesResponseType<BaseResponse>(StatusCodes.Status200OK)]
        //public async Task<IActionResult> DeleteDatabase(string databaseId)
        //{
        //    var result = await _cosmosDbService.DeleteDatabaseAsync(databaseId);
        //    return result.Status == Status.Succeeded ?
        //        new OkObjectResult(result) :
        //        new BadRequestObjectResult(result);
        //}
        //
        //[HttpPost("DataService/database/{databaseId}/container/create")]
        ////[OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "Name (id) of database")]
        //
        ////[ProducesResponseType<BaseResponse>(StatusCodes.Status200OK)]
        //public async Task<IActionResult> CreateDatabaseContainer([FromQuery] string containedId, [FromQuery] string partitionKeyPath, [FromRoute] string databaseId)
        //{
        //    var result = await _cosmosDbService.CreateDatabaseContainerAsync(databaseId, containedId, partitionKeyPath);
        //    return result.Status == Status.Succeeded ?
        //        new OkObjectResult(result) :
        //        new BadRequestObjectResult(result);
        //}
        //
        //[HttpGet("DataService/database/list")]
        ////[OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "Name (id) of database")]
        //
        ////[ProducesResponseType<BaseResponse>(StatusCodes.Status200OK)]
        //public async Task<IActionResult> GetDatabaseList()
        //{
        //    var result = await _cosmosDbService.GetDatabaseListAsync();
        //    return result.Status == Status.Succeeded ?
        //        new OkObjectResult(result) :
        //        new BadRequestObjectResult(result);
        //}


        #region Character
        [SwaggerOperation(Tags = new[] { "Character" })]
        [HttpGet("Character/{id}")]
        public async Task<IActionResult> ReadCharacter(string id)
        {
            var result = await _cosmosDbService.ReadCharacterAsync(id);
            return result.Status == Status.Succeeded ?
                new OkObjectResult(result) :
                new BadRequestObjectResult(result);
        }

        [SwaggerOperation(Tags = new[] { "Character" })]
        [HttpGet("Character")]
        public async Task<IActionResult> ReadCharacters([FromQuery] CharacterFilter characterFilter)
        {
            var result = await _cosmosDbService.ReadCharacterAsync(characterFilter);
            return result.Status == Status.Succeeded ?
                new OkObjectResult(result) :
                new BadRequestObjectResult(result);
        }

        //[ApiExplorerSettings(GroupName ="Test")]
        //[ActionName(nameof(Character))]
        [SwaggerOperation(Tags = new[] { "Character" })]
        //[Route("Character/add")]
        [HttpPost("Character")]
        //[OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "Name (id) of database")]

        //[ProducesResponseType<BaseResponse>(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateCharacter([FromBody] Character character)
        {
            var result = await _cosmosDbService.CreateCharacterAsync(character);
            return result.Status == Status.Succeeded ?
                new OkObjectResult(result) :
                new BadRequestObjectResult(result);
        }

        [SwaggerOperation(Tags = new[] { "Character" })]
        [HttpDelete("Character/{id}")]
        public async Task<IActionResult> DeleteCharacter(string id)
        {
            var result = await _cosmosDbService.DeleteCharacterAsync(id);
            return result.Status == Status.Succeeded ?
                new OkObjectResult(result) :
                new BadRequestObjectResult(result);
        }

        [SwaggerOperation(Tags = new[] { "Character" })]
        [HttpPut("Character/{id}")]
        public async Task<IActionResult> UpdateCharacter([FromBody] Character character)
        {
            var result = await _cosmosDbService.UpdateCharacterAsync(character);
            return result.Status == Status.Succeeded ?
                new OkObjectResult(result) :
                new BadRequestObjectResult(result);
        }
        #endregion Character



        //[SwaggerOperation(Tags = new[] { "Admin" })]
        //[HttpGet("admin/config")]
        //public async Task<IActionResult> GetAppSettings()
        //{
        //    return new OkObjectResult(_cosmosDbDemoServiceOptions);
        //}



        [SwaggerOperation(Tags = new[] { "ServiceBusDemo" })]
        [HttpPost("ServiceBusDemo")]
        public async Task<IActionResult> PostServiceBusMessage([FromQuery] string queueName, [FromBody] SampleMessageModel messageContent)
        {
            if (_queueServices == null) throw new ArgumentNullException(nameof(QueueServices));
            //var messageObject = new SampleMessageModel { SomeStringProperty = messageContent };
            await _queueServices.SendMessageAsync(messageContent, queueName);
            return new OkResult();
        }




        // Scaffolded CRUD:

        // GET: api/<DataServiceController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}
        //
        //// GET api/<DataServiceController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}
        //
        //// POST api/<DataServiceController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}
        //
        //// PUT api/<DataServiceController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}
        //
        //// DELETE api/<DataServiceController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
