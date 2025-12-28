using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartProductManagementSystem.Data;
using SmartProductManagementSystem.Models;
using System.Text.Json; // JSON Session ke liye
using System.Collections.Generic; // Dictionary ke liye (Important Fix)

// Design Pattern Namespaces
using SmartProductManagementSystem.DesignPatterns.Behavioral.Command;
using SmartProductManagementSystem.DesignPatterns.Behavioral.Observer;
using SmartProductManagementSystem.DesignPatterns.Creational.Builder;
using SmartProductManagementSystem.DesignPatterns.Creational.Factory;
using SmartProductManagementSystem.DesignPatterns.Structural.Adapter;
using SmartProductManagementSystem.DesignPatterns.Structural.Decorator;

namespace SmartProductManagementSystem.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        // --- SESSION HELPER (Professional Undo) ---
        private void SetUndoSession(string actionType, int productId)
        {
            var undoData = new { Action = actionType, Id = productId };
            HttpContext.Session.SetString("LastCommand", JsonSerializer.Serialize(undoData));
        }

        // READ - List all products
        public async Task<IActionResult> Index()
        {
            if (TempData["StockNotification"] != null)
            {
                ViewBag.Notification = TempData["StockNotification"];
            }

            var products = _context.Products.Include(p => p.Category);
            return View(await products.ToListAsync());
        }

        // CREATE - GET
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // CREATE - POST (Uses BUILDER & COMMAND Pattern)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                var selectedCategory = await _context.Categories.FindAsync(product.CategoryId);
                string categoryName = selectedCategory?.Name ?? "Grocery";

                // --- 1. BUILDER PATTERN IMPLEMENTATION ---
                // "new Product()" ki jagah Builder use kar rahe hain
                var builder = new ProductBuilder();
                Product finalProduct = builder
                    .SetType(categoryName)
                    .SetBasicInfo(product.Name, product.CategoryId)
                    .SetFinancials(product.Price, product.StockQuantity)
                    .Build();

                // --- 2. COMMAND PATTERN EXECUTION ---
                ICommand command = new AddProductCommand(_context, finalProduct);
                command.Execute();

                // --- 3. SAVE STATE IN SESSION (For Undo) ---
                SetUndoSession("Add", finalProduct.Id);

                TempData["StockNotification"] = "Product added successfully via Builder!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            return View(product);
        }

        // EDIT - GET
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            return View(product);
        }

        // EDIT - POST (Command Pattern + Session)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Product updatedProduct)
        {
            if (id != updatedProduct.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existingProduct = _context.Products.Find(id);
                if (existingProduct == null) return NotFound();

                // Command Execute
                ICommand command = new UpdateProductCommand(_context, existingProduct, updatedProduct);
                command.Execute();

                // Save Session for Undo
                SetUndoSession("Edit", id);

                return RedirectToAction(nameof(Index));
            }
            return View(updatedProduct);
        }

        // DELETE - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                // ERROR FIX: Removed _lastCommand, used local variable
                ICommand command = new DeleteProductCommand(_context, product);
                command.Execute();
            }
            return RedirectToAction(nameof(Index));
        }

        // --- UNDO FEATURE (READS FROM SESSION) ---
        public IActionResult Undo()
        {
            var sessionData = HttpContext.Session.GetString("LastCommand");

            if (!string.IsNullOrEmpty(sessionData))
            {
                // Deserialize logic fixed for System.Text.Json
                var undoInfo = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(sessionData);

                // ERROR FIX: Used GetString() instead of ToString()
                string action = undoInfo["Action"].GetString();
                int id = undoInfo["Id"].GetInt32();

                if (action == "Add")
                {
                    var product = _context.Products.Find(id);
                    if (product != null)
                    {
                        var command = new DeleteProductCommand(_context, product);
                        command.Execute();
                    }
                }
                // (Future: Add logic for Edit Undo here)

                HttpContext.Session.Remove("LastCommand");
                TempData["StockNotification"] = "Last Action Undone Successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        // DECORATOR PATTERN: Apply Discount
        public IActionResult ApplyDiscount(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();

            IProductPrice price = new BaseProductPrice(product);
            string discountMessage = "";

            if (product.Price >= 3000)
            {
                price = new PercentageDiscountDecorator(price, 10);
                //discountMessage = "🔥 Mega Saver: 10% Off Applied";
            }
            else if (product.Price >= 500)
            {
                price = new FlatDiscountDecorator(price, 150);
                //discountMessage = "🏷️ Special Bonus: Flat 150 PKR Off";
            }
            else
                discountMessage = "Standard Price (No Discount on items below 500)";

            ViewBag.OriginalPrice = product.Price;
            ViewBag.FinalPrice = price.GetPrice();
            ViewBag.ProductName = product.Name;
            ViewBag.DiscountMessage = discountMessage;

            return View();
        }

        // ADAPTER PATTERN: Convert Price
        public IActionResult ConvertPrice(int id, string currency)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();

            ICurrencyAdapter adapter = new CurrencyAdapter();
            if (string.IsNullOrEmpty(currency)) currency = "PKR";

            ViewBag.OriginalPrice = product.Price;
            ViewBag.ConvertedPrice = adapter.ConvertTo(currency, product.Price);
            ViewBag.Currency = currency;
            ViewBag.ProductName = product.Name;

            return View(product);
        }

        // OBSERVER PATTERN: Update Stock
        public IActionResult UpdateStock(int productId, int newStock)
        {
            var product = _context.Products.Find(productId);
            if (product == null) return NotFound();

            product.StockQuantity = newStock;
            _context.SaveChanges();

            // Notify via TempData (Visible to User)
            var stockSubject = new StockSubject();
            stockSubject.Attach(new UserObserver());

            string msg = $"Stock updated for {product.Name}. New Quantity: {newStock}";
            stockSubject.Notify(msg);

            TempData["StockNotification"] = msg;

            return RedirectToAction("Index");
        }

        // STRATEGY PATTERN
        public IActionResult ApplyStrategyDiscount(int id, string type)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            IDiscountStrategy? strategy = type switch
            {
                "Percentage" => new PercentageDiscountStrategy(10),
                "Flat" => new FlatDiscountStrategy(200),
                _ => null
            };

            ViewBag.FinalPrice = strategy != null ? strategy.ApplyDiscount(product.Price) : product.Price;
            return View(product);
        }
    }
}