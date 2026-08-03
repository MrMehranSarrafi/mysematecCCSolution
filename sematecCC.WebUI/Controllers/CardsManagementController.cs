using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Core.Domain.Entities;
using Core.Enums;
using Core.Services;
using Core.ServiceContracts;
using Application.DTO;

namespace SematecCC.WebUI.Controllers;

////NOTE: این attribute فقط روی کنترلر یا اکشن کار می‌کند
[Authorize]//If not logged-in, then go to login page defined in program.cs
public class CardsManagementController : MyControllersBase
{
    private readonly CardsManagementService _cardsManagerService;
    private readonly IUserContextService _userContext;
    public CardsManagementController(CardsManagementService cardsManager, IUserContextService userContext) : base(userContext)
    {
        _cardsManagerService = cardsManager;
    }
    public IActionResult Index()
    {
        return View();
    }
    public async Task<IActionResult> CardOrderIndex()
    {
        var (isAdmin, companyId, userId) = GetAuthenticationInfo();
        ViewBag.IsAdmin = isAdmin;
        ViewBag.CompanyId = companyId;
        ViewBag.UserId = userId;
        if(!HasPermission(UserPermissionsEnum.CardOrderList))
        {//راه عالی: Filter and attribute over action name
            //راه 2:
            TempData["ErrorMessage"] = "مجوز ورود به این صفحه را ندارید.";
            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }
            // اگر Referer موجود نبود، به صفحه اصلی برگردان
            return RedirectToAction("IndexNew", "Home");
        }
        var cardOrders = await _cardsManagerService.GetAllCardOrdersAsync(OrderEnum.Descending, isAdmin, companyId, userId);
        return View(cardOrders);
    }

    public async Task<IActionResult> CardOrderIndex2()
    {
        var cardOrders = await _cardsManagerService.GetAllCardOrdersAsync(OrderEnum.Descending);
        return View(cardOrders);
    }
    public async Task<IActionResult> CardOrderCreate()
    {

        var (isAdmin, companyId, _) = GetAuthenticationInfo();
        List<ComboItemsList> Companies = await _cardsManagerService.GetCompanyIDs();
        if (isAdmin == false && companyId.HasValue)
        {
            Companies = Companies.Where(c => c.Value == companyId.Value.ToString()).ToList();
        }
        List<ComboItemsList> Organizations = await _cardsManagerService.GetOrganizations();
        ViewBag.IsAdmin = isAdmin;
        ViewBag.Companies = new SelectList(Companies, "Value", "Text");
        ViewBag.Organizations = new SelectList(Organizations, "Value", "Text");

        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CardOrderCreate(CardOrder cardOrder)
    {
        //ModelState.Remove("Cards");//اکشن اصلا نیازی به Cards ندارد
        //var test = User.Identity?.Name;  یوزرنیم  Claim related to Name NOT NameIdentifier.
        var (isAdmin, companyId, _) = GetAuthenticationInfo();
        List<ComboItemsList> Companies = await _cardsManagerService.GetCompanyIDs();
        if (isAdmin == false && companyId.HasValue)
        {
            Companies = Companies.Where(c => c.Value == companyId.Value.ToString()).ToList();
        }
        List<ComboItemsList> Organizations = await _cardsManagerService.GetOrganizations();
        ViewBag.IsAdmin = isAdmin;
        ViewBag.Companies = new SelectList(Companies, "Value", "Text");
        ViewBag.Organizations = new SelectList(Organizations, "Value", "Text");
        if (!ModelState.IsValid)
        {
            return View(cardOrder);
        }

        if (!User.Identity.IsAuthenticated)
        {
            ModelState.AddModelError("", "شما وارد سیستم نشده اید");
            return View(cardOrder);
        }
        
        var result = await _cardsManagerService.CreateAsync(cardOrder/*, userId*/);
        if (result.Success)
        {
            //if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            //{
            //    return Redirect(returnUrl);// از جزِییات لیست سفارش های کارت، لینک مالک کارت آمده، 
            //}
            //else
            //{

            //TempData["Message"] = result.Message;
            TempData["SuccessMessage"] = result.Message;//layout
            return RedirectToAction(nameof(CardOrderIndex));
            //}
        }

        // نمایش پیام  خطای عمومی
        //- این خطا در بخش Validation Summary (معمولاً بالای فرم) نمایش داده می‌شود
        ModelState.AddModelError(result.PropertyName, result.Message);
        ViewBag.errs = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
        //TempData["ErrorMessage"]= ModelState.Values.SelectMany
        return View(cardOrder);
    }
    // "ConfirmOrderCard" asp-rout-OrderCardId="@c.Id">تایید</td>
    //   <td asp-action="CancelOrderCard" asp-rout-OrderCardId="@c.Id">لغو</td>
    public async Task<IActionResult> ConfirmCardOrder(int cardOrderId)
    {
        List<ComboItemsList> Organizations = await _cardsManagerService.GetOrganizations();
        ViewBag.Organizations = new SelectList(Organizations, "Value", "Text");
        var cardOrder = await _cardsManagerService.GetCardOrderAsync(cardOrderId, CardNo: "");
        return View(cardOrder);
    }
    public async Task<IActionResult> CancelCardOrder(int cardOrderId)
    {
        List<ComboItemsList> Organizations = await _cardsManagerService.GetOrganizations();
        ViewBag.Organizations = new SelectList(Organizations, "Value", "Text");
        var cardOrder = await _cardsManagerService.GetCardOrderAsync(cardOrderId, CardNo: "");
        return View(cardOrder);
    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> ConfirmCardOrder(CardOrder cardOrder, string CompanyCode)
    {
        var result = await _cardsManagerService.ConfirmCardOrder(cardOrder, CompanyCode);
        if (result.Success)
        {
            //TempData["Message"] = result.Message;
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(CardOrderIndex));
        }
        ModelState.AddModelError(result.PropertyName, result.Message);
        return View(cardOrder);

    }
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> CancelCardOrder(CardOrder cardOrder)
    {
        var result = await _cardsManagerService.CancelCardOrder(cardOrder);
        if (result.Success)
        {
            // TempData["Message"] = result.Message;
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(CardOrderIndex));
        }
        ModelState.AddModelError(result.PropertyName, result.Message);
        return View(cardOrder);

    }

    public async Task<IActionResult> CardOrderDetails(int Id, string CardNo)
    {
        if (!User.HasPermission(UserPermissionsEnum.CardOrderDetailsView))
        {
            TempData["ErrorMessage"] = "شما دسترسی مشاهده جزئیات سفارش کارت را ندارید";

            // برگشت به صفحه قبلی
            var referer = Request.Headers["Referer"].ToString();

            if (!string.IsNullOrEmpty(referer) && Url.IsLocalUrl(referer))
            {
                return LocalRedirect(referer);
            }
             
            // اگر Referer موجود نبود، به صفحه Index برگردان
            return RedirectToAction("CardOrderIndex", "CardsManagement");
        }

        List<ComboItemsList> Organizations = await _cardsManagerService.GetOrganizations();
        ViewBag.Organizations = new SelectList(Organizations, "Value", "Text");
        var cardOrder = await _cardsManagerService.GetCardOrderAsync(Id, CardNo);
        ViewBag.CardNo = CardNo;
        return View(cardOrder);
    }


    public async Task<IActionResult> CardOrderEdit(int Id)
    {
        /*
        //نکته مهم امنیتی:
        //POST/PUT/DELETE (تغییر): قطعاً باید چک کنید که کاربر مجوز ویرایش یا حذف این رکورد را دارد.
        کاملاً درست می‌گویید. این یکی از رایج‌ترین آسیب‌پذیری‌های امنیتی وب است که به آن Insecure Direct Object Reference (IDOR) یا سطح دسترسی نامناسب گفته می‌شود.

در سمت کلاینت (مرورگر)، هر چیزی که کاربر ببیند یا ارسال کند، قابل دستکاری است. اگر شما فقط به Id در URL یا فرم اعتماد کنید و چک نکنید که آیا این Id متعلق به کاربر فعلی است یا خیر، هر کسی می‌تواند داده‌های دیگران را بخواند یا تغییر دهد.

راه حل: اعتبارسنجی در سمت سرور (Backend)
شما نباید به آدرس URL اعتماد کنید. باید در کنترلر (Controller) بررسی کنید که:

آیا رکورد با این ID وجود دارد؟
آیا این رکورد متعلق به کاربر لاگین کرده فعلی است؟
        */
        var (isAdmin, companyId, _) = GetAuthenticationInfo();
        List<ComboItemsList> Companies = await _cardsManagerService.GetCompanyIDs();
        if (isAdmin == false && companyId.HasValue)
        {
            Companies = Companies.Where(c => c.Value == companyId.Value.ToString()).ToList();
        }

        ViewBag.IsAdmin = isAdmin;
        ViewBag.Companies = new SelectList(Companies, "Value", "Text");

        List<ComboItemsList> Organizations = await _cardsManagerService.GetOrganizations();
        ViewBag.Organizations = new SelectList(Organizations, "Value", "Text");

        var cardOrder = await _cardsManagerService.GetCardOrderAsync(Id, CardNo: "");

        if (cardOrder == null)
        {
            return NotFound();
        }

        return View(cardOrder);

    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CardOrderEdit(int Id, CardOrder cardOrder)
    {
        var (isAdmin, companyId, _) = GetAuthenticationInfo();
        List<ComboItemsList> Companies = await _cardsManagerService.GetCompanyIDs();
        if (isAdmin == false && companyId.HasValue)
        {
            Companies = Companies.Where(c => c.Value == companyId.Value.ToString()).ToList();
        }

        ViewBag.IsAdmin = isAdmin;
        ViewBag.Companies = new SelectList(Companies, "Value", "Text");

        List<ComboItemsList> Organizations = await _cardsManagerService.GetOrganizations();
        ViewBag.Organizations = new SelectList(Organizations, "Value", "Text");

        if (!ModelState.IsValid)
        {
            return View(cardOrder);
        }
        OperationResultDto operationResult = await _cardsManagerService.EditAsync(cardOrder);
        if (operationResult.Success)
        {
            //TempData["Message"] = operationResult.Message;
            TempData["SuccessMessage"] = operationResult.Message;//layout
            return RedirectToAction(nameof(CardOrderIndex));
        }

        //خطا در لایه های پایین هنگام ادیت
        ModelState.AddModelError(operationResult.PropertyName, operationResult.Message);

        ViewBag.errs = ModelState.Values.SelectMany(x => x.Errors);
        TempData["ErrorMessage"] = ModelState.Values.SelectMany(x => x.Errors);
        return View(cardOrder);
    }
    public async Task<IActionResult> CardOrderSearch(string companyCodeOrCardNo)//companyCodeOrCardNo
    {
        var (isAdmin, currentUserCompanyId, currentUserId) = GetAuthenticationInfo();
        //List<CardOrder> list = await _cardsManager.CardOrderSearch(companyId);
        List<CardOrder> list = await _cardsManagerService.CardOrderSearch(isAdmin, currentUserCompanyId, currentUserId, companyCodeOrCardNo);

        return PartialView("_CardOrderRowsPartialView", list);
    }
    public async Task<IActionResult> CardOrderDetailsSearch(int cardOrderId, string cardNoInitials)//ابتدای شماره کارت
    {
        List<Card> list = await _cardsManagerService.CardOrderDetailsSearch(cardOrderId, cardNoInitials);
        return PartialView("_CardRowsPartialView", list);//لیست کارت ها
    }

    public async Task<IActionResult> GetCardtransactions(int cardId)
    {
        var transactions = await _cardsManagerService.GetCardtransactions(cardId);
        var list = transactions.Select(t => new CardTransactionVM
        {
            Id = t.Id,
            DateCreated = t.DateCreated,
            Amount = t.Amount,
            RemainedAmount = t.RemainedAmount,
            CardId = t.CardId,
            Status = t.Status,
            Sign = t.CardTransactionType.Sign,
            CardTransactionTypeTitle = t.CardTransactionType.Title,
            Description = t.Description,
            ProviderId = t.ProviderId,
            TerminalId = t.TerminalId,
            BranchId = t.BranchId,

        });

        return PartialView("_CardTransactaionRowsPartialView", list);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DisableCard(int cardId)
    {
        var result = new OperationResultDto();

        result = await _cardsManagerService.DisableCard(cardId);

        return Json(result);

    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EnableCard(int cardId)
    {
        var result = new OperationResultDto();

        result = await _cardsManagerService.EnableCard(cardId);

        return Json(result);

    }
    public async Task<IActionResult> DisableCardOrder(int cardOrderId)
    {
        var result = await _cardsManagerService.DisableCardOrder(cardOrderId);
        //TempData["Message"] = result.Message;
        if (result.Success)
        {
            TempData["SuccessMessage"] = result.Message;
        }
        else
        {
            TempData["ErrorMessage"] = result.Message;
        }


        return RedirectToAction(nameof(CardOrderIndex));

    }
    public async Task<IActionResult> EnableCardOrder(int cardOrderId)
    {
        //؟ در کدام لایه
        //if(!_userContext.HasPermission(UserPermissionsEnum.CardOrderEnable))
        //{

        //}
        var result = await _cardsManagerService.EnableCardOrder(cardOrderId);
        //TempData["Message"] = result.Message;
        if (result.Success)
            TempData["SuccessMessage"] = result.Message;
        else
            TempData["ErrorMessage"] = result.Message;
        return RedirectToAction(nameof(CardOrderIndex));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> IncrementCardCredit([FromBody] CardCreditVM Credit)//
    {
        var result = new OperationResultDto();
        var (isAdmin, currentUserCompanyId, currentUserId) = GetAuthenticationInfo();
        //result = await _cardsManager.IncrementCardCredit(cardId, amount, description,  isAdmin, currentUserCompanyId, currentUserId);
        result = await _cardsManagerService.IncrementCardCredit(Credit.CardId, Credit.Amount, Credit.Description, isAdmin, currentUserCompanyId, currentUserId);
        return Json(result);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DecreaseCardCredit([FromBody] CardCreditVM Credit)
    {
        var result = new OperationResultDto();
        var (isAdmin, currentUserCompanyId, currentUserId) = GetAuthenticationInfo();
        result = await _cardsManagerService.DecreaseCardCredit(Credit.CardId, Credit.Amount, Credit.Description, isAdmin, currentUserCompanyId, currentUserId);
        return Json(result);
    }



    [Authorize]
    public async Task<IActionResult> CardsList()
    {
        var (isAdmin, currentUserCompanyId, currentUserId) = GetAuthenticationInfo();
        List<ComboItemsList> Companies = await _cardsManagerService.GetCompanyIDs(isAdmin, currentUserCompanyId, currentUserId);
        ViewBag.Companies = new SelectList(Companies, "Value", "Text");

        List<ComboItemsList> Organizations = await _cardsManagerService.GetOrganizations();
        ViewBag.Organizations = new SelectList(Organizations, "Value", "Text");
        
        return View();
    }
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CardsListSearchCards(string CardNo, string Serial, int? CompanyId, int? OrganizationId)
    {
        var (isAdmin, currentUserCompanyId, currentUserId) = GetAuthenticationInfo();
        //List<CardDisplayDto> list = await _cardsManager.GetCardsAsync(CardNo, Serial, CompanyId, OrganizationId);
        List<CardDisplayDto> list = await _cardsManagerService.GetCardsAsync(isAdmin, currentUserCompanyId, currentUserId, CardNo, Serial, CompanyId, OrganizationId);
        return PartialView("_CardRows2PartialView", list);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]// پیش فررض هست
    public async Task<IActionResult> SetCardOwner(int cardId, int ownerPersonId)
    {
        var result = new OperationResultDto();

        result = await _cardsManagerService.SetCardOwner(cardId, ownerPersonId);

        return Json(result);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetCardExpireDateFa(int cardId, string expireDateFa)
    {
        var result = new OperationResultDto();

        result = await _cardsManagerService.SetCardExpireDateFa(cardId, expireDateFa);

        return Json(result);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetAllCardsExpireDateFa(int cardOrderId, string expireDateFa)
    {
        var result = new OperationResultDto();

        result = await _cardsManagerService.SetAllCardsExpireDateFa(cardOrderId, expireDateFa);

        return Json(result);
    }
    public async Task<IActionResult> ExportExcel(int cardOrderId, string? cardNo)//Excel نیاز به نصب   پکیجی  دارد
    {
        MemoryStream memoryStream = await _cardsManagerService.GetCardsExcel(cardOrderId, cardNo);
        return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "cards.xlsx");

    }
    public async Task<IActionResult> ExportCsv(int cardOrderId, string? cardNo)//csv نیاز به نصب هیچ پکیجی ندارد
    {
        MemoryStream memoryStream = await _cardsManagerService.GetCardsCsv(cardOrderId, cardNo);
        return File(memoryStream, "text/csv", "report.csv");
    }
}
