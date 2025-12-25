using SmartProductManagementSystem.Models;
using SmartProductManagementSystem.Data;

namespace SmartProductManagementSystem.DesignPatterns.Behavioral.Command
{
    public class AddProductCommand : ICommand
    {
        private readonly AppDbContext _context;
        private readonly Product _product;

        public AddProductCommand(AppDbContext context, Product product)
        {
            _context = context;
            _product = product;
        }

        public void Execute()
        {
            _context.Products.Add(_product);
            _context.SaveChanges();
        }

        public void Undo()
        {
            _context.Products.Remove(_product);
            _context.SaveChanges();
        }
    }
}
