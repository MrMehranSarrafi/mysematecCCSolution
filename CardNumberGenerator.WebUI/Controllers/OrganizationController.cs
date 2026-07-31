using CardNoGenerator.Core;
using CardNoGenerator.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardNoGenerator.WebUI.Controllers;

[Authorize]
public class OrganizationController : MyControllersBase
{
    private readonly OrganizationService _organizationService;

    public OrganizationController(OrganizationService organizationService, IUserContextService _userContextService) :base(_userContextService)
    {
        _organizationService = organizationService; 
    }
     
    public async Task<IActionResult> Index()
    {
        try
        {
            int RowNumber = 1;
            List<Organization> organizations = await _organizationService.GetAllAsync();
            var organVMs = organizations.Select(o =>
           new OrganizationVM
           {
               Id = o.Id,
               OrganizationName = o.OrganizationName,
               Description = o.Description,
               Mobile = o.Mobile,
               Telephone = o.Telephone,
               RowNo = RowNumber++
           });

            //ViewBag.OrganizationNew = 
            return View(organVMs);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"خطا در دریافت اطلاعات: {ex.Message}";
            return View(new List<Organization>());
        }
    }
    public async Task<IActionResult> Create(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OrganizationVM organization, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(organization);

        }
        var organ = new Organization()
        {
            Description = organization.Description,
            Mobile = organization.Mobile,
            OrganizationName = organization.OrganizationName.Trim(),
            Telephone = organization.Telephone

        };
        OperationResultDto result = await _organizationService.AddAsync(organ);
        if (result.Success)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                TempData["SelectedOrganizationId"] = organ.Id;
                /// CardsManagement / CardOrderCreate ? area = &tedad = 5 & companyId = 1 & amount = 500 & description = kkk
                // ✅ ذخیره مقادیر فرم قبلی
                //TempData["CardOrder_Tedad"] = Request.Form["Tedad"];
                //TempData["CardOrder_CompanyId"] = Request.Form["CompanyId"];
                //TempData["CardOrder_Amount"] = Request.Form["Amount"];
                //TempData["CardOrder_Description"] = Request.Form["Description"];
                // ✅ استخراج مقادیر از URL با QueryHelpers
                if (returnUrl.Contains('?'))
                {
                    //var uri = new Uri(returnUrl, UriKind.Absolute);
                    var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(returnUrl.Split('?')[1]);

                    // ✅ ذخیره در TempData
                    TempData["CardOrder_Tedad"] = query["tedad"].ToString();
                    TempData["CardOrder_CompanyId"] = query["companyId"].ToString();
                    TempData["CardOrder_Amount"] = query["amount"].ToString();
                    //TempData["CardOrder_Description"] = query["description"].ToString();
                }

                return Redirect(returnUrl);// from CardOrderCreate  or CardOrderEdit
            }
            else
            {
                //TempData["Message"] = result.Message;
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
        }
        // نمایش پیام  خطای عمومی
        //- این خطا در بخش Validation Summary (معمولاً بالای فرم) نمایش داده می‌شود
        ModelState.AddModelError(result.PropertyName, result.Message);
        ViewBag.errs = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
        return View(organization);
    }

    public async Task<IActionResult> Edit(int Id)
    {
        var organ = await _organizationService.GetAsync(Id);
        var organVM = new OrganizationVM();
        if (organ != null)
        {
            organVM.Id = Id;
            organVM.Telephone = organ.Telephone;
            organVM.Mobile = organ.Mobile;
            organVM.Description = organ.Description;
            organVM.OrganizationName = organ.OrganizationName;
        }
        //if(await _organizationService.HasCardOrders(Id))
        //{
        //    ViewData["Message"] = "این سازمان در سفارش کارت ها استفاده شده و قابل ویرایش نمی باشد.";
        //}
        return View(organVM);

    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int Id, OrganizationVM organVM)
    {
        if (!ModelState.IsValid)
        {
            return View(organVM);
        }
        Organization organ = new Organization()
        {
            Id = organVM.Id,
            Telephone = organVM.Telephone,
            Mobile = organVM.Mobile,
            Description = organVM.Description,
            OrganizationName = organVM.OrganizationName.Trim()
            //,UserIdChanged =  //?بعدا

        };
        OperationResultDto operationresult = await _organizationService.EditAsync(organ);
        if (operationresult.Success)
        {
            //TempData["Message"] = operationresult.Message;
            TempData["SuccessMessage"] = operationresult.Message;
            return RedirectToAction(nameof(Index));
        }


        ModelState.AddModelError(operationresult.PropertyName, operationresult.Message);

        ViewBag.Message = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);

        return View(organVM);
    }

    //public async Task<IActionResult> CardOrderSearchTest(string companyId)
    //{
    //    List<CardOrder> list = await new CardsManagementService().CardOrderSearch(companyId);

    //    return PartialView("_CardOrderRowsPartialView", list);
    //}
    public async Task<IActionResult> Search(string searchItems)
    {
        //var users = await new UserService().GetUsersAsync(searchItems);
        var organs = await _organizationService.GetAllFilteredAsync(searchItems);
        var organVMs = organs.Select(o =>
            new OrganizationVM
            {
                Id = o.Id,
                OrganizationName = o.OrganizationName.Trim(),
                Description = o.Description,
                Mobile = o.Mobile,
                Telephone = o.Telephone
            });
        //return View("Index2",users);
        return Json(organVMs);
        //return PartialView(organizations);
    }

    [HttpGet]
    public async Task<IActionResult> SearchOrganizations(string term)
    {
        var organizationsCombo = await _organizationService.SearchOrganizations(term);
        return Json(organizationsCombo);
    }

    //[HttpGet]
    //public async Task<IActionResult> GetById(int id)
    //{
    //    var organization = await _dbContext.Organizations
    //        .Where(o => o.Id == id)
    //        .Select(o => new { o.Id, o.Name })
    //        .FirstOrDefaultAsync();

    //    return Json(organization);
    //}

}
