using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using WebApplication1.Models;
using WebApplication1.Models.Home;
using WebApplication1.Services.Hash;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        //Приклад інжекції - _logger
        private readonly ILogger<HomeController> _logger;
        //Інжуктеємо наш (хеш-) сервис
        private readonly IHashService _hashService;
        public HomeController(ILogger<HomeController> logger, IHashService hashService)
        {
            _logger = logger;
            _hashService = hashService;
            //інжекція через конструктор - найбільш рекомендований варіант. 
            // Контейнер служб (інжектор) аналізує параметри контрукотора і сам підсталяє до нього необхідна об'єкти служб 
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Intro()
        {
            return View();
        }
        public IActionResult Razor()
        {
            return View();
        }
        public IActionResult Ioc()
        {
            ViewData["hash"] = _hashService.Digest("123");
            ViewData["hashCode"] = _hashService.GetHashCode();
            return View();
        }
        public IActionResult SignUp()
        {
            SignUpPageModel model = new();
            //На початку перевіряємо чи є збережена сесія (редирект)
            if(HttpContext.Session.Keys.Contains("signup-data"))
            {
                // є дфні - це редирект, обробляємо дані
                var formModel = JsonSerializer.Deserialize<SignUpFormModel>(
                    HttpContext.Session.GetString("signup-data")!)!;
                model.FormModel = formModel;
                model.ValidationErrors = _Validate(formModel);
                ViewData["data"] = $"email: {formModel.UserEmail}, name: {formModel.UserName}";

                HttpContext.Session.Remove("signup-data");
            }
            return View(model);
        }
        public IActionResult Demo([FromQuery(Name ="user-email")]String userEmail, [FromQuery(Name = "user-name")] String userName)
        {
            /* Прийом даних від форми, варіант 1: через параметри action
             * Зв'язування автоматично відбуваєтсья за збігом імен
             * <input name="userName"/> ------ Demo(String userName)
             * якщо в HTML використовуються імена, які неможливі у С# 
             * (user-name), то додається атрибут [From...] із зазначенням імені 
             * перед потрібним параметром
             * 
             * Варіант 1 використовується коли к-сть параметрів невелика (1-2)
             * Більш рекомендований спосіб - використання моделей
             */
            ViewData["data"] = $"email: {userEmail}, name: {userName}";
            return View();
        }
        public IActionResult RegUser(SignUpFormModel formModel)
        {
            HttpContext.Session.SetString("signup-data", 
                System.Text.Json.JsonSerializer.Serialize(formModel));

            return RedirectToAction(nameof(SignUp));

            //ViewData["data"] = $"email: {formModel.UserEmail}, name: {formModel.UserName}";
            //return View("Demo");
            /*якщо сторінка побудована через передачу форми то її оновленя у браузері
             * а) видає повідомлення, на яке ми не впливаємо
             * б) повторно передає діна форми що можуть призвести до дублювання даних у бд, файлів. тощо
             * Рішення: "Скидання даних" - переадресація відповіді із запам'ятовуванням даних
             * 
             * Client(Browser)                       Server(ASP)
             * [form]------------------POST RegUser----------> [form]---Session
             * <-----------------------302 SignUp------------             |
             * ------------------------GET SignUp------------>            |
             * <-----------------------HTML---------------------------- оброблення
             */
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private Dictionary<String, String?> _Validate(SignUpFormModel model)
        {
            /* Валідація - перевірка даних на відповідність певним шаблонам/правилам
             * Результат валідації -{
             *    "UserEmail": null,    null - як ознака успішної валідації
             *    "UserName": "Too short" значення - повідомлення про помилку
             * }
             */
            Dictionary<String, String?> res = new();
            var emailRegex = new Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
            res[nameof(model.UserEmail)] =
                String.IsNullOrEmpty(model.UserEmail)
                ? "Не допускаєжться порожнє поле"
                :emailRegex.IsMatch(model.UserEmail)
                    ? null
                    : "Введіть коректну адресу";

            var nameRegex = new Regex(@"^\w{2,}(\s+\w{2,})*$");
            res[nameof(model.UserName)] =
                String.IsNullOrEmpty(model.UserName)
                ? "Не допускаєжться порожнє поле"
                : nameRegex.IsMatch(model.UserName)
                    ? null
                    : "Введіть коректне ім'я";

            if (String.IsNullOrEmpty(model.UserPassword))
            {
                res[nameof(model.UserPassword)] = "Не допускаєжться порожнє поле";
            }
            else if (model.UserPassword.Length < 3)
            {
                res[nameof(model.UserPassword)] = "Пароль не має бути коротшим за 8 символів";
            }
            else
            {
                List<String> parts = [];
                if (!Regex.IsMatch(model.UserPassword, @"\d"))
                {
                    parts.Add(" одну цифру");
                }
                if (!Regex.IsMatch(model.UserPassword, @"\D"))
                {
                    parts.Add(" одну літеру");
                }
                if (!Regex.IsMatch(model.UserPassword, @"\D"))
                {
                    parts.Add(" один спецсимвол");
                }
                if (parts.Count > 0)
                {
                    res[nameof(model.UserPassword)] = "Пароль повинен містити щонайменше" + String.Join(",", parts);
                }
                else
                {
                    res[nameof(model.UserPassword)] = null;
                }
            }


            res[nameof(model.UserRepeat)] = model.UserPassword == model.UserRepeat
                ? null
                : "Паролі не збігаються";


            res[nameof(model.IsAgree)] = model.IsAgree ? null : "необхідно прийняти правила сайту";
            return res;
        }
    }
}
