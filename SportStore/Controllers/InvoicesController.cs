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
    public class InvoicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InvoicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        private void LoadProducts()
        {
            var productData = _context.Products
                .Include(p => p.ProductDetails)
                .ToList();
            var products = productData.Select(p => new
            {
                productId = p.ProductId,
                productCode = p.ProductCode,
                productName = p.FullName,
                price = p.ProductDetails.FirstOrDefault()?.Price ?? 0,
            }).ToList();
            ViewBag.Products = System.Text.Json.JsonSerializer.Serialize(products);
        }

        // GET: Invoices
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Invoices
                .Include(i => i.InvoiceDetails)
                .Include(i => i.Customer)
                .Include(i => i.Employee);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Invoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .Include(i => i.Customer)
                .Include(i => i.Employee)
                .FirstOrDefaultAsync(m => m.InvoiceId == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // GET: Invoices/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.CustomerId = new SelectList(_context.Customers, "CustomerId", "FullName");
            var employeeId = HttpContext.Session.GetInt32("AccountId");
            ViewBag.EmployeeName = HttpContext.Session.GetString("FullName");
            ViewBag.EmployeeId = employeeId;

            var max = await _context.Invoices.MaxAsync(i => i.InvoiceId);
            var newInvCode = "INV" + (max + 1).ToString("D3");
            var model = new Invoice
            {
                InvoiceCode = newInvCode,
                InvoiceDate = DateOnly.FromDateTime(DateTime.Now),
                InvoiceStatus = 0 
            };

            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "FullName");
            var productData = _context.Products
                .Include(p => p.ProductDetails)
                .ToList();

            var product = productData.Select(p => new
            {
                productId = p.ProductId,
                productCode = p.ProductCode,
                productName = p.FullName,
                price = p.ProductDetails.FirstOrDefault()?.Price ?? 0,
            }).Where(p => p.price > 0).ToList();

            ViewBag.Products = System.Text.Json.JsonSerializer.Serialize(product);


            return View(model);
        }

        // POST: Invoices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Invoice inv, List<InvoiceDetail> InvoiceDetails)
        {
            try
            {
                //Console.WriteLine("\n=== INVOICE OBJECT ===");
                //Console.WriteLine($"inv.CustomerId: {inv.CustomerId}");
                //Console.WriteLine($"inv.EmployeeId: {inv.EmployeeId}");
                //Console.WriteLine($"inv.InvoiceCode: {inv.InvoiceCode}");
                //Console.WriteLine($"inv.InvoiceDate: {inv.InvoiceDate}");
                //Console.WriteLine($"inv.InvoiceStatus: {inv.InvoiceStatus}");

                var curEmp = HttpContext.Session.GetInt32("AccountId");
                var empName = HttpContext.Session.GetString("FullName");

                if (!curEmp.HasValue)
                {
                    TempData["ErrorMessage"] = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại.";
                    return RedirectToAction("Login", "Accounts");
                }
                if (InvoiceDetails == null || !InvoiceDetails.Any())
                {
                    ModelState.AddModelError("", "Vui lòng thêm ít nhất sản phẩm");
                    ViewBag.CustomerId = new SelectList(_context.Customers, "CustomerId", "CustomerId", inv.CustomerId);
                    ViewBag.EmployeeId = curEmp.Value;
                    ViewBag.EmployeeName = empName;
                    ViewBag.EmployeeName = HttpContext.Session.GetString("FullName");

                    LoadProducts();
                    return View(inv);
                }

                var invoice = new Invoice
                {
                    InvoiceCode = inv.InvoiceCode,
                    InvoiceDate = inv.InvoiceDate,
                    InvoiceStatus = inv.InvoiceStatus ?? 0,
                    CustomerId = inv.CustomerId,
                    EmployeeId = curEmp.Value,

                    InvoiceDetails = new List<InvoiceDetail>()
                };

                //Console.WriteLine($"invoice.CustomerId = {invoice.CustomerId}");
                //Console.WriteLine($"invoice.EmployeeId = {invoice.EmployeeId}");
                //Console.WriteLine($"invoice.InvoiceCode = {invoice.InvoiceCode}");
                //await _context.SaveChangesAsync();

                foreach (var detail in InvoiceDetails)
                {
                    invoice.InvoiceDetails.Add(new InvoiceDetail
                    {
                        ProductId = detail.ProductId,
                        Quantity = detail.Quantity,
                        UnitPrice = detail.UnitPrice
                    });
                }
                ;

                if (invoice.InvoiceDetails.Count == 0)
                {
                    ModelState.AddModelError("", "Không có sản phẩm hợp lệ trong hóa đơn!");

                    ViewBag.CustomerId = new SelectList(_context.Customers, "CustomerId", "FullName", inv.CustomerId);
                    ViewBag.EmployeeId = new SelectList(_context.Employees, "EmployeeId", "FullName", inv.EmployeeId);
                    LoadProducts();

                    return View(inv);
                }
                _context.Add(invoice);
                await _context.SaveChangesAsync();

                //Console.WriteLine($"invoice.InvoiceId = {invoice.InvoiceId}");
                //Console.WriteLine($"invoice.CustomerId = {invoice.CustomerId}");

                TempData["SuccessMessage"] = "Tạo hóa đơn thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi tạo hóa đơn: " + ex.Message);

                ViewBag.CustomerId = new SelectList(_context.Customers, "CustomerId", "FullName", inv.CustomerId);
                ViewBag.EmployeeId = new SelectList(_context.Employees, "EmployeeId", "FullName", inv.EmployeeId);
                LoadProducts();

                return View(inv);
            }
        }

        // GET: Invoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", invoice.CustomerId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", invoice.EmployeeId);
            return View(invoice);
        }

        // POST: Invoices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InvoiceId,InvoiceCode,InvoiceDate,InvoiceType,CustomerId,EmployeeId")] Invoice invoice)
        {
            if (id != invoice.InvoiceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invoice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvoiceExists(invoice.InvoiceId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", invoice.CustomerId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", invoice.EmployeeId);
            return View(invoice);
        }

        // GET: Invoices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .Include(i => i.Customer)
                .Include(i => i.Employee)
                .FirstOrDefaultAsync(m => m.InvoiceId == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // POST: Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice != null)
            {
                _context.Invoices.Remove(invoice);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InvoiceExists(int id)
        {
            return _context.Invoices.Any(e => e.InvoiceId == id);
        }
    }
}
