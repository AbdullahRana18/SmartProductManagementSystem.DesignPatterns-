using SmartProductManagementSystem.Models;
using SmartProductManagementSystem.DesignPatterns.Creational.Factory;

namespace SmartProductManagementSystem.DesignPatterns.Creational.Builder
{
    // Requirement: "Helps build complex product objects step-by-step"
    public class ProductBuilder
    {
        private Product _product = new Product();

        // Step 1: Factory se Type set karo
        public ProductBuilder SetType(string categoryName)
        {
            try
            {
                var productTypeObj = ProductFactory.CreateProduct(categoryName);
                _product.ProductType = productTypeObj.GetProductType();
            }
            catch
            {
                _product.ProductType = categoryName; // Fallback
            }
            return this;
        }

        // Step 2: Basic Info set karo
        public ProductBuilder SetBasicInfo(string name, int categoryId)
        {
            _product.Name = name;
            _product.CategoryId = categoryId;
            return this;
        }

        // Step 3: Price aur Stock set karo
        public ProductBuilder SetFinancials(decimal price, int stock)
        {
            _product.Price = price;
            _product.StockQuantity = stock;
            return this;
        }

        // Final Build
        public Product Build()
        {
            return _product;
        }
    }
}