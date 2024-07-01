using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Models;
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
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
