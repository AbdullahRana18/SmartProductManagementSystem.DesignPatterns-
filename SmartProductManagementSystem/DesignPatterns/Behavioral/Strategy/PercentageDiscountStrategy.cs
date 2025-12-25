public class PercentageDiscountStrategy : IDiscountStrategy
{
    private readonly decimal _percent;
    public PercentageDiscountStrategy(decimal percent) => _percent = percent;
    public decimal ApplyDiscount(decimal price) => price - (price * _percent / 100);
}
