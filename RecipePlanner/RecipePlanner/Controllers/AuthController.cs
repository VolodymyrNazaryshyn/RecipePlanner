using Microsoft.AspNetCore.Mvc;

namespace RecipePlanner.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserdbContext _userContext;

        public AuthController(UserdbContext userContext)
        {
            _userContext = userContext;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Models.RegUserModel UserData)
        {
            if (UserData == null)
            {

            }
            //ViewData["err"] = new string[] { "1", "2" };
            return View(UserData);
        }
    }
}
