using Microsoft.AspNetCore.Mvc;
using SmartProductManagementSystem.Data;
using SmartProductManagementSystem.DesignPatterns.Structural;
using SmartProductManagementSystem.DesignPatterns.Creational.Singleton;

namespace SmartProductManagementSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly AdminDashboardFacade _dashboardFacade;

        public AdminController(AppDbContext context)
        {
            _dashboardFacade = new AdminDashboardFacade(context);
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
