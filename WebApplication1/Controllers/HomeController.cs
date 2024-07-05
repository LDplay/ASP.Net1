using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
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
    }
}
