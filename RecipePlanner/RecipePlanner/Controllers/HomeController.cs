using Microsoft.AspNetCore.Mvc;
using RecipePlanner.Models;
using RecipePlanner.Services;
using System.Diagnostics;
using System.Linq;

namespace RecipePlanner.Controllers
{
    public class HomeController : Controller
    {
        private readonly RecipeDatabaseContext _recipeDbContext;
        private readonly UserdbContext _userContext;
        private readonly IAuthService _authService;

        public HomeController( // Внедрение зависимостей через конструктор
            RecipeDatabaseContext recipeDbContext,
            UserdbContext userContext,
            IAuthService authService)
        {
            _recipeDbContext = recipeDbContext;
            _userContext = userContext;
            _authService = authService;
        }

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            ViewData["AuthUser"] = _authService.User;
            // base.OnActionExecuting(context);
        }

        public IActionResult Index()
        {
            // проверяем службу авторизации
            ViewData["authUserName"] = _authService.User?.UserName;
            ViewData["UsersCount"] = _userContext.Users.Count();
            ViewData["UsersRealName"] = _userContext.Users.Select(u => u.UserName).ToList();
            // string str = _recipeDbContext.Alergens.FirstOrDefault().Name.ToString();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public ViewResult About()
        {
            var model = new Models.AboutModel
            {
                Data = "The Model Data"
            };
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
