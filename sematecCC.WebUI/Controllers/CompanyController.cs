using Application.DTO;
using Core.Domain.Entities;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SematecCC.WebUI.Controllers;

[Authorize]
//[AllowAnonymous]
public class CompanyController : Controller
{
    private readonly CompanyService _companyService;

    public CompanyController(CompanyService companyService)
    {
        _companyService = companyService;

    }
    public async Task<IActionResult> Index()
    {
        try
        {                
            List<Company> companies = await _companyService.GetAllAsync();
            int row = 1;
            companies.ForEach(c => c.RowNo = row++);
            //companies.ForEach(c => { c.RowNo = row++; });

            return View(companies);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"خطا در دریافت اطلاعات: {ex.Message}";
            return View(new List<Organization>());
        }
    }
    public async Task<IActionResult> Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Company company)
    {
        if (!ModelState.IsValid)
        {
            return View(company);
        }

        OperationResultDto result = await _companyService.AddAsync(company);
        if (result.Success)
        {
            //TempData["Message"] = result.Message;
            TempData["SuccessMessage"] = result.Message; 
            return RedirectToAction(nameof(Index));
        }


        ModelState.AddModelError(result.PropertyName, result.Message);
        ViewBag.errs = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
        return View(company);
    }


    public async Task<IActionResult> Edit(int Id)
    {
        var company = await _companyService.GetByIdAsync(Id);
        return View(company);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int Id, Company company)
    {
        if (!ModelState.IsValid)
        {
            return View(company);
        }

        OperationResultDto operationresult = await _companyService.EditAsync(company);
        if (operationresult.Success)
        {
            //TempData["Message"] = operationresult.Message;
            TempData["SuccessMessage"] = operationresult.Message;
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(operationresult.PropertyName, operationresult.Message);
        ViewBag.Message = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
        return View(company);
    }


    public async Task<IActionResult> Search(string searchItems)
    {
        //var users = await new UserService().GetUsersAsync(searchItems);
        var companies = await _companyService.GetAllFilteredAsync(searchItems);
        return Json(companies);
        //return PartialView(organizations);
    }

    [HttpGet]
    public async Task<IActionResult> SearchCompanies(string term)
    {
        var companiesCombo = await _companyService.SearchCompanies(term);
        return Json(companiesCombo);
    }

}
