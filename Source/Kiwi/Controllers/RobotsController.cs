using System;
using Data.Services.Blog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Model.Entities.Blog;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Kiwi.Infrastructure.Sitemap;

namespace Kiwi.Controllers
{
    public class RobotsController : Controller
    {
        private readonly IBlogService _blog;
        private readonly IOptionsSnapshot<BlogSettings> _settings;

        public RobotsController(IBlogService blog, IOptionsSnapshot<BlogSettings> settings)
        {
            _blog = blog;
            _settings = settings;
        }

        [Route("/robots.txt")]
        [OutputCache(Profile = "default")]
        public string RobotsTxt()
        {
            string host = Request.Scheme + "://" + Request.Host;
            var sb = new StringBuilder();
            sb.AppendLine("User-agent: *");
            sb.AppendLine("Disallow:");
            sb.AppendLine($"sitemap: {host}/sitemap.xml");

            return sb.ToString();
        }

        [Route("/sitemap.xml")]
        public async Task<ActionResult> SitemapAsync()
        {
            string baseUrl = $"{Request.Scheme}://{Request.Host}";

            var siteMapBuilder = new SitemapBuilder();

            // add the home page
            siteMapBuilder.AddUrl(baseUrl, modified: DateTime.UtcNow, changeFrequency: ChangeFrequency.Weekly, priority: 1.0);

            // add blog posts
            var posts = await _blog.GetPosts();
            foreach (var post in posts)
            {
                string postUrl = $"{baseUrl}/blog/{post.ID}";
                siteMapBuilder.AddUrl(postUrl, modified: post.LastModified, changeFrequency: null, priority: 0.9);
            }

            return Content(siteMapBuilder.ToString(), "text/xml");
        }
    }
}
