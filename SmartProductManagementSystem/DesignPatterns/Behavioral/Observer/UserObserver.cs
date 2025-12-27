namespace SmartProductManagementSystem.DesignPatterns.Behavioral.Observer
{
    public class UserObserver : IStockObserver
    {
        public string LastMessage { get; private set; } = "";

        public void Update(string message)
        {
            
            LastMessage = message;
        }
    }
}