namespace SmartProductManagementSystem.DesignPatterns.Structural
{
    public class FlatDiscountDecorator : ProductPriceDecorator
    {
        private readonly decimal _amount;

        public FlatDiscountDecorator(IProductPrice productPrice, decimal amount)
            : base(productPrice)
        {
            _amount = amount;
        }

        public override decimal GetPrice()
        {
            return _productPrice.GetPrice() - _amount;
        }
    }
}
