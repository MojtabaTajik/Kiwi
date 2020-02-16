using Data.Services.Blog;
using Microsoft.AspNetCore.Mvc;

namespace Kiwi.ViewComponents
{
    public class SidebarViewComponent : ViewComponent
    {
        private readonly IBlogService _blogService;

        public SidebarViewComponent(IBlogService blog)
        {
            _blogService = blog;
        }

        public IViewComponentResult Invoke(object parameter)
        {
            var categories = _blogService.GetCategories()?.Result;
            return View(categories);
        }
    }
}