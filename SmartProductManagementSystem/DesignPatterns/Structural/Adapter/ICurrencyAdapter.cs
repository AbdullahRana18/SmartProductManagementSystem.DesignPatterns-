namespace SmartProductManagementSystem.DesignPatterns.Structural.Adapter
{
    public interface ICurrencyAdapter
    {
        decimal ConvertTo(string currency, decimal amount);
    }
}
