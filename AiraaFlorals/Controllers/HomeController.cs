using AiraaFlorals.Models;
using Microsoft.AspNetCore.Mvc;

namespace AiraaFlorals.Controllers
{
    public class HomeController : Controller
    {
        private readonly FloralsContext _context;

        public HomeController(FloralsContext context)
        {
            _context = context;
        }
        public IActionResult Index(String? successmsg)
        {
            ViewBag.SuccessMsg = TempData["successmsg"];
            ViewBag.Loginerror = TempData["loginerror"];
            return View("LoginRegister");
        }

        public IActionResult Homescreen()
        {
            ViewBag.Fullname = HttpContext.Session.GetString("CustName");
            return View();
        }

        [HttpPost]
        public IActionResult Login(string UserName, string Password)
        {
            bool iscustomerexist = CustomerExists(UserName, Password);
            if (iscustomerexist)
            {
                var customer = _context.Customers.FirstOrDefault(e => e.UserName == UserName && e.Password == Password);

                if (customer != null)
                {
                   HttpContext.Session.SetString("CustomerId", customer.CustomerId.ToString());
                   HttpContext.Session.SetString("CustName", customer.CustomerName);
                }

                
                return RedirectToAction(nameof(Homescreen));
            }
            else
            {
                TempData["loginerror"] = "Login failed!";
                return RedirectToAction(nameof(Index));
            }

        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("CustomerId");
            HttpContext.Session.Remove("CustName");

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,CustomerName,UserName,Password")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                TempData["successmsg"] = "Registered successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        private bool CustomerExists(string username, string password)
        {
            bool result = (_context.Customers?.Any(e => e.UserName == username && e.Password == password)).GetValueOrDefault();
            
            return result;
        }

        
    }
}
