using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartProductManagementSystem.Data;
using SmartProductManagementSystem.DesignPatterns.Behavioral;
using SmartProductManagementSystem.DesignPatterns.Structural;
using SmartProductManagementSystem.DesignPatterns.Behavioral.Command;
using SmartProductManagementSystem.DesignPatterns.Behavioral.Observer;
using SmartProductManagementSystem.Models;
using SmartProductManagementSystem.DesignPatterns.Creational.Factory;
using SmartProductManagementSystem.DesignPatterns.Structural.Adapter;
using SmartProductManagementSystem.DesignPatterns.Structural.Decorator;

namespace SmartProductManagementSystem.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private static ICommand? _lastCommand; // Undo feature ke liye static state

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        // READ - List all products
        public async Task<IActionResult> Index()
        {
            var products = _context.Products.Include(p => p.Category);
            return View(await products.ToListAsync());
        }

        // CREATE - GET
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // CREATE - POST (Dynamic Factory + Command Pattern)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                // 1. Database se selected Category ka naam nikalna
                var selectedCategory = await _context.Categories.FindAsync(product.CategoryId);
                string categoryName = selectedCategory?.Name ?? "Grocery";

                // 2. Dynamic Factory Pattern Link
                try
                {
                    // Agar factory mein ye type mojud hai (Electronics, Grocery, etc)
                    var productTypeObj = ProductFactory.CreateProduct(categoryName);
                    product.ProductType = productTypeObj.GetProductType();
                }
                catch
                {
                    // Agar koi aisi category hai jo factory mein nahi, to uska naam hi type ban jaye
                    product.ProductType = categoryName;
                }

                // 3. Save using Command Pattern
                _lastCommand = new AddProductCommand(_context, product);
                _lastCommand.Execute();

                // 4. Alert Notification
                TempData["StockNotification"] = "Product added successfully!";

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

        // EDIT - POST (Command Pattern)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Product updatedProduct)
        {
            if (id != updatedProduct.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existingProduct = _context.Products.Find(id);
                if (existingProduct == null) return NotFound();

                _lastCommand = new UpdateProductCommand(_context, existingProduct, updatedProduct);
                _lastCommand.Execute();

                return RedirectToAction(nameof(Index));
            }
            return View(updatedProduct);
        }

        // DELETE - POST (Command Pattern)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _lastCommand = new DeleteProductCommand(_context, product);
                _lastCommand.Execute();
            }
            return RedirectToAction(nameof(Index));
        }

        // DECORATOR PATTERN: Apply Discount
        public IActionResult ApplyDiscount(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();

            IProductPrice price = new BaseProductPrice(product);
            price = new PercentageDiscountDecorator(price, 10);
            price = new FlatDiscountDecorator(price, 100);

            ViewBag.OriginalPrice = product.Price;
            ViewBag.FinalPrice = price.GetPrice();
            ViewBag.ProductName = product.Name;

            return View();
        }

        // ADAPTER PATTERN: Convert Price
        public IActionResult ConvertPrice(int id, string currency)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();

            ICurrencyAdapter adapter = new CurrencyAdapter();
            if (string.IsNullOrEmpty(currency))
            {
                currency = "PKR";
            }
            ViewBag.OriginalPrice = product.Price;
            ViewBag.ConvertedPrice = adapter.ConvertTo(currency, product.Price);
            ViewBag.Currency = currency;
            ViewBag.ProductName = product.Name;

            return View(product);
        }

        // OBSERVER PATTERN: Update Stock Notification
        public IActionResult UpdateStock(int productId, int newStock)
        {
            var product = _context.Products.Find(productId);
            if (product == null) return NotFound();

            product.StockQuantity = newStock;
            _context.SaveChanges();

            var stockSubject = new StockSubject();
            var userObserver = new UserObserver();
            stockSubject.Attach(userObserver);

            string msg = $"Stock updated for {product.Name}. New Quantity: {newStock}";
            stockSubject.Notify(msg);

            TempData["StockNotification"] = msg;

            return RedirectToAction("Index");
        }

        // STRATEGY PATTERN: Apply Discount Strategy
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

        // UNDO Feature
        public IActionResult Undo()
        {
            _lastCommand?.Undo();
            return RedirectToAction(nameof(Index));
        }
    }
}