using SmartProductManagementSystem.Data;

namespace SmartProductManagementSystem.DesignPatterns.Structural
{
    public class AdminDashboardFacade
    {
        private readonly AppDbContext _context;

        public AdminDashboardFacade(AppDbContext context)
        {
            _context = context;
        }

        public int GetTotalProducts()
        {
            return _context.Products.Count();
        }

        public int GetTotalCategories()
        {
            return _context.Categories.Count();
        }

        public int GetLowStockProducts()
        {
            return _context.Products.Count(p => p.StockQuantity < 5);
        }
    }
}
