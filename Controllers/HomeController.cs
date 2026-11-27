using System.Diagnostics;
using Demo.PL.Models;
using Microsoft.AspNetCore.Mvc;

namespace Demo.PL.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult ServerError()
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            return View("InternalServerError");
        }

        // Use ActionName to avoid hiding ControllerBase.StatusCode(int).
        [ActionName("StatusCode")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult StatusCodePage(int code)
        {
            if (code == StatusCodes.Status404NotFound)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return View("NotFound");
            }

            if (code >= StatusCodes.Status500InternalServerError)
            {
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return View("InternalServerError");
            }

            Response.StatusCode = code;
            return View("NotFound");
        }
    }
}
