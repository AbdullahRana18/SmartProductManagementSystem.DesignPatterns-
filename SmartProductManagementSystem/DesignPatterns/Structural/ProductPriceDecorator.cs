namespace SmartProductManagementSystem.DesignPatterns.Structural
{
    public abstract class ProductPriceDecorator : IProductPrice
    {
        protected IProductPrice _productPrice;

        protected ProductPriceDecorator(IProductPrice productPrice)
        {
            _productPrice = productPrice;
        }

        public abstract decimal GetPrice();
    }
}
