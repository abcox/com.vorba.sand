using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Reflection.Metadata;
using System.Threading.Tasks;
//using AutoFixture;
using com.vorba.sand.method.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;

namespace com.vorba.sand.method
{
    public class BlogApiFunctions
    {
        private readonly ILogger<BlogApiFunctions> _logger;
        private readonly OpenApiSettings _openapi;
        //private readonly Fixture _fixture;

        public BlogApiFunctions(
            ILogger<BlogApiFunctions> log,
            OpenApiSettings openapi
            //, Fixture fixture
            )
        {
            this._logger = log.ThrowIfNullOrDefault();
            this._openapi = openapi.ThrowIfNullOrDefault();
            //this._fixture = fixture.ThrowIfNullOrDefault();
        }

        [FunctionName(nameof(DeleteBlog))]
        [OpenApiOperation(operationId: nameof(DeleteBlog), tags: new[] { "blog" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "The id of the blog")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(string), Description = "The blog", Summary = "Success response")]
        public async Task<IActionResult> DeleteBlog(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "blog/{id}")] HttpRequest req, int id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            //int? id = int.TryParse(req.Query["id"], out int result) ? result : null;
            int blogId = id;

            using var db = new BloggingContext();

            // Read
            var blog = db.Blogs
                .FirstOrDefault(b => b.BlogId == id);

            if (blog == null)
            {
                return new BadRequestObjectResult(new { blogId = id });
            }

            db.Blogs.Remove(blog);
            await db.SaveChangesAsync();

            return new OkObjectResult(blog);
        }

        [FunctionName(nameof(GetBlog))]
        [OpenApiOperation(operationId: nameof(GetBlog), tags: new[] { "blog" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        //[OpenApiParameter(name: "id", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The id of the blog")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "The id of the blog")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(string), Description = "The blog", Summary = "Success response")]
        public async Task<IActionResult> GetBlog(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "blog/{id}")] HttpRequest req, int id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

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

        [FunctionName(nameof(GetBlogList))]
        [OpenApiOperation(operationId: nameof(GetBlogList), tags: new[] { "blog" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(string), Description = "The blog", Summary = "Success response")]
        public async Task<IActionResult> GetBlogList(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "blogs")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            using var db = new BloggingContext();

            // Read
            var res = await db.Blogs.ToListAsync();

            if (res == null)
            {
                return new EmptyResult();
            }

            return new OkObjectResult(res);
        }

        [FunctionName(nameof(SearchBlogs))]
        [OpenApiOperation(operationId: nameof(SearchBlogs), tags: new[] { "blog" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "q", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "Query search string contained in Url")]
        [OpenApiParameter(name: "pageLimit", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = nameof(PagedRequest.PageSize))]
        [OpenApiParameter(name: "pageStart", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = nameof(PagedRequest.PageNumber))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(List<Post>), Description = "Search results", Summary = "request successful")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = "bad request", Summary = "bad request")]
        public async Task<IActionResult> SearchBlogs(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "blogs/search")] HttpRequest req)
        {
            _logger.LogInformation($"{nameof(SearchBlogs)}");

            IQueryable<Blog> filteredBlogs = null;

            using var db = new BloggingContext();

            filteredBlogs = db.Blogs;

            string q = req.Query["q"];
            if (q != null)
                filteredBlogs = filteredBlogs.Where(x => x.Url.Contains(q));

            //if (filteredBlogs.Count() == 0)
            //    return new BadRequestObjectResult(new { q });

            // paging
            int? pageLimit = int.TryParse(req.Query["pageLimit"], out int pageLimitResult) ? pageLimitResult : null;
            int? pageStart = int.TryParse(req.Query["pageStart"], out int pageStartResult) ? pageStartResult : null;
            PagedRequest pagedRequest = new PagedRequest(limit: pageLimit, start: ++pageStart);
            var pagedData = await pagedRequest.GetDataAsync(filteredBlogs);
            var count = await filteredBlogs.CountAsync();

            var response = new BlogSearchResponse(pagedData, pagedRequest.PageNumber, pagedRequest.PageSize, count);

            return new OkObjectResult(response);
        }

        [FunctionName(nameof(SearchBlogs2))]
        [OpenApiOperation(operationId: nameof(SearchBlogs2), tags: new[] { "blog" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody("request", typeof(BlogSearchRequest), Required = false, Description = "Blog search request")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(BlogSearchResponse), Description = "Search results", Summary = "request successful")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = "bad request", Summary = "bad request")]
        public async Task<IActionResult> SearchBlogs2(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "blogs/search/v2")] HttpRequest req)
        {
            _logger.LogInformation($"{nameof(SearchBlogs)}");

            IQueryable<Blog> filteredBlogs = null;

            using var db = new BloggingContext();

            filteredBlogs = db.Blogs;


            // paging
            //int? pageLimit = int.TryParse(req.Query["pageLimit"], out int pageLimitResult) ? pageLimitResult : null;
            //int? pageStart = int.TryParse(req.Query["pageStart"], out int pageStartResult) ? pageStartResult : null;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonConvert.DeserializeObject<BlogSearchRequest>(requestBody);
            PagedRequest pagedRequest = new PagedRequest(limit: request.PageSize, start: ++request.PageNumber);
            
            var q = request.q.ToString();
            if (!string.IsNullOrEmpty(q))
                filteredBlogs = filteredBlogs.Where(x => x.Url.Contains(q));

            var pagedData = await pagedRequest.GetDataAsync(filteredBlogs);
            var count = await filteredBlogs.CountAsync();

            var response = new BlogSearchResponse(pagedData, pagedRequest.PageNumber, pagedRequest.PageSize, count);

            return new OkObjectResult(response);
        }

        [FunctionName(nameof(CreateBlog))]
        [OpenApiOperation(operationId: nameof(CreateBlog), tags: new[] { "blog" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        //[OpenApiParameter(name: nameof(CreateBlogPostRequest), In = ParameterLocation.Query, Required = true, Type = typeof(CreateBlogPostRequest), Description = nameof(CreateBlogPostRequest))]
        [OpenApiRequestBody(contentType: MediaTypeNames.Application.Json, bodyType: typeof(Blog), Description = "Create blog", Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(Blog), Description = "Blog created", Summary = "Success response")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = "bad request", Summary = "bad request")]
        public async Task<IActionResult> CreateBlog(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "blog")] HttpRequest req)
        {
            _logger.LogInformation($"{nameof(CreateBlog)}");

            //int? id = int.TryParse(req.Query["id"], out int result) ? result : null;

            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            //name = name ?? data?.name;
            //
            //string responseMessage = string.IsNullOrEmpty(name)
            //    ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //    : $"Hello, {name}. This HTTP triggered function executed successfully.";


            //int blogId;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var blog = JsonConvert.DeserializeObject<Blog>(requestBody);
            blog.Posts.Clear(); // remove any posts
            using var db = new BloggingContext();
            var result = db.Blogs.Add(blog);
            db.SaveChanges();

            //blogId = blog.BlogId;
            //string responseMessage = $"blogId: {blogId}";

            //blog.BlogId = (result as Blog).BlogId;

            //string responseMessage = blog.Posts.FirstOrDefault()?.Title;

            //return new OkObjectResult(blog);
            //return await Task.FromResult(new OkObjectResult(this._fixture.Create<Blog>())).ConfigureAwait(false);

            return new OkObjectResult(blog);
        }

        [FunctionName(nameof(UpdateBlog))]
        [OpenApiOperation(operationId: nameof(UpdateBlog), tags: new[] { "blog" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "Id of the blog")]
        [OpenApiRequestBody(contentType: MediaTypeNames.Application.Json, bodyType: typeof(Blog), Description = "Update blog", Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(Blog), Description = "Blog created", Summary = "Success response")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = "bad request", Summary = "bad request")]
        public async Task<IActionResult> UpdateBlog(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "blog/{id}")] HttpRequest req, int id)
        {
            _logger.LogInformation($"{nameof(UpdateBlog)}");

            using var db = new BloggingContext();
            Blog entity = db.Blogs.Find(id);
            if (entity == null)
                return new BadRequestObjectResult(new { id, message = "not found" });

            Blog blog = null;
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                blog = JsonConvert.DeserializeObject<Blog>(requestBody);
                blog.Posts.Clear(); // remove any posts
                blog.BlogId = id;
                //var result = db.Blogs.Update(blog);
                db.Entry(entity).CurrentValues.SetValues(blog);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                var context = new { id, message = ex.Message };
                _logger.LogError(ex, $"{nameof(UpdateBlog)} failed", context);
                return new BadRequestObjectResult(context);
            }

            return new OkObjectResult(blog);
        }

        [FunctionName(nameof(DeletePost))]
        [OpenApiOperation(operationId: nameof(DeletePost), tags: new[] { "post" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "The id of the post")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(string), Description = "The post", Summary = "Success response")]
        public async Task<IActionResult> DeletePost(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "post/{id}")] HttpRequest req, int id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            using var db = new BloggingContext();

            var post = db.Posts
                .FirstOrDefault(x => x.PostId == id);

            if (post == null)
            {
                return new BadRequestObjectResult(new { postId = id });
            }

            db.Posts.Remove(post);
            await db.SaveChangesAsync();

            return new OkObjectResult(post);
        }

        [FunctionName(nameof(GetPost))]
        [OpenApiOperation(operationId: nameof(GetPost), tags: new[] { "post" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "The id of post")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> GetPost(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "post/{id}")] HttpRequest req, int id)
        {
            int postId = id;

            using var db = new BloggingContext();

            // Read
            var post =await db.Posts
                .FirstOrDefaultAsync(b => b.PostId == postId);

            if (post == null)
            {
                return new BadRequestObjectResult(new { postId });
            }

            return new OkObjectResult(post);
        }

        [FunctionName(nameof(GetPosts))]
        [OpenApiOperation(operationId: nameof(GetPosts), tags: new[] { "blog" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "The id of blog")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> GetPosts(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "blog/{id}/posts")] HttpRequest req, int id)
        {
            using var db = new BloggingContext();

            // Read
            var res = await db.Posts.Where(b => b.BlogId == id).ToListAsync();

            if (res == null)
            {
                return new BadRequestObjectResult(new { id });
            }

            return new OkObjectResult(res);
        }

        [FunctionName(nameof(SearchPosts))]
        [OpenApiOperation(operationId: nameof(SearchPosts), tags: new[] { "blog" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "The Id of blog")]
        [OpenApiParameter(name: "q", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "Query search string contained in Url")]
        [OpenApiParameter(name: "pageLimit", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = nameof(PagedRequest.PageSize))]
        [OpenApiParameter(name: "pageStart", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = nameof(PagedRequest.PageNumber))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(List<Post>), Description = "Search results", Summary = "request successful")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = "bad request", Summary = "bad request")]
        public async Task<IActionResult> SearchPosts(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "blog/{id}/posts/search")] HttpRequest req, int id)
        {
            _logger.LogInformation($"{nameof(SearchPosts)}");

            IQueryable<Post> filteredPosts = null;

            using var db = new BloggingContext();

            filteredPosts = db.Posts.Where(b => b.BlogId == id);

            string q = req.Query["q"].FirstOrDefault()?.ToLower();
            if (q != null)
                filteredPosts = filteredPosts
                    .Where(x => x.Content.ToLower().Contains(q) || x.Title.ToLower().Contains(q));

            // paging
            int? pageLimit = int.TryParse(req.Query["pageLimit"], out int pageLimitResult) ? pageLimitResult : null;
            int? pageStart = int.TryParse(req.Query["pageStart"], out int pageStartResult) ? pageStartResult : null;
            PagedRequest pagedRequest = new PagedRequest(limit: pageLimit, start: ++pageStart);
            var pagedData = await pagedRequest.GetDataAsync(filteredPosts);
            var count = await filteredPosts.CountAsync();

            var response = new PostsSearchResponse(pagedData, pagedRequest.PageNumber, pagedRequest.PageSize, count);

            return new OkObjectResult(response);
        }

        [FunctionName(nameof(SearchPosts2))]
        [OpenApiOperation(operationId: nameof(SearchPosts2), tags: new[] { "post" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody("request", typeof(PostsSearchRequest), Required = false, Description = "Posts search request")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(PostsSearchResponse), Description = "Search results", Summary = "request successful")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = "bad request", Summary = "bad request")]
        public async Task<IActionResult> SearchPosts2(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "post/search/v2")] HttpRequest req)
        {
            _logger.LogInformation($"{nameof(SearchPosts2)}");

            IQueryable<Post> filteredPosts = null;

            using var db = new BloggingContext();

            filteredPosts = db.Posts;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonConvert.DeserializeObject<PostsSearchRequest>(requestBody);
            PagedRequest pagedRequest = new PagedRequest(limit: request.PageSize, start: ++request.PageNumber);

            var blogId = request.blogId;
            if (blogId != null)
                filteredPosts = filteredPosts.Where(x => x.BlogId == blogId);

            var q = request.q?.ToString();
            if (!string.IsNullOrEmpty(q))
                filteredPosts = filteredPosts
                    .Where(x => x.Content.ToLower().Contains(q) || x.Title.ToLower().Contains(q));

            var pagedData = await pagedRequest.GetDataAsync(filteredPosts);
            var count = await filteredPosts.CountAsync();

            var response = new PostsSearchResponse(pagedData, pagedRequest.PageNumber, pagedRequest.PageSize, count);

            return new OkObjectResult(response);
        }

        [FunctionName(nameof(CreatePost))]
        [OpenApiOperation(operationId: nameof(CreatePost), tags: new[] { "post" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: MediaTypeNames.Application.Json, bodyType: typeof(Post), Description = "Create blog post", Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(Post), Description = "Post created", Summary = "request successful")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = "bad request", Summary = "bad request")]
        public async Task<IActionResult> CreatePost(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "post")] HttpRequest req)
        {
            _logger.LogInformation($"{nameof(CreatePost)}");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Post post = JsonConvert.DeserializeObject<Post>(requestBody);
            using var db = new BloggingContext();
            var blog = db.Blogs.Find(post?.BlogId);

            if (blog == null)
                return new BadRequestObjectResult(new { post.BlogId });

            try
            {
                post.Blog = null;
                db.Posts.Add(post);
                db.SaveChanges();
                post.Blog = null; // resolve exception -> Newtonsoft.Json: Self referencing loop detected with type 'Post'. Path 'blog.posts'.

                return new OkObjectResult(post);
            }
            catch (Exception ex)
            {
                _logger.LogError("ERROR", ex);
                throw;
            }
        }

        [FunctionName(nameof(SearchPost))]
        [OpenApiOperation(operationId: nameof(SearchPost), tags: new[] { "post" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "title", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "Title of the post")]
        [OpenApiParameter(name: "blogId", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "Blog Id of the post")]
        [OpenApiParameter(name: "pageLimit", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = nameof(PagedRequest.PageSize))]
        [OpenApiParameter(name: "pageStart", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = nameof(PagedRequest.PageNumber))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(List<Post>), Description = "Search results", Summary = "request successful")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = "bad request", Summary = "bad request")]
        public async Task<IActionResult> SearchPost(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "post/search")] HttpRequest req)
        {
            _logger.LogInformation($"{nameof(SearchPost)}");

            using var db = new BloggingContext();

            IQueryable<Post> filteredPosts = db.Posts;           

            int? blogId = int.TryParse(req.Query["blogId"], out int blogIdResult) ? blogIdResult : null;
            if (blogId != null)
                filteredPosts = filteredPosts.Where(x => x.BlogId == blogId);

            string title = req.Query["title"];
            if (title != null)
                filteredPosts = filteredPosts.Where(x => x.Title.Contains(title));

            //if (filteredPosts.Count() == 0)
            //    return new BadRequestObjectResult(new { title });

            // paging
            int? pageLimit = int.TryParse(req.Query["pageLimit"], out int pageLimitResult) ? pageLimitResult : null;
            int? pageStart = int.TryParse(req.Query["pageStart"], out int pageStartResult) ? pageStartResult : null;
            PagedRequest pagedRequest = new PagedRequest(limit: pageLimit, start: pageStart);
            var pagedData = await pagedRequest.GetDataAsync(filteredPosts);
            var count = await filteredPosts.CountAsync();

            var response = new PostsSearchResponse(pagedData, pagedRequest.PageNumber, pagedRequest.PageSize, count);
            return new OkObjectResult(response);
        }

        [FunctionName(nameof(UpdatePost))]
        [OpenApiOperation(operationId: nameof(UpdatePost), tags: new[] { "post" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "The id of the post")]
        [OpenApiRequestBody(contentType: MediaTypeNames.Application.Json, bodyType: typeof(Post), Description = "Updated post", Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(Post), Description = "Blog created", Summary = "Success response")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = "bad request", Summary = "bad request")]
        public async Task<IActionResult> UpdatePost(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "post/{id}")] HttpRequest req, int id)
        {
            _logger.LogInformation($"{nameof(UpdateBlog)}");

            using var db = new BloggingContext();
            var entity = db.Posts.Find(id);
            if (entity == null)
                return new BadRequestObjectResult(new { id, message = "not found" });

            Post post = null;
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                post = JsonConvert.DeserializeObject<Post>(requestBody);
                post.PostId = id;
                //post.Blog = entity.Blog;
                post.BlogId = entity.BlogId;
                db.Entry(entity).CurrentValues.SetValues(post);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                var context = new { id, message = ex.InnerException.Message };
                _logger.LogError(ex, $"{nameof(UpdatePost)} failed", context);
                return new BadRequestObjectResult(context);
            }

            return new OkObjectResult(post);
        }
    }
}

