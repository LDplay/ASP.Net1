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
        //������� �������� - _logger
        private readonly ILogger<HomeController> _logger;
        //��������� ��� (���-) ������
        private readonly IHashService _hashService;
        public HomeController(ILogger<HomeController> logger, IHashService hashService)
        {
            _logger = logger;
            _hashService = hashService;
            //�������� ����� ����������� - ������� �������������� ������. 
            // ��������� ����� (��������) ������ ��������� ������������ � ��� �������� �� ����� ��������� ��'���� ����� 
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
            //�� ������� ���������� �� � ��������� ���� (��������)
            if(HttpContext.Session.Keys.Contains("signup-data"))
            {
                // � ��� - �� ��������, ���������� ���
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
            /* ������ ����� �� �����, ������ 1: ����� ��������� action
             * ��'�������� ����������� ���������� �� ����� ����
             * <input name="userName"/> ------ Demo(String userName)
             * ���� � HTML ���������������� �����, �� �������� � �# 
             * (user-name), �� �������� ������� [From...] �� ����������� ���� 
             * ����� �������� ����������
             * 
             * ������ 1 ��������������� ���� �-��� ��������� �������� (1-2)
             * ����� �������������� ����� - ������������ �������
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
            /*���� ������� ���������� ����� �������� ����� �� �� �������� � �������
             * �) ���� �����������, �� ��� �� �� ��������
             * �) �������� ������ ��� ����� �� ������ ��������� �� ���������� ����� � ��, �����. ����
             * г�����: "�������� �����" - ������������� ������ �� �����'����������� �����
             * 
             * Client(Browser)                       Server(ASP)
             * [form]------------------POST RegUser----------> [form]---Session
             * <-----------------------302 SignUp------------             |
             * ------------------------GET SignUp------------>            |
             * <-----------------------HTML---------------------------- ����������
             */
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
