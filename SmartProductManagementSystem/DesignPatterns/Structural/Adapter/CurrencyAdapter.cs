namespace SmartProductManagementSystem.DesignPatterns.Structural.Adapter
{
    public class CurrencyAdapter : ICurrencyAdapter
    {
        public decimal ConvertTo(string currency, decimal amount)
        {
            // Dummy conversion rates
            return currency switch
            {
                "USD" => amount / 300m,
                "EUR" => amount / 320m,
                _ => amount
            };
        }
    }
}
