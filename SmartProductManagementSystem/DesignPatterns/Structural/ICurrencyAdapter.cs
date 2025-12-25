namespace SmartProductManagementSystem.DesignPatterns.Structural
{
    public interface ICurrencyAdapter
    {
        decimal ConvertTo(string currency, decimal amount);
    }
}
