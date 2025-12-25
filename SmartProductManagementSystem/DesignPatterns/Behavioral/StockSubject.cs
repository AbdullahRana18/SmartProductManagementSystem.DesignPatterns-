using System.Collections.Generic;

namespace SmartProductManagementSystem.DesignPatterns.Behavioral
{
    public class StockSubject
    {
        private readonly List<IStockObserver> _observers = new();

        public void Attach(IStockObserver observer) => _observers.Add(observer);
        public void Detach(IStockObserver observer) => _observers.Remove(observer);

        public void Notify(string message)
        {
            foreach (var obs in _observers)
                obs.Update(message);
        }
    }
}
