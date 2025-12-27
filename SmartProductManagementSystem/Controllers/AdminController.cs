using Microsoft.AspNetCore.Mvc;
using SmartProductManagementSystem.Data; // Agar zaroorat ho to
using SmartProductManagementSystem.DesignPatterns.Creational.Singleton;
using SmartProductManagementSystem.DesignPatterns.Structural.Facade;

namespace SmartProductManagementSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly AdminDashboardFacade _dashboardFacade;

        // Constructor Injection (Program.cs se Facade automatic ayega)
        public AdminController(AdminDashboardFacade dashboardFacade)
        {
            _dashboardFacade = dashboardFacade;
        }

        public IActionResult Dashboard()
        {
            ViewBag.TotalProducts = _dashboardFacade.GetTotalProducts();
            ViewBag.TotalCategories = _dashboardFacade.GetTotalCategories();
            ViewBag.LowStockProducts = _dashboardFacade.GetLowStockProducts();

            return View();
        }

        public IActionResult Index()
        {
            var settings = AppSettingsSingleton.Instance;

            ViewBag.AppName = settings.ApplicationName;
            ViewBag.Currency = settings.Currency;

            return View();
        }
    }
}