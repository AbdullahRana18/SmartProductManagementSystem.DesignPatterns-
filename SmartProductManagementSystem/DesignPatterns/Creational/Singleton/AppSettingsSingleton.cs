namespace SmartProductManagementSystem.DesignPatterns.Creational.Singleton
{
    public sealed class AppSettingsSingleton
    {
        // Static private instance (single object)
        private static AppSettingsSingleton? _instance;

        // Lock object for thread safety
        private static readonly object _lock = new object();

        // Private constructor (cannot create object from outside)
        private AppSettingsSingleton()
        {
            ApplicationName = "Smart Product Management System";
            Currency = "PKR";
        }

        // Public static method to get instance
        public static AppSettingsSingleton Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new AppSettingsSingleton();
                    }
                    return _instance;
                }
            }
        }

        // Shared properties
        public string ApplicationName { get; set; }
        public string Currency { get; set; }
    }
}
