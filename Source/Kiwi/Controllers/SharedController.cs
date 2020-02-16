using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace Kiwi.Controllers
{
    public class SharedController : Controller
    {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}