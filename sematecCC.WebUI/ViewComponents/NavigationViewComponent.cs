using SematecCC.Core.Services;
using Microsoft.AspNetCore.Mvc;
namespace SematecCC.WebUI.ViewComponents;

public class NavigationViewComponent: ViewComponent
{
    private readonly UserService _userService;
    public NavigationViewComponent(UserService userService)
    {
        _userService = userService;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var menus = await _userService.GetAllPermittedMenus(); //GetAllMenus();
        return View("Navigation", menus);  
    }
}
