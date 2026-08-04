using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Application.DTO;
using Core.Domain.Entities;
using Domain.Helpers;
using Domain.ServiceContracts;
using Domain.Services;

namespace SematecCC.WebUI.Controllers;

[Authorize]
public class PersonController : MyControllersBase
{
    private readonly PersonService _ownerPersonService;
     
    //int i = 1;
   
    public PersonController(PersonService ownerPersonService, CompanyService companyService,IUserContextService userContext):base(userContext, companyService) 
    {
        _ownerPersonService = ownerPersonService;
        
    }
    public async Task<IActionResult> Index()
    {
        (bool? isAdmin, int? currentCompanyId, int? currentUserId) = GetAuthenticationInfo();
        ViewBag.IsAdmin = isAdmin == true ? true : false;
        var owners = await _ownerPersonService.GetAllAsync(isAdmin, currentCompanyId, currentUserId);
        var ownerVMs = owners.Select(o => new PersonVM
        {
            Id = o.Id,
            Mobile = o.Mobile,
            BirthDate = o.BirthDate,
            BirthDateFa = o.BirthDateFa,
            FirstName = o.FirstName,
            LastName = o.LastName,
            JobPlace = o.JobPlace,
            Phone = o.Phone,
            NationalCode = o.NationalCode,
            GivId = o.GivId,
            Company = o.Company

        }).ToList();
        return View(ownerVMs);
    }
    public async Task<IActionResult> Create(string MobileNO = "")
    {
        //bool? IsAdmin = _userContext.IsUserAdmin();
        //ViewBag.IsAdmin = IsAdmin;
        //List<ComboItemsList> Companies = await _companyService.GetComboCompanies();//.GetCompanyIDs();
        //int CompanyId = await _userService.GetCompanyId(_userContext.GetCurrentUserId());
        //ViewBag.CompanyId = CompanyId;

        //if (IsAdmin == false)
        //{
        //    Companies = Companies.Where(c => c.Value == CompanyId.ToString()).ToList();
        //}
        //ViewBag.Companies = new SelectList(Companies, "Value", "Text");

        #region GetComboAllowedCompanies 
        (bool? isAdmin, int? currentCompanyId, int? currentUserId) = GetAuthenticationInfo();
        //var companies = await _companyService.GetComboAllowedCompanies(isAdmin, currentCompanyId,currentUserId);
        //ViewBag.Companies = new SelectList(companies, "Value", "Text");
        var companies = await GetComboAllowedCompaniesSelectList(isAdmin, currentCompanyId, currentUserId);
        ViewBag.Companies = new SelectList(companies, "Value", "Text");
        //ViewBag.CompanyId = currentCompanyId;
        ViewBag.IsAdmin = isAdmin;
        #endregion
        //i = 2;
        ViewBag.MobileNO = MobileNO;
        return View();
    }

    

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PersonVM personVm)
    {
        //var ii = i; نکته: i==1 است چون درخواست جدید است و مجددا شیء ایجاد شده و به دفعه قبل و درخواست قبل ربطی ندارد
        (bool? isAdmin, int? currentCompanyId, int? currentUserId) = GetAuthenticationInfo();
        personVm.UserCreated = currentUserId;//GetCurrentUserId();
        if (!ModelState.IsValid)
        {
            return View(personVm);
        }
        var owner = new Person
        {
            Id = personVm.Id,
            BirthDate = personVm.BirthDateFa.ToMiladi(),
            BirthDateFa = personVm.BirthDateFa,
            FirstName = personVm.FirstName,
            LastName = personVm.LastName,
            JobPlace = personVm.JobPlace,
            Mobile = personVm.Mobile,
            NationalCode = personVm.NationalCode,
            Phone = personVm.Phone,
            UserIdCreated = personVm.UserCreated.Value,
            GivId = personVm.GivId,
            CompanyId = personVm.CompanyId

        };
        OperationResultDto result = await _ownerPersonService.CreateAsync(owner);
        if (result.Success)
        {
            //TempData["Message"] = result.Message;
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // نمایش پیام  خطای عمومی
        ModelState.AddModelError(result.PropertyName, result.Message);
        ViewBag.errs = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
        #region GetComboAllowedCompanies 
        var companies = await GetComboAllowedCompaniesSelectList(isAdmin, currentCompanyId, currentUserId);
        ViewBag.Companies = new SelectList(companies, "Value", "Text");
        ViewBag.IsAdmin = isAdmin;
        #endregion

        return View(personVm);
    }

    public async Task<IActionResult> Search(string searchItems)
    {
        (bool? isAdmin, int? currentCompanyId, int? currentUserId) = GetAuthenticationInfo();
        //var owners = await _ownerPersonService.GetAllFilteredAsync(searchItems);
        var owners = await _ownerPersonService.GetAllFilteredAsync(searchItems, isAdmin, currentCompanyId, currentUserId);
        var ownerVMs = owners.Select(o =>
            new PersonVM
            {
                Id = o.Id,
                Mobile = o.Mobile,
                BirthDate = o.BirthDate,
                BirthDateFa = o.BirthDateFa,
                FirstName = o.FirstName,
                LastName = o.LastName,
                JobPlace = o.JobPlace,
                Phone = o.Phone,
                NationalCode = o.NationalCode,
                GivId = o.GivId,
                Company = o.Company
            });
        //return View("Index2",users);
        return Json(ownerVMs);
        //return PartialView(organizations);
    }
    public async Task<IActionResult> Edit(int Id, string? returnUrl=null)
    {
        #region GetComboAllowedCompanies 
        (bool? isAdmin, int? currentCompanyId, int? currentUserId) = GetAuthenticationInfo();
        //var companies = GetComboAllowedCompaniesSelectList(isAdmin, currentCompanyId, currentUserId);//new SelectList(companies, "Value", "Text");
        var companies = await _companyService.GetComboAllowedCompanies(isAdmin, currentCompanyId,currentUserId); 
        ViewBag.Companies =  new SelectList(companies, "Value", "Text");           
        ViewBag.CompanyId = currentCompanyId;
        ViewBag.IsAdmin = isAdmin;
        #endregion
        var owner = await _ownerPersonService.GetByIdAsync(Id);
        var vmOwner = new PersonVM();
        if (owner != null)
        {
            vmOwner.Id = owner.Id;
            vmOwner.LastName = owner.LastName;
            vmOwner.FirstName = owner.FirstName;
            vmOwner.Phone = owner.Phone;
            vmOwner.BirthDate = owner.BirthDate;
            vmOwner.BirthDateFa = owner.BirthDateFa;
            vmOwner.NationalCode = owner.NationalCode;
            vmOwner.JobPlace = owner.JobPlace;
            vmOwner.Mobile = owner.Mobile;
            vmOwner.GivId = owner.GivId;
            vmOwner.CompanyId = owner.CompanyId;
            vmOwner.Company = owner.Company;
        }
        ViewBag.returnUrl = returnUrl;//از لیست سفارش های کارت میاد، لینک مالک کارت
        return View(vmOwner);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int Id, PersonVM ownerVM, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(ownerVM);
        }
        var owner = new Person()
        {
            Id = ownerVM.Id,
            LastName = ownerVM.LastName,
            FirstName = ownerVM.FirstName,
            Phone = ownerVM.Phone,
            BirthDate = ownerVM.BirthDateFa.ToMiladi(),
            BirthDateFa = ownerVM.BirthDateFa,
            NationalCode = ownerVM.NationalCode,
            JobPlace = ownerVM.JobPlace,
            Mobile = ownerVM.Mobile,
            Company = ownerVM.Company,
            CompanyId = ownerVM.CompanyId,
            GivId  = ownerVM.GivId
        };
        OperationResultDto operationresult = await _ownerPersonService.EditAsync(owner);
        if (operationresult.Success)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);// از جزِییات لیست سفارش های کارت، لینک مالک کارت آمده، 
            }
            else
            {
                //TempData["Message"] = operationresult.Message;
                TempData["SuccessMessage"] = operationresult.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        ModelState.AddModelError(operationresult.PropertyName, operationresult.Message);

        ViewBag.Message = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);

        return View(ownerVM);
    }

    [HttpGet]
    public async Task<IActionResult> GetPersonByMobile(string mobileNO, int companyId)
    {
        var owner = new PersonResult
        {
            Success = true,
            Id = -1,
            FirstName = "",
            LastName = "",
            Message = ""
        };       
        
        var owners = await _ownerPersonService.GetPersonByMobileAsync(mobileNO, companyId);
        //string message = string.Empty;
        
        //if (owners == null || owners.Count ==0)
        if (owners == null || !owners.Any())
        {
            owner.Success = false;
            owner.Message = "موردی یافت نشد";
        }
        else if (owners.Count > 1)
        {
            owner.Success = false;
            owner.Message = "بیش از 1 مورد با شماره موبایل مذکور یافت شد.";
        }
        else
        {
            owner.Success = true;
            owner.Id = owners[0].Id;
            owner.FirstName = owners[0].FirstName;
            owner.LastName = owners[0].LastName;
            owner.Message = "";
        }
        return Json(owner);
    }

    public async Task<IActionResult> Details(int personId, string? returnUrl = null)
    {
        var owner = await _ownerPersonService.GetByIdAsync(personId);
        var ownerVM = new PersonVM();
        if (owner != null)
        {
            ownerVM.Id = owner.Id;
            ownerVM.LastName = owner.LastName;
            ownerVM.FirstName = owner.FirstName;
            ownerVM.Phone = owner.Phone;
            ownerVM.BirthDate = owner.BirthDate;
            ownerVM.BirthDateFa = owner.BirthDateFa;
            ownerVM.NationalCode = owner.NationalCode;
            ownerVM.JobPlace = owner.JobPlace;
            ownerVM.Mobile = owner.Mobile;
            ownerVM.Company = owner.Company;
            ownerVM.CompanyId = owner.CompanyId;
            ownerVM.GivId = owner.GivId;
        }
        ViewBag.returnUrl = returnUrl;
        return View(ownerVM);
    }
}
public class PersonResult
{
    public bool Success { get; set; } = true;
    public int Id { get; set; } = -1;
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Message { get; set; } = "";
}
