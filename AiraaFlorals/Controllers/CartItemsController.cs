using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AiraaFlorals.Models;
using Microsoft.CodeAnalysis;

namespace AiraaFlorals.Controllers
{
    public class CartItemsController : Controller
    {
        private readonly FloralsContext _context;

        private readonly FloralsContext _cartcontext;

        private readonly FloralsContext _bouquetcontext;

        public CartItemsController(FloralsContext context)
        {
            _context = context;
            _cartcontext = context;
            _bouquetcontext = context;
        }

        // GET: CartItems
        public async Task<IActionResult> Index()
        {
            int CustomerId = 0;
            string customerIdString = HttpContext.Session.GetString("CustomerId");
            if (!string.IsNullOrEmpty(customerIdString) && int.TryParse(customerIdString, out int parsedCustomerId))
            {
                CustomerId = parsedCustomerId;

                var cartItems = await _context.CartItems
                    .Include(c => c.Bouquets)
                    .Include(c => c.Cart)
                    .Where(c => c.Cart.CustomerId == CustomerId && !c.Cart.IsCheckout) 
                    .ToListAsync();

                // Retrieve the total price from the first cart item's associated cart
                decimal? totalPrice = cartItems.FirstOrDefault()?.Cart?.TotalPrice;

                ViewData["TotalPrice"] = totalPrice;
                ViewData["CartId"] = cartItems.FirstOrDefault()?.CartId;

                return View(cartItems);
            }

            // Handle the case when the customer ID is not valid or not found
            return View(new List<CartItem>()); // Or any appropriate action you want to take
        }

        // GET: CartItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CartItems == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItems
                .Include(c => c.Bouquets)
                .Include(c => c.Cart)
                .FirstOrDefaultAsync(m => m.CartItemId == id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        // GET: CartItems/Create
        public IActionResult Create()
        {
            ViewData["BouquetsId"] = new SelectList(_context.Bouquets, "BouquetId", "Description");
            ViewData["CartId"] = new SelectList(_context.Carts, "CartId", "CartId");
            return View();
        }

        // POST: CartItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CartItemId,ItemQuantity,SubTotal,CartId,BouquetsId")] CartItem cartItem)
        {
            int CustomerId = 0;
            int cartId = 0;

            string customerIdString = HttpContext.Session.GetString("CustomerId");
            if (!string.IsNullOrEmpty(customerIdString) && int.TryParse(customerIdString, out int parsedCustomerId))
            {
                CustomerId = parsedCustomerId;
                cartId = GetCartIdIfExists(CustomerId);

                if(cartItem != null && cartId == 0)
                {
                   cartId = await GetOrCreateCartForCustomer(CustomerId, (decimal)((decimal)cartItem.SubTotal * cartItem.ItemQuantity)).ConfigureAwait(false);
                }

                if (ModelState.IsValid && CustomerId != 0 && cartId !=0)
                {
                    bool isProductExist = GetCartProductsIfExists(CustomerId, cartItem.BouquetsId);
                    int? tempqty = cartItem.ItemQuantity;
                    if (isProductExist)
                    {
                        var cartproduct = _context.CartItems.FirstOrDefault(e => e.Cart.CustomerId == CustomerId && e.Cart.IsCheckout == false && e.BouquetsId == cartItem.BouquetsId);
                        cartproduct.ItemQuantity = cartproduct.ItemQuantity + cartItem.ItemQuantity;
                        cartproduct.SubTotal = cartproduct.SubTotal + (decimal)((decimal)cartItem.SubTotal * cartItem.ItemQuantity);
                        _context.Update(cartproduct);
                        await _context.SaveChangesAsync();
                        
                    }
                    else
                    {
                        cartItem.CartId = cartId;
                        cartItem.SubTotal = (decimal)((decimal)cartItem.SubTotal * cartItem.ItemQuantity);
                        _context.Add(cartItem);
                        await _context.SaveChangesAsync();
                       
                    }
                    await managestockAsync(cartItem.BouquetsId, tempqty);
                    await updateCartTotal(cartId, (decimal)cartItem.SubTotal);

                    return RedirectToAction("Index", "Bouquets");
                }

            }

            
            ViewData["BouquetsId"] = new SelectList(_context.Bouquets, "BouquetId", "Description", cartItem.BouquetsId);
            ViewData["CartId"] = new SelectList(_context.Carts, "CartId", "CartId", cartItem.CartId);
            return View(cartItem);
        }

        // GET: CartItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CartItems == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }
            ViewData["BouquetsId"] = new SelectList(_context.Bouquets, "BouquetId", "Description", cartItem.BouquetsId);
            ViewData["CartId"] = new SelectList(_context.Carts, "CartId", "CartId", cartItem.CartId);
            return View(cartItem);
        }

        // POST: CartItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CartItemId,ItemQuantity,SubTotal,CartId,BouquetsId")] CartItem cartItem)
        {
            if (id != cartItem.CartItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cartItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartItemExists(cartItem.CartItemId))
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
            ViewData["BouquetsId"] = new SelectList(_context.Bouquets, "BouquetId", "Description", cartItem.BouquetsId);
            ViewData["CartId"] = new SelectList(_context.Carts, "CartId", "CartId", cartItem.CartId);
            return View(cartItem);
        }

        // GET: CartItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CartItems == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItems
                .Include(c => c.Bouquets)
                .Include(c => c.Cart)
                .FirstOrDefaultAsync(m => m.CartItemId == id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        // POST: CartItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CartItems == null)
            {
                return Problem("Entity set 'FloralsContext.CartItems'  is null.");
            }
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem != null)
            {
               await managestockAsync(cartItem.BouquetsId, -cartItem.ItemQuantity);
               await updateCartTotal(cartItem.CartId, -cartItem.SubTotal);
                _context.CartItems.Remove(cartItem);
            }
            
            await _context.SaveChangesAsync();
            

            return RedirectToAction(nameof(Index));
        }

        private bool CartItemExists(int? id)
        {
          return (_context.CartItems?.Any(e => e.CartItemId == id)).GetValueOrDefault();
        }

        private int GetCartIdIfExists(int? customerId)
        {
            var cart = _context.Carts.FirstOrDefault(e => e.CustomerId == customerId && e.IsCheckout == false);

            if (cart != null)
            {
                return cart.CartId;
            }

            return 0;
        }


        private bool GetCartProductsIfExists(int? customerId, int? productId)
        {
            var cartproduct = _context.CartItems.FirstOrDefault(e => e.Cart.CustomerId == customerId && e.Cart.IsCheckout == false && e.BouquetsId == productId);

            if (cartproduct != null)
            {
                return true;
            }

            return false;
        }


        private async Task<int> GetOrCreateCartForCustomer(int? customerId, decimal? subtotal)
        {
            int cartId = GetCartIdIfExists(customerId);
            if (cartId == 0)
            {
                // Create a cart for the customer
                Cart cart = new Cart { CustomerId = customerId, TotalPrice = (decimal?)0.0 };
                _cartcontext.Add(cart);
                await _cartcontext.SaveChangesAsync();
                cartId = cart.CartId;
            }
            return cartId;
        }

        private async Task managestockAsync(int? productid, int? qty)
        {
            var bouquet = _bouquetcontext.Bouquets.FirstOrDefault(e => e.BouquetId == productid);

            if (bouquet != null)
            {
                // Create a cart for the customer
                Bouquets bouquets = new Bouquets();
                bouquets = bouquet;
                bouquets.Stock = bouquet.Stock - qty;
                _bouquetcontext.Update(bouquets);
                await _bouquetcontext.SaveChangesAsync();
            }
        }


        private async Task updateCartTotal(int? cartid, decimal? subtotal)
        {
            var cart = _cartcontext.Carts.FirstOrDefault(e => e.CartId == cartid);

            if (cart != null)
            {
                // Create a cart for the customer
                Cart carts = new Cart();
                carts = cart;
                carts.TotalPrice = cart.TotalPrice + subtotal;
                _cartcontext.Update(carts);
                await _cartcontext.SaveChangesAsync();
            }
        }

    }
}
