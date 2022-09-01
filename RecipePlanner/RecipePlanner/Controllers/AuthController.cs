using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace RecipePlanner.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserdbContext _userContext;
        private readonly Services.IHasher _hasher;
        private readonly Services.IAuthService _authService;

        public AuthController(
            UserdbContext userContext,
            Services.IHasher hasher,
            Services.IAuthService authService)
        {
            _userContext = userContext;
            _hasher = hasher;
            _authService = authService;
        }

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            ViewData["AuthUser"] = _authService.User;
        }

        public ViewResult Index()
        {
            // Сюда мы попадаем как при начале авторизации, так и в результате перенаправлений
            // из других методов. Каждый из них устанавливает свои сессионные атрибуты
            String LoginError = HttpContext.Session.GetString("LoginError");
            if(LoginError != null)
            {   // Был запрос на авторизацию (логин) и он завершился ошибкой
                ViewData["LoginError"] = LoginError;
                HttpContext.Session.Remove("LoginError");
            }
            String userId = HttpContext.Session.GetString("userId");
            if(userId != null)
            {   // Был запрос на авторизацию (логин) и он завершился успехом
                // находим пользователя по id и передаем найденный объект
                ViewData["AuthUser"] = _userContext.Users.Find(Guid.Parse(userId));
            }

            return View();
        }

        public ViewResult Register()
        {
            String regErrors = HttpContext.Session.GetString("RegErrors");

            if(regErrors != null) // Есть сообщение
            {
                ViewData["regErrors"] = regErrors.Split(";"); // разделение строки на массив
                HttpContext.Session.Remove("RegErrors"); // удаляем из сессии - однократный вывод

                ViewData["saveData"] = HttpContext.Session.GetString("saveData").Split(";");
                HttpContext.Session.Remove("saveData");

                ViewData["validData"] = HttpContext.Session.GetString("validData").Split(";");
                HttpContext.Session.Remove("validData");
            }

            return View();
        }

        [HttpPost]
        public RedirectResult      // Возвращает перенаправление (302 Response status)
            Login(                 // Название метода
            String UserEmail,      // параметры связываются по именам
            String UserPassword)   // в форме должны быть такие же имена
        {
            // Валидация данных - проверка на пустоту и шаблоны
            // Для многократных проверок часто пользуются try-catch

            try
            {
                if(String.IsNullOrEmpty(UserEmail))
                    throw new Exception("Email empty");

                if(String.IsNullOrEmpty(UserPassword))
                    throw new Exception("Password empty");

                // проверяем (авторизируемся)
                // 1. По Email ищем пользователя и извлекаем соль, хеш пароля
                _authService.User =
                    _userContext
                    .Users
                    .Where(u => u.Email == UserEmail)
                    .FirstOrDefault();

                if(_authService.User == null) // нет пользователя с таким логином
                    throw new Exception("Email invalid");

                // 2. Хешируем введенный пароль + соль
                String PassHash = _hasher.Hash(UserPassword + _authService.User.PassSalt);

                // 3. Проверяем равенство полученного и хранимого хешей
                if(PassHash != _authService.User.PassHash)
                    throw new Exception("Password invalid");
            }
            catch(Exception ex)
            {   // сюда мы попадаем если была ошибка
                HttpContext.Session.SetString("LoginError", ex.Message);
                return Redirect("/Auth/Register"); // завершаем обработку
            }
            // Если не было catch, то и не было ошибок
            // тогда user - авторизованный пользователь, сохраняем его данные в сессии (обычно ограничиваются id)

            HttpContext.Session.SetString("userId", _authService.User.Id.ToString()); // регистрируем userId
            // создаем метку времени начала авторизации для контроля ее длительности
            HttpContext.Session.SetString("AuthMoment", DateTime.Now.Ticks.ToString());

            // Метод закончится установкой сессии LoginError либо userId и перенаправлением
            return Redirect("/"); //    /Home/Index - главная страница
        }

        public RedirectResult Logout()
        {
            HttpContext.Session.Remove("userId");
            _userContext.SaveChanges();
            return Redirect("/");
        }

        private bool EmailExist(String email)
        {
            var listOfEmails = _userContext.Users.Select(u => u.Email).ToList();
            foreach(var item in listOfEmails)
                if(email == item)
                    return true; // проверка e-mail на уникальность

            return false;
        }

        [HttpPost]
        public IActionResult RegUser(Models.RegUserModel UserData)
        {
            // return Json(UserData); // способ проверить передачу данных
            
            String[] errors = new string[10]; // ошибки валидации
            String[] saveData = new string[4]; // имя, пароли и e-mail
            bool isValidEmail = true;
            bool isValidRealName = true;
            int errorCount = 0;

            if(UserData == null)
            {
                errors[0] = "Incorrect request (no data)";
                errorCount++;
            }
            else
            {
                if(String.IsNullOrEmpty(UserData.RealName))
                {
                    errors[1] = "Name couldn`t be empty";
                    errorCount++;
                }
                // серверная валидация по типу "Имя Фамилия"
                else if(!Regex.IsMatch(UserData.RealName, @"^[A-Z][a-z]+ [A-Z][a-z]+$"))
                {
                    errors[2] = "Doesn`t follow the pattern \"FirstName LastName\"";
                    isValidRealName = false;
                    errorCount++;
                }
                saveData[0] = UserData.RealName;

                if(String.IsNullOrEmpty(UserData.Email))
                {
                    errors[3] = "E-mail couldn`t be empty";
                    errorCount++;
                }
                else if(EmailExist(UserData.Email))
                {
                    errors[4] = "Such an e-mail already exists";
                    isValidEmail = false;
                    errorCount++;
                }
                saveData[1] = UserData.Email;

                if(String.IsNullOrEmpty(UserData.Password1))
                {
                    errors[5] = "Password couldn`t be empty";
                    errorCount++;
                }
                saveData[2] = UserData.Password1;

                if(UserData.Password1 != UserData.Password2)
                {
                    errors[6] = "Passwords don`t match";
                    errorCount++;
                }
                saveData[3] = UserData.Password2;

                if(String.IsNullOrEmpty(UserData.BirthdayDate.ToString()))
                {
                    errors[7] = "Birthday date couldn`t be empty";
                    errorCount++;
                }

                if(String.IsNullOrEmpty(UserData.Gender))
                {
                    errors[8] = "Gender couldn`t be empty";
                    errorCount++;
                }

                if(String.IsNullOrEmpty(UserData.Region))
                {
                    errors[9] = "Region couldn`t be empty";
                    errorCount++;
                }

                // если валидация пройдена (нет сообщений об ошибках) - добавляем пользователя в БД
                if(errorCount == 0)
                {
                    var user = new User();
                    user.UserName = UserData.RealName;
                    user.Email = UserData.Email;
                    user.PassSalt = _hasher.Hash(DateTime.Now.ToString());
                    user.PassHash = _hasher.Hash(UserData.Password1 + user.PassSalt);
                    user.BirthdayDate = UserData.BirthdayDate;
                    user.Gender = UserData.Gender;
                    user.Region = UserData.Region;
                    user.RegMoment = DateTime.Now;

                    _userContext.Users.Add(user);
                    _userContext.SaveChanges();

                    saveData[0] = null; // RealName
                    saveData[1] = null; // Email
                }
            }

            String[] validData = new string[2];
            validData[0] = isValidRealName.ToString();
            validData[1] = isValidEmail.ToString();

            // Сессия - "межзапросное хранилище", обычно сохраняют значимые типы
            HttpContext.Session.SetString("RegErrors", String.Join(";", errors));
            HttpContext.Session.SetString("saveData", String.Join(";", saveData));
            HttpContext.Session.SetString("validData", String.Join(";", validData));

            //return View(UserData);
            return RedirectToAction("Register");
        }

        public IActionResult Profile()
        {
            // если пользователь не авторизован
            if(_authService.User == null)
            {
                // перенаправить на страницу логина
                return Redirect("/Auth/Register");
            }

            return View();
        }

        public String ChangeRealName(String NewName)
        {
            if(_authService.User == null) // если пользователь не авторизован
            {
                return "Unauthorized user"; // запрещенный доступ
            }

            if(String.IsNullOrEmpty(NewName)) // проверка на пустоту
            {
                return "Name couldn`t be empty!";
            }
            else if(!Regex.IsMatch(NewName, @"^[A-Z][a-z]+ [A-Z][a-z]+$")) // серверная валидация имени
            {
                return $"{NewName} doesn`t follow the pattern (example: Linus Torvalds)";
            }
            else // не было ошибок
            {
                // обновляем данные в БД
                _authService.User.UserName = NewName;
                _userContext.SaveChanges();
                return "Name was updated!";
            }
        }

        [HttpPut]
        public JsonResult ChangeEmail([FromForm] String NewEmail)
        {
            String message = $"OK {NewEmail}";
            if(_authService.User == null) // если пользователь не авторизован
            {
                message = "Unauthorized user"; // запрещенный доступ
            }
            else if(String.IsNullOrEmpty(NewEmail))
            {
                message = "Email couldn`t be empty";
            }
            else if(Regex.IsMatch(NewEmail, @"\s"))
            {
                message = "Email couldn`t contain space(s)";
            }
            else if(_userContext.Users.Where(u => u.Email == NewEmail).Count() > 0)
            {
                message = "Email in use";
            }
            else if(!Regex.IsMatch(NewEmail, @"^[A-z][A-z\d_]{3,16}@([a-z]{1,10}\.){1,5}[a-z]{2,3}$"))
            {
                message = NewEmail + @" doesn`t follow the pattern (example: volodimir@gmail.com, Alex@mail.odessa.ua)";
            }

            if(message == $"OK {NewEmail}") // не было ошибок
            {
                // обновляем данные в БД
                _authService.User.Email = NewEmail;
                _userContext.SaveChanges();
            }

            return Json(message);
        }

        [HttpPut]
        public JsonResult ChangePassword([FromForm] String NewPassword)
        {
            String message = "OK";
            if(_authService.User == null) // если пользователь не авторизован
            {
                message = "Unauthorized user"; // запрещенный доступ
            }
            else if(String.IsNullOrEmpty(NewPassword))
            {
                message = "Password couldn`t be empty";
            }
            else if(Regex.IsMatch(NewPassword, @"\s"))
            {
                message = "Password couldn`t contain space(s)";
            }
            else if(!Regex.IsMatch(NewPassword, @"(?=^.{8,}$)((?=.*\d)|(?=.*\W+))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*"))
            {
                message = "Password doesn`t follow the pattern: Minimum 8 characters, one digit, one uppercase letter and one lowercase letter";
            }

            if(message == "OK") // не было ошибок
            {
                // Хешируем введенный пароль + соль
                String PassHash = _hasher.Hash(NewPassword + _authService.User.PassSalt);
                // обновляем данные в БД
                _authService.User.PassHash = PassHash;
                _userContext.SaveChanges();
                message = "Password was updated!";
            }

            return Json(message);
        }
    }
}
