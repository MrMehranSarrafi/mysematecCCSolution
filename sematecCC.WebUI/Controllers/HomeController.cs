using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Core.Services;

namespace SematecCC.WebUI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserService _userService;


    public HomeController(ILogger<HomeController> logger, UserService userService)
    {
        _logger = logger;
        _userService = userService;
    }
    [Authorize]
    public IActionResult Index()
    {
        return View();
    }
    [Authorize]
    public async Task<IActionResult> IndexNew()
    {
        var menus = await _userService.GetAllPermittedMenus();//GetAllMenus();
        ViewBag.Menus = menus;
        return View();
    }
    [Authorize]
    public IActionResult IndexNew1()
    {
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