using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportStore.Data;
using SportStore.Models;

namespace SportStore.Controllers
{
    public class MainController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MainController(ApplicationDbContext context)
        {
            _context = context;
        }

        public void GetDataCategories()
        {
            ViewBag.category = _context.Categories.ToList();
        }



        // GET: Main
        public async Task<IActionResult> Index()
        {
            GetDataCategories();
            var applicationDbContext = _context.Products.Include(p => p.Category).Include(p => p.Supplier).Include(p => p.ProductDetails);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> SearchByCaterogy(int? CategoryId)
        {
            GetDataCategories();

            var products = _context.Products
                                   .Include(p => p.Category)
                                   .Include(p => p.Supplier)
                                   .Include(p => p.ProductDetails)
                                    .Where(p => p.CategoryId == CategoryId);



            return View(await products.ToListAsync());
        }

        // GET: Main/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.ProductDetails)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public async Task<IActionResult> ProductDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.ProductDetails)
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            var relatedProducts = await _context.Products
                .Include(p => p.ProductDetails)
                .Where(p => p.CategoryId == product.CategoryId && p.ProductId != id)
                .Take(4)
                .ToListAsync();

            ViewBag.RelatedProducts = relatedProducts;

            return View(product);
        }
        public void GetData()
        {
            ViewBag.product = _context.Products.Include(p => p.ProductDetails).ToList();

        }
        public async Task<IActionResult> Search(string keyword)
        {
            GetData();

            ViewBag.SearchKeyword = keyword;
            var applicationDbContext = _context.Products.Where(p => p.FullName.Contains(keyword) || p.Brand.Contains(keyword));
            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> MyOrders()
        {
            int? customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == 0)
            {
                return RedirectToAction("Login", "Customers");
            }
            var orders = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.Product)
                .Where(i => i.CustomerId == customerId)
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();

            return View(orders);
        }
        public async Task<IActionResult> MyOrderDetails(int id)
        {
            int? customerId = HttpContext.Session.GetInt32("CustomerId");

            if (!customerId.HasValue)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var invoice = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                    .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(i => i.InvoiceId == id && i.CustomerId == customerId.Value);

            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }
    }
}
