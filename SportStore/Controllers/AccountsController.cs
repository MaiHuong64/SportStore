using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SportStore.Data;
using SportStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.Controllers
{
    public class AccountsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<Account> _passwordHasher;

        public AccountsController(ApplicationDbContext context, IPasswordHasher<Account> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }
        
        public void GetData()
        {
            var customerId= HttpContext.Session.GetInt32("CustomerId");
            if(customerId.HasValue && customerId.Value > 0)
            {
                ViewBag.Kh = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId.Value);
            }
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(string hoten, string email, string sodienthoai, string diachi, string matkhau)
        {
            var maxID = await _context.Customers.MaxAsync(c => c.CustomerId);

            var customer = new Customer
            {
                CustomerCode = "CUS" + (maxID + 1).ToString("D3"),
                FullName = hoten,
                Email = email,
                PhoneNumber = sodienthoai,
                Address = diachi
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            Account account = new Account
            {
                EmployeeId = null,
                Role = "Khách hàng",
                Status = 1,
                PhoneNumber = sodienthoai,
                CustomerId = customer.CustomerId,
            };
            account.Password = _passwordHasher.HashPassword(new Account(), matkhau);
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Login));
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string sodienthoai, string password)
        {
            var acc = await _context.Accounts.Include(name => name.Customer).Include(a => a.Employee).FirstOrDefaultAsync(a => a.PhoneNumber == sodienthoai);
            if (acc == null ||_passwordHasher.VerifyHashedPassword(acc, acc.Password, password) == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Số điện thoại hoặc mật khẩu không đúng");
                return View();
            }
            var pass = _passwordHasher.VerifyHashedPassword(acc, acc.Password, password);
            if (pass == PasswordVerificationResult.Failed)
            {
                return View();
            }
            HttpContext.Session.SetInt32("AccountId", acc.AccountId);
            HttpContext.Session.SetString("Role", acc.Role);
            HttpContext.Session.SetString("PhoneNumber", acc.PhoneNumber);
           

            if (acc.Role == "Admin" || acc.Role == "Nhân Viên")
            {
                if (acc.EmployeeId.HasValue)
                {
                    HttpContext.Session.SetInt32("EmployeeId", acc.EmployeeId.Value);
                    HttpContext.Session.SetString("FullName", acc.Employee.FullName);
                }

                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (acc.CustomerId.HasValue)
                {
                    HttpContext.Session.SetInt32("CustomerId", acc.CustomerId.Value);
                    HttpContext.Session.SetString("FullName", acc.Customer.FullName);
                }

                return RedirectToAction("Index", "Main");
            }
        }


        public IActionResult CustomerInfo()
        {
            GetData();
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            var fullName = HttpContext.Session.GetString("FullName");
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // GET: Accounts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Accounts.Include(a => a.Customer).Include(a => a.Employee);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Accounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Customer)
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: Accounts/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "FullName");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FullName");
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountId,PhoneNumber,Password,Role,Status,CustomerId,EmployeeId")] Account account)
        {
            if (ModelState.IsValid)
            {
                account.Password = _passwordHasher.HashPassword(account, account.Password);
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", account.CustomerId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", account.EmployeeId);
            return View(account);
        }

        // GET: Accounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", account.CustomerId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", account.EmployeeId);
            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountId,PhoneNumber,Password,Role,Status,CustomerId,EmployeeId")] Account account)
        {
            if (id != account.AccountId)
            {
                return NotFound();
            }
            var existAccount = await _context.Accounts.FindAsync(id);
        
            if (ModelState.IsValid)
            {
                try
                {
                    existAccount.PhoneNumber = account.PhoneNumber;
                    existAccount.Password = _passwordHasher.HashPassword(existAccount, account.Password);
                    existAccount.Role = account.Role;
                    existAccount.Status = account.Status;
                    existAccount.CustomerId = account.CustomerId;
                    existAccount.EmployeeId = account.EmployeeId;
                    _context.Update(existAccount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.AccountId))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", account.CustomerId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", account.EmployeeId);
            return View(account);
        }

        // GET: Accounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Customer)
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                _context.Accounts.Remove(account);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.AccountId == id);
        }
    }
}
