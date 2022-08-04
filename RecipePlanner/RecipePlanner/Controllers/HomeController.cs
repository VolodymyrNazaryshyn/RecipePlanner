using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RecipePlanner.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RecipePlanner.Controllers
{
    public class HomeController : Controller
    {
        private readonly RecipeDatabaseContext _recipeDatabaseContext;

        public HomeController(RecipeDatabaseContext recipeDatabaseContext)
        {
            _recipeDatabaseContext = recipeDatabaseContext;
        }

        public IActionResult Index()
        {
            RecipeDatabaseContext recipeDatabaseContext = new RecipeDatabaseContext();
            string str = _recipeDatabaseContext.Alergens.FirstOrDefault().Name.ToString();
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
