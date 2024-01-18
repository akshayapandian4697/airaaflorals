using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AiraaFlorals.Models;

namespace AiraaFlorals.Controllers
{
    public class OrdersController : Controller
    {
        private readonly FloralsContext _context;

        private readonly FloralsContext _cartcontext;

        public OrdersController(FloralsContext context)
        {
            _context = context;
            _cartcontext = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {

            int CustomerId = 0;
            string customerIdString = HttpContext.Session.GetString("CustomerId");
            if (!string.IsNullOrEmpty(customerIdString) && int.TryParse(customerIdString, out int parsedCustomerId))
            {
                CustomerId = parsedCustomerId;

                var orderItems = await _context.Orders
                    .Include(c => c.Cart)
                    .Include(c => c.Customer)
                    .Where(c => c.CustomerId == CustomerId)
                    .ToListAsync();

                return View(orderItems);
            }

            return View(new List<Order>());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Cart)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public async Task<IActionResult> Create(int? id)
        {
            ViewData["CartId"] = id;

                var cart = _cartcontext.Carts.Include(c => c.Customer).FirstOrDefault(e => e.CartId == id);
                ViewData["Total"] = cart?.TotalPrice;
                ViewData["CustomerName"] = cart?.Customer?.CustomerName;
                ViewData["datenow"] = DateTime.Now;


            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,OrderDate,OrderTotalPrice,ShippingAddress,CustomerId,CartId")] Order order)
        {
            if (ModelState.IsValid)
            {
                int customerid = 0;
                order.OrderDate = DateTime.Now;

                string customerIdString = HttpContext.Session.GetString("CustomerId");
                if (!string.IsNullOrEmpty(customerIdString) && int.TryParse(customerIdString, out int parsedCustomerId))
                {
                    customerid = parsedCustomerId;
                    order.CustomerId= customerid;
                }
                    _context.Add(order);
                await _context.SaveChangesAsync();
               await updateCart(order.CartId);
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Index", "Carts");
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CartId"] = new SelectList(_context.Carts, "CartId", "CartId", order.CartId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerName", order.CustomerId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,OrderDate,OrderTotalPrice,ShippingAddress,CustomerId,CartId")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            ViewData["CartId"] = new SelectList(_context.Carts, "CartId", "CartId", order.CartId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerName", order.CustomerId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Cart)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'FloralsContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
          return (_context.Orders?.Any(e => e.OrderId == id)).GetValueOrDefault();
        }

        private async Task updateCart(int? cartid)
        {
            var cart = _cartcontext.Carts.FirstOrDefault(e => e.CartId == cartid);

            if (cart != null)
            {
                cart.IsCheckout = true;
                _cartcontext.Update(cart);
                await _cartcontext.SaveChangesAsync();
            }
        }
    }
}
