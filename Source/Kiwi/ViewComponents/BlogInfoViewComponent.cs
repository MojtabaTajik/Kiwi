using Data.Services.Blog;
using Microsoft.AspNetCore.Mvc;

namespace Kiwi.ViewComponents
{
    public class BlogInfoViewComponent : ViewComponent
    {
        private readonly IBlogService _blogService;

        public BlogInfoViewComponent(IBlogService blog)
        {
            _blogService = blog;
        }

        public IViewComponentResult Invoke(object parameter)
        {
            var blogInfo = _blogService.GetBlogInfo()?.Result;
            return View(blogInfo);
        }
    }
}