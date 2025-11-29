using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportStore.Data;
using SportStore.Models;

namespace SportStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");
            if(role == null)
            {
                return RedirectToAction("Login", "Accounts");
            }
            if(role == "Admin" || role =="Nhân viên")
            {
                ViewBag.Layout = "_LayoutAdmin";

                var invoices = _context.Invoices.Include(i => i.InvoiceDetails).ToList();
                ViewBag.TotalRevenue = invoices
                    .Where(i => i.InvoiceStatus == 1)
                    .Sum(i => i.InvoiceDetails.Sum(d => (d.Quantity ?? 0) * (d.UnitPrice ?? 0)));

                ViewBag.TotalOrders = _context.Invoices.Count();
                ViewBag.TotalCustomers = _context.Customers.Count();
                ViewBag.TotalProducts = _context.Products.Count();

                var RecentInvoices = _context.Invoices
                    .Include(i => i.Customer)
                    .Include(i => i.InvoiceDetails)

                    .OrderByDescending(i => i.InvoiceDate)
                    .Take(4)
                    .ToList();
                ViewBag.RecentInvoices = RecentInvoices;

                ViewBag.TopProducts = _context.Products
                    .Include(p => p.InvoiceDetails)
                    .Select(p => new {
                        p.FullName,
                        TotalSold = p.InvoiceDetails.Sum(d => d.Quantity ?? 0),
                        Revenue = p.InvoiceDetails.Sum(d => (d.Quantity ?? 0) * (d.UnitPrice ?? 0))
                    })
                    .OrderByDescending(p => p.TotalSold)
                    .Take(4)
                    .ToList();
            }
            else
            {
                ViewBag.Layout = "_LayoutCustomer";
            }
            return View();
        }
    }
}
