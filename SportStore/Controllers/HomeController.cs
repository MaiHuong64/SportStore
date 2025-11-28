using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SportStore.Models;

namespace SportStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");
            if(role == null)
            {
                return RedirectToAction("Login", "Accounts");
                //ViewBag.Layout = "_LayoutCutomer";
            }
            if(role == "Admin" || role =="Nhân viên")
            {
                ViewBag.Layout = "_Layout";
            }
            else
            {
                ViewBag.Layout = "_LayoutCutomer";
            }
                return View();
        }
    }
}
