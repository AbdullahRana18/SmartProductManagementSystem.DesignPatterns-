using System;

namespace SmartProductManagementSystem.DesignPatterns.Creational.Factory
{
    public class ProductFactory
    {
        public static IProduct CreateProduct(string type)
        {
            return type switch
            {
                "Electronics" => new ElectronicsProduct(),
                "Grocery" => new GroceryProduct(),
                "Furniture" => new FurnitureProduct(),
                _ => throw new ArgumentException("Invalid product type")
            };
        }
    }
}
