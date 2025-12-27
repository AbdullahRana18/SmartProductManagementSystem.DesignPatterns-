using SmartProductManagementSystem.Models;

namespace SmartProductManagementSystem.DesignPatterns.Structural.Decorator
{
    public class BaseProductPrice : IProductPrice
    {
        private readonly Product _product;

        public BaseProductPrice(Product product)
        {
            _product = product;
        }

        public decimal GetPrice()
        {
            return _product.Price;
        }
    }
}
