using System.IO;
using Data.Services.Blog;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Kiwi.Middlewares
{
    public class ResourceRequestDbRetrieveMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IBlogService _blog;

        public ResourceRequestDbRetrieveMiddleware(RequestDelegate next, IBlogService blog)
        {
            _next = next;
            _blog = blog;
        }

        public async Task Invoke(HttpContext context, IOptionsSnapshot<BlogSettings> settings)
        {
            await _next(context);

            string requestedResourceExtension = Path.GetExtension(context.Request.Path);
            var allowedExtensions = settings.Value.ValidUploadExtensions;

            if (allowedExtensions.Contains(requestedResourceExtension))
            {
                string fileId = context.Request.Path.Value.Split('/').LastOrDefault();

                var fileBytes = _blog.RetrieveFile(fileId);

                await context.Response.Body.WriteAsync(fileBytes);
            }
        }
    }
}