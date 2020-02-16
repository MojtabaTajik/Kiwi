using Data.Services.Blog;
using Microsoft.AspNetCore.Http;
using Model.Entities.Blog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kiwi.Middlewares
{
    public class VisitorInfoLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IBlogService _blog;

        public VisitorInfoLogMiddleware(RequestDelegate next,
            IBlogService blog)
        {
            _next = next;
            _blog = blog;
        }

        public async Task Invoke(HttpContext context)
        {
            // Log user IP & Agent
            var userAgent = context.Request.Headers.SingleOrDefault(h => h.Key == "User-Agent").Value.First();
            var ipAddress = context.Connection.RemoteIpAddress.MapToIPv4().ToString();

            await _blog.LogVisitor(new VisitorInfo
            {
                ID = ipAddress,
                LastVisit = DateTime.UtcNow,
                UserAgent = userAgent,
            });

            await _next(context);
        }
    }
}