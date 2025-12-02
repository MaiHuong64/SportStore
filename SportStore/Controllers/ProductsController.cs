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
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }
      
        public string? Upload(IFormFile file)
        {
            string? uploadFileName = null;
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName);
                var shortGuid = Guid.NewGuid().ToString("N").Substring(0, 8);
                uploadFileName = shortGuid + extension;
                var path = $"wwwroot\\images\\{uploadFileName}";
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
            return uploadFileName;
        }

        public void GetData()
        {
            ViewBag.CategoryId = new SelectList(_context.Categories.ToList(), "CategoryId", "FullName");
            ViewBag.SupplierId = new SelectList(_context.Suppliers.ToList(), "SupplierId", "SupplierName");
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.ProductDetails);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            GetData();
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

        // GET: Products/Create
        public IActionResult Create()

        {
            GetData();
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        public class ProductVariant
        {
            public byte? Size { get; set; }
            public string? Color { get; set; }
            public decimal Price { get; set; }
            public int? Quantity { get; set; }
            public string? SKU { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            IFormFile Img,
            [Bind("ProductCode,FullName,Description,Brand,CategoryId,SupplierId")] Product product,
            List<ProductVariant> Variants)
        {
            GetData();

            ModelState.Remove("Category");
            ModelState.Remove("Supplier");
            ModelState.Remove("ProductDetails");
            ModelState.Remove("InvoiceDetails");
            ModelState.Remove("Img");

            if (Variants == null || !Variants.Any())
            {
                ModelState.AddModelError("", "Vui lòng thêm ít nhất một biến thể sản phẩm");
            }
            else if (!Variants.Any(v => v.Price > 0))
            {
                ModelState.AddModelError("", "Vui lòng nhập giá cho ít nhất một biến thể");
            }

            if (ModelState.IsValid)
            {
                product.Img = Upload(Img);
                _context.Add(product);
                await _context.SaveChangesAsync();

                foreach (var variant in Variants)
                {
                    if (variant.Price > 0)
                    {
                        var productDetail = new ProductDetail
                        {
                            ProductId = product.ProductId,
                            Price = variant.Price,
                            Quantity = variant.Quantity ?? 0,
                            Size = variant.Size,
                            Color = variant.Color,
                            Sku = string.IsNullOrEmpty(variant.SKU)
                                ? $"{product.ProductCode}-{variant.Size}-{variant.Color?.Replace(" ", "")}"
                                : variant.SKU
                        };

                        _context.ProductDetails.Add(productDetail);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SubmittedVariants = Variants;
            return View(product);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            GetData();
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductDetails)
                .FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            IFormFile? ImgFile,
            [Bind("ProductId,ProductCode,FullName,Description,Brand,CategoryId,SupplierId")] Product product,
            List<ProductDetail> ProductDetails)
        {
            GetData();
            if (id != product.ProductId)
            {
                return NotFound();
            }

            ModelState.Remove("Category");
            ModelState.Remove("Supplier");
            ModelState.Remove("ProductDetails");
            ModelState.Remove("InvoiceDetails");

            if (ProductDetails != null)
            {
                for (int i = 0; i < ProductDetails.Count; i++)
                {
                    ModelState.Remove($"ProductDetails[{i}].Product");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == id);
                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    if (ImgFile != null && ImgFile.Length > 0)
                    {
                        product.Img = Upload(ImgFile);
                    }
                    else
                    {
                        product.Img = existingProduct.Img;
                    }

                    _context.Update(product);

                    var existingDetails = await _context.ProductDetails
                        .Where(pd => pd.ProductId == id)
                        .ToListAsync();

                    foreach (var existingDetail in existingDetails)
                    {
                        var matchingDetail = ProductDetails?.FirstOrDefault(pd => pd.ProductDetailId == existingDetail.ProductDetailId);
                        if (matchingDetail == null)
                        {
                            _context.ProductDetails.Remove(existingDetail);
                        }
                    }

                    if (ProductDetails != null && ProductDetails.Any())
                    {
                        foreach (var detail in ProductDetails)
                        {
                            if (detail.ProductDetailId > 0)
                            {
                                var existingDetail = existingDetails.FirstOrDefault(ed => ed.ProductDetailId == detail.ProductDetailId);
                                if (existingDetail != null)
                                {
                                    existingDetail.Size = detail.Size;
                                    existingDetail.Color = detail.Color;
                                    existingDetail.Price = detail.Price;
                                    existingDetail.Quantity = detail.Quantity;
                                    existingDetail.Sku = string.IsNullOrEmpty(detail.Sku)
                                        ? $"{product.ProductCode}-{detail.Size}-{detail.Color?.Replace(" ", "")}"
                                        : detail.Sku;
                                    _context.ProductDetails.Update(existingDetail);
                                }
                            }
                            else
                            {
                                var newDetail = new ProductDetail
                                {
                                    ProductId = id,
                                    Size = detail.Size,
                                    Color = detail.Color,
                                    Price = detail.Price,
                                    Quantity = detail.Quantity,
                                    Sku = string.IsNullOrEmpty(detail.Sku)
                                        ? $"{product.ProductCode}-{detail.Size}-{detail.Color?.Replace(" ", "")}"
                                        : detail.Sku
                                };
                                _context.ProductDetails.Add(newDetail);
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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

            var currentProduct = await _context.Products
                .Include(p => p.ProductDetails)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (currentProduct != null)
            {
                if (string.IsNullOrEmpty(product.Img))
                {
                    product.Img = currentProduct.Img;
                }

                if (ProductDetails == null || !ProductDetails.Any())
                {
                    product.ProductDetails = currentProduct.ProductDetails.ToList();
                }
                else
                {
                    product.ProductDetails = ProductDetails;
                }
            }

            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            GetData();
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
            GetData();
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

       
    }
}
