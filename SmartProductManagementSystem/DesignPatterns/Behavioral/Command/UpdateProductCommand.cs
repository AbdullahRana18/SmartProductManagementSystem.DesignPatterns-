using SmartProductManagementSystem.Models;
using SmartProductManagementSystem.Data;

namespace SmartProductManagementSystem.DesignPatterns.Behavioral.Command
{
    public class UpdateProductCommand : ICommand
    {
        private readonly AppDbContext _context;
        private readonly Product _oldProduct;
        private readonly Product _newProduct;

        public UpdateProductCommand(AppDbContext context, Product oldProduct, Product newProduct)
        {
            _context = context;
            _oldProduct = oldProduct;
            _newProduct = newProduct;
        }

        public void Execute()
        {
            _context.Entry(_oldProduct).CurrentValues.SetValues(_newProduct);
            _context.SaveChanges();
        }

        public void Undo()
        {
            _context.Entry(_newProduct).CurrentValues.SetValues(_oldProduct);
            _context.SaveChanges();
        }
    }
}
