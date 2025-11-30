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
        // GET: Main
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Products.Include(p => p.Category).Include(p => p.Supplier).Include(p => p.ProductDetails);
            return View(await applicationDbContext.ToListAsync());
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
        public void GetData()
        {
            ViewBag.product = _context.Products.Include(p => p.ProductDetails).ToList();

        }
        public async Task<IActionResult> Search(string keyword)
        {
            GetData();
            var applicationDbContext = _context.Products.Where(p => p.FullName.Contains(keyword) || p.Brand.Contains(keyword));
            return View(await applicationDbContext.ToListAsync());
        }
        //public async Task<IActionResult> Brand
    }
}
