namespace SmartProductManagementSystem.DesignPatterns.Structural.Decorator
{
    public class PercentageDiscountDecorator : ProductPriceDecorator
    {
        private readonly decimal _percentage;

        public PercentageDiscountDecorator(IProductPrice productPrice, decimal percentage)
            : base(productPrice)
        {
            _percentage = percentage;
        }

        public override decimal GetPrice()
        {
            var price = _productPrice.GetPrice();
            return price - (price * _percentage / 100);
        }
    }
}
