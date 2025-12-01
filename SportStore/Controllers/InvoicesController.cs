using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportStore.Data;
using SportStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                .ThenInclude(id => id.Product)
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

            var invoice = await _context.Invoices
                .Include(d => d.InvoiceDetails)
                .ThenInclude (d => d.Product)
                .Include (i => i.Customer)
                .Include (i => i.Employee)
                .FirstOrDefaultAsync(m => m.InvoiceId == id);

           
            if (invoice == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", invoice.CustomerId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", invoice.EmployeeId);

            ViewBag.InvoiceStatusList = new SelectList(new[]
           {
                new { Value = 0, Text = "🕐 Chờ xử lý" },
                new { Value = 1, Text = "✓ Đã xác nhận" },
                new { Value = 2, Text = "⟳ Đang xử lý" },
                new { Value = 3, Text = "🚚 Đang giao hàng" },
                new { Value = 4, Text = "✓ Hoàn thành" },
                new { Value = 5, Text = "✕ Đã hủy" }
            }, "Value", "Text", (int)invoice.InvoiceStatus);

            return View(invoice);
        }

        // POST: Invoices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Invoice invoice,List<InvoiceDetail> invoiceDetails)
        {
            if (id != invoice.InvoiceId)
            {
                return NotFound();
            }

            ModelState.Remove("InvoiceDetails");
            ModelState.Remove("Customer");
            ModelState.Remove("Employee");
            ModelState.Remove("InvoiceStatus");
            ModelState.Remove("inv.InvoiceStatus");

            if (!ModelState.IsValid)
            {
                ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "FullName", invoice.CustomerId);
                ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FullName", invoice.EmployeeId);
                ViewBag.InvoiceStatusList = new SelectList(new[]
                {
            new { Value = 0, Text = "🕐 Chờ xử lý" },
            new { Value = 1, Text = "✓ Đã xác nhận" },
            new { Value = 2, Text = "⟳ Đang xử lý" },
            new { Value = 3, Text = "🚚 Đang giao hàng" },
            new { Value = 4, Text = "✓ Hoàn thành" },
            new { Value = 5, Text = "✕ Đã hủy" }
                 }, "Value", "Text", invoice.InvoiceStatus);

                return View(invoice);
            }

            try
            {
                var existingInvoice = await _context.Invoices
                    .Include(i => i.InvoiceDetails)
                    .ThenInclude(d => d.Product)
                    .FirstOrDefaultAsync(i => i.InvoiceId == id);

                if (existingInvoice == null)
                    return NotFound();

                System.Diagnostics.Debug.WriteLine($"Status CŨ: {existingInvoice.InvoiceStatus}");
                System.Diagnostics.Debug.WriteLine($"Status MỚI: {invoice.InvoiceStatus}");

                existingInvoice.InvoiceDate = invoice.InvoiceDate;
                existingInvoice.InvoiceStatus = invoice.InvoiceStatus ?? existingInvoice.InvoiceStatus;

                System.Diagnostics.Debug.WriteLine($"Status SAU KHI GÁN: {existingInvoice.InvoiceStatus}");
                foreach (var detail in invoiceDetails)
                {
                    var exist = existingInvoice.InvoiceDetails
                        .FirstOrDefault(d => d.InvoiceDetailId == detail.InvoiceDetailId);
                    if (exist != null)
                    {
                        exist.ProductId = detail.ProductId;
                        exist.Quantity = detail.Quantity;
                        exist.UnitPrice = detail.UnitPrice;
                    }
                    else
                    {
                        existingInvoice.InvoiceDetails.Add(new InvoiceDetail
                        {
                            ProductId = detail.ProductId,
                            Quantity = detail.Quantity,
                            UnitPrice = detail.UnitPrice
                        });
                    }
                }
                var detailsToRemove = existingInvoice.InvoiceDetails
                    .Where(d => !invoiceDetails.Any(f => f.InvoiceDetailId == d.InvoiceDetailId))
                    .ToList();
                _context.InvoiceDetails.RemoveRange(detailsToRemove);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật hóa đơn thành công!";
                return RedirectToAction(nameof(Details), new { id = invoice.InvoiceId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi cập nhật hóa đơn: " + ex.Message);
                ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "FullName", invoice.CustomerId);
                ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FullName", invoice.EmployeeId);
                ViewBag.InvoiceStatusList = new SelectList(new[]
                 {
                        new { Value = 0, Text = "🕐 Chờ xử lý" },
                        new { Value = 1, Text = "✓ Đã xác nhận" },
                        new { Value = 2, Text = "⟳ Đang xử lý" },
                        new { Value = 3, Text = "🚚 Đang giao hàng" },
                        new { Value = 4, Text = "✓ Hoàn thành" },
                        new { Value = 5, Text = "✕ Đã hủy" }
                    }, "Value", "Text", invoice.InvoiceStatus);
                LoadProducts();
                return View(invoice);
            }
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
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);
            if (invoice != null)
            {
                _context.InvoiceDetails.RemoveRange(invoice.InvoiceDetails);
                _context.Invoices.Remove(invoice);
            }

            TempData["Success"] = "Xóa hóa đơn thành công.";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InvoiceExists(int id)
        {
            return _context.Invoices.Any(e => e.InvoiceId == id);
        }
    }
}
