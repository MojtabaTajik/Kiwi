using Data.Services.Blog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.Entities.Account;
using Model.Entities.Blog;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;

namespace Kiwi.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    public class ManagementController : Controller
    {
        private readonly IBlogService _blog;

        public ManagementController(IBlogService blog, IWebHostEnvironment env)
        {
            _blog = blog;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Drafts()
        {
            var posts = await _blog.GetPosts();
            var drafts = posts.Where(w => w.IsPublished == false);

            return View(drafts);
        }

        public async Task<IActionResult> BlogInfo()
        {
            var blogInfo = await _blog.GetBlogInfo();
            return View(blogInfo);
        }

        public async Task<IActionResult> EditBlogInfo(string id)
        {
            var blogInfo = await _blog.GetBlogInfo(id);

            return View(blogInfo);
        }

        [HttpPost]
        public async Task<IActionResult> EditBlogInfo(BlogInfo blogInfo)
        {
            var files = HttpContext.Request.Form.Files;
            if (files.Any())
            {
                var avatarInfo = files[0];

                string avatarExtension = Path.GetExtension(avatarInfo.FileName);

                await using var ms = new MemoryStream();
                avatarInfo.CopyTo(ms);

                blogInfo.AvatarId = _blog.SaveFile(ms.ToArray(), avatarExtension, blogInfo.AvatarId);
            }

            bool setBlogInfoResult = await _blog.SetBlogInfo(blogInfo);

            return RedirectToAction("BlogInfo");
        }

        public async Task<IActionResult> Backup()
        {
            var dbData = await _blog.BackupDatabase();

            this.HttpContext.Response.Headers.Add("Content-Disposition", "attachment; filename=BlogBackup.db");
            return File(dbData, "text/plain");
        }

        [HttpPost]
        public async Task<IActionResult> Restore(IFormFile databaseFile)
        {
            using (var memoryStream = new MemoryStream())
            {
                databaseFile.CopyTo(memoryStream);
                await _blog.RestoreDatabase(memoryStream.ToArray());
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ExportVisitors()
        {
            var visitorsString = await _blog.ExportVisitors();
            var visitorsData = Encoding.Unicode.GetBytes(visitorsString);

            this.HttpContext.Response.Headers.Add("Content-Disposition", "attachment; filename=Visitors.json");
            return File(visitorsData, "text/plain");
        }

        public async Task<IActionResult> ChangeAdmin()
        {
            var adminInfo = await _blog.GetAdminInfo();
   
            return View(adminInfo);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeAdmin(Login login)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var adminInfo = await _blog.GetAdminInfo();
            if (adminInfo.Password != KiwiCryptography.HashPassword(login.CurrentPassword))
            {
                ModelState.AddModelError(string.Empty, "Current password is incorrect.");
                return View();
            }

            var setResult = await _blog.SetAdminInfo(login);
            return View("Index");
        }
    }
}