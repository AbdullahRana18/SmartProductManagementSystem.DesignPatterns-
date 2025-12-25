public class FlatDiscountStrategy : IDiscountStrategy
{
    private readonly decimal _amount;
    public FlatDiscountStrategy(decimal amount) => _amount = amount;
    public decimal ApplyDiscount(decimal price) => price - _amount;
}
