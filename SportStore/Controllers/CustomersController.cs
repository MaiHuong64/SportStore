using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SportStore.Data;
using SportStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace SportStore.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult ViewCart()
        {
            return View(GetCartItems());
        }

        public List<CartItem> GetCartItems()
        {
            var session = HttpContext.Session;
            string jsoncart = session.GetString("shopcart");
            if (jsoncart != null)
            {
                var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(jsoncart);
                return cartItems ?? new List<CartItem>();
            }
            return new List<CartItem>();
        }
        public void SaveCartSession(List<CartItem> list)
        {
            var session = HttpContext.Session;

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                // Hoặc dùng: PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

            string jsoncart = JsonConvert.SerializeObject(list, settings);
            session.SetString("shopcart", jsoncart);
        }

        void ClearCart()
        {
            var session = HttpContext.Session;
            session.Remove("shopcart");
        }
        public IActionResult ClearAllCart()
        { 
          
            ClearCart();
            TempData["Success"] = "Đã xóa toàn bộ giỏ hàng";
            return RedirectToAction(nameof(ViewCart));
        }
        public async Task<IActionResult> AddToCart(int id)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerID");
            if(customerId == null)
            {
                return RedirectToAction("Login", "Accounts");
            }
            var pd = await _context.ProductDetails
                .Include(pd => pd.Product)
                .FirstOrDefaultAsync(pd => pd.ProductDetailId == id);
            if(pd == null)
            {
                return NotFound("Sản phẩm không tồn tại");
            }
            var cart = GetCartItems();
            var item = cart.Find(c => c.producDetailId == id);

            if (item != null)
            {
                item.quantity++;
            }
            else
            {
                cart.Add(new CartItem() { product = pd.Product, producDetailId = id, price = pd.Price, quantity = 1 });
            }
            SaveCartSession(cart);
            return RedirectToAction(nameof(ViewCart));
        }

        [HttpPost]
        public IActionResult UpdateItem(int productDetailId, int quantity)
        {
            var cart = GetCartItems();
            var item = cart.Find(c => c.producDetailId == productDetailId);

            if (item != null)
            {
                var pd = _context.ProductDetails.Find(productDetailId);
                if (pd != null)
                {
                    if (quantity > 0)
                    {
                        item.quantity = quantity;
                        SaveCartSession(cart);
                        TempData["Success"] = "Đã cập nhật số lượng";
                    }
                    else
                    {
                        TempData["Error"] = "Số lượng phải lớn hơn 0";
                    }
                }
            }
            else
            {
                TempData["Error"] = "Không tìm thấy sản phẩm trong giỏ hàng";
            }

            return RedirectToAction(nameof(ViewCart));
        }

        [HttpPost]
        public IActionResult RemoveItem(int productDetailId)
        {
            var cart = GetCartItems();
            var item = cart.Find(c => c.producDetailId == productDetailId);

            if (item != null)
            {
                cart.Remove(item);
                SaveCartSession(cart);
                TempData["Success"] = "Đã xóa sản phẩm khỏi giỏ hàng";
            }
            else
            {
                TempData["Error"] = "Không tìm thấy sản phẩm trong giỏ hàng";
            }

            return RedirectToAction(nameof(ViewCart));
        }

        public IActionResult Checkout()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerID");
            if (customerId == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để thanh toán";
                return RedirectToAction("Login", "Accounts");
            }
            var cart = GetCartItems();
            if (cart.Count == 0)    
            {
                TempData["Error"] = "Giỏ hàng trống";
                return RedirectToAction("ViewCart", "Customers");
            }
            var customer = _context.Customers.Find(customerId.Value);
            ViewBag.Customer = customer;

            return View(cart);
        }
        public async Task<IActionResult> ProcessCheckout()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerID");
            var maxID = await _context.Invoices.MaxAsync(i => i.InvoiceId);
            var cart = GetCartItems();
            string invCode = "INV" + (maxID + 1).ToString("D3");
            var inv = new Invoice
            {
                InvoiceCode = invCode,
                InvoiceDate = DateOnly.FromDateTime(DateTime.Now),
                InvoiceStatus = 1,
                CustomerId = customerId,
                EmployeeId = null,
            };
            _context.Invoices.Add(inv);
            await _context.SaveChangesAsync();

            foreach (var item in cart)
            {
                var detail = new InvoiceDetail
                {
                    InvoiceId = inv.InvoiceId,
                    ProductId = item.product.ProductId,
                    Quantity = item.quantity,
                    UnitPrice = item.price
                };
                _context.InvoiceDetails.Add(detail);
            }
            await _context.SaveChangesAsync();
            ClearAllCart();
            TempData["Success"] = "Đặt hàng thành công!";
            return RedirectToAction("successNotification");
        }
        public IActionResult successNotification()
        {
            return View();
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Customers.ToListAsync());
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,CustomerCode,FullName,Email,PhoneNumber,Address")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,CustomerCode,FullName,Email,PhoneNumber,Address")] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
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
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
