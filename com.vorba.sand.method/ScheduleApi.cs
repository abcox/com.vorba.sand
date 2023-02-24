using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Azure.WebJobs;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Extensions;
using Microsoft.EntityFrameworkCore;

namespace com.vorba.sand.method
{
    public class ScheduleApi
    {
        private readonly ILogger<ScheduleApi> _logger;
        private readonly OpenApiSettings _openapi;

        public ScheduleApi(
            ILogger<ScheduleApi> log,
            OpenApiSettings openapi
            )
        {
            this._logger = log.ThrowIfNullOrDefault();
            this._openapi = openapi.ThrowIfNullOrDefault();
            //this._fixture = fixture.ThrowIfNullOrDefault();
        }

        [FunctionName(nameof(GetSchedule))]
        [OpenApiOperation(operationId: nameof(GetSchedule), tags: new[] { "schedule" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        //[OpenApiParameter(name: "id", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The id of the blog")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "The unique Id of the schedule")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(string), Description = "The blog", Summary = "Success response")]
        public async Task<IActionResult> GetSchedule(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "schedule/{id}")] HttpRequest req, int id)
        {
            //_logger.LogInformation("C# HTTP trigger function processed a request.");

            //int? id = int.TryParse(req.Query["id"], out int result) ? result : null;
            int blogId = id;

            using var db = new BloggingContext();

            // Read
            var blog = await db.Blogs.FirstOrDefaultAsync(b => b.BlogId == id);

            if (blog == null)
            {
                return new BadRequestObjectResult(new { blogId = id });
            }

            return new OkObjectResult(blog);
        }
    }
}
