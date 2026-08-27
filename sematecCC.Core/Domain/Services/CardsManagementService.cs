using Application.DTO;
using Core.Domain.Entities;
using Core.Domain.RepositoryContracts;
using Domain.Enums;
using Domain.Helpers;
using Domain.ServiceContracts;
using System.Text;

namespace Domain.Services;

public class CardsManagementService : MyServicesBase
{
    private readonly ICardsManagementRepo _cardsManagementRepo;
    private readonly IUserContextService _userContext;


    public CardsManagementService(ICardsManagementRepo cardsManagementRepo, IUserContextService userContext)
    {
        _cardsManagementRepo = cardsManagementRepo;
        _userContext = userContext;
    }

    public async Task<OperationResultDto> ConfirmCardOrder(CardOrder cardOrder, string CompanyCode)
    {
        var result = new OperationResultDto();
       
        ConfirmCardOrderResult enumValue = (ConfirmCardOrderResult)(-1);
        try
        {
            if (!_userContext.IsAuthenticated())
                return Fail("هنوز وارد سیستم نشده اید.");

            cardOrder.UserIdChanged = _userContext.GetCurrentUserId().Value;
            bool? isAdmin = _userContext.IsUserAdmin();

            List<CardDto> cards = await GenerateCardsList(cardOrder.Id, CompanyCode, cardOrder.Tedad);
            if (cardOrder.ExpireDayNumber != null)
            {
                cardOrder.ExpireDate = DateTime.Now.AddDays(cardOrder.ExpireDayNumber.Value);
                cardOrder.ExpireDateFa = cardOrder.ExpireDate.ToPersian();

            }
            int resultInt = await _cardsManagementRepo.ConfirmCardOrder(cardOrder, cards);
            await _cardsManagementRepo.SaveChangesAsync();
            if (resultInt == 0)
            {
                result.Success = true;
                result.Message = "سفارش کارت با موفقیت تایید شد.";
            }
            else
            {
                result.Success = false;
                enumValue = (ConfirmCardOrderResult)resultInt;
                result.Message = enumValue.GetDescriptionAttributeValue();
            }
        }
        catch (Exception ex)
        {
            result = OperationResultHelper.Fail($" تایید سفارش کارت با خطا مواجه شد. \n  {ex.Message} \n {enumValue.GetDisplayAttributeValue()}");
        }
        return result;
    }

    Random rnd = new Random();
    private async Task<List<CardDto>> GenerateCardsList(int cardOrderId, string companyCode, int tedad)
    {
        List<CardDto> list = new List<CardDto>();
        Random rnd = new Random();

        string maxSerialNo = await _cardsManagementRepo.GetCardMaxSerialNo(companyCode);
        int iSerialNo = string.IsNullOrWhiteSpace(maxSerialNo) ? 0 : int.Parse(maxSerialNo);
        string serialNo = iSerialNo.ToString("D7"); // همیشه به صورت 7 رقمی

        for (int i = 1; i <= tedad; i++)
        {
            CardDto card = new CardDto();
            serialNo = (iSerialNo + i).ToString("D7");
            card.SerialNo = serialNo;

            StringBuilder cardNoBuilder = new StringBuilder(companyCode);
            cardNoBuilder.Append(serialNo);
            cardNoBuilder.Append(rnd.Next(0, 9));
            card.CardNo = cardNoBuilder.ToString();

            card.Password = rnd.Next(10000, 99999).ToString();//GeneratePassword();
            card.RowNo = i;

            list.Add(card);
        }
        return list;
    }
    private string GeneratePassword()
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz!@#$%*";//l ال را حذف کردم
        var digits = "0123456789";
        StringBuilder sb = new StringBuilder();

        // اضافه کردن 3 کاراکتر از chars
        Random rnd = new Random(); // بهتره Random رو اینجا تعریف کنی
        for (int i = 0; i < 3; i++)
        {
            sb.Append(chars[rnd.Next(chars.Length)]);
        }

        // اضافه کردن 2 رقم از digits
        for (int i = 0; i < 2; i++)
        {
            sb.Append(digits[rnd.Next(digits.Length)]);
        }

        return sb.ToString();
    }


    public async Task<OperationResultDto> CancelCardOrder(CardOrder cardOrder)
    {
        var result = new OperationResultDto();
        try
        {

            if (!_userContext.IsAuthenticated())
                return Fail("هنوز وارد سیستم نشده اید.", "");

            cardOrder.UserIdChanged = _userContext.GetCurrentUserId().Value;

            await _cardsManagementRepo.CancelCardOrder(cardOrder);
            await _cardsManagementRepo.SaveChangesAsync();
            result.Success = true;
            result.Message = "سفارش کارت با موفقیت لغو شد.";

        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = $" لغو سفارش کارت با خطا مواجه شد. \n  {ex.Message}";

        }
        return result;
    }
    public async Task<OperationResultDto> CreateAsync(CardOrder cardOrder/*, int userId*/)
    {
        var result = new OperationResultDto();
        try
        {
            if (!_userContext.IsAuthenticated())
                return Fail("هنوز وارد سیستم نشده اید.", "");

            cardOrder.UserIdCreated = _userContext.GetCurrentUserId().Value;
            cardOrder.DateCreated = DateTime.Now;
            cardOrder.Status = CardOrderStatus.NewOrInitial;

            await _cardsManagementRepo.CreateAsync(cardOrder/*, userId*/);
            await _cardsManagementRepo.SaveChangesAsync();
            result.Success = true;
            result.Message = "اطلاعات سفارش شماره کارت با موفقیت ثبت شد.";
        }
        catch (Exception ex)
        {
            return Fail($"در ثبت اطلاعات خطا رخ داد.\n {ex.Message}", "");

        }
        return result;
    }

    public async Task<List<CardOrder>> GetAllCardOrdersAsync(OrderEnum order)
    {
        return await _cardsManagementRepo.GetAllCardOrdersAsync(order);
    }
    public async Task<List<CardOrder>> GetAllCardOrdersAsync(OrderEnum order, bool? isAdmin, int? companyId, int? userId)
    {
        return await _cardsManagementRepo.GetAllCardOrdersAsync(order, isAdmin, companyId, userId);
    }

    public async Task<CardOrder> GetCardOrderAsync(int cardOrderId, string CardNo)
    {
        return await _cardsManagementRepo.GetCardOrderAsync(cardOrderId, CardNo);
    }

    public async Task<OperationResultDto> EditAsync(CardOrder cardOrder)
    {
        var result = new OperationResultDto();
        try
        {
            if (!_userContext.IsAuthenticated())
                return Fail("هنوز وارد سیستم نشده اید.", "");

            cardOrder.UserIdChanged = _userContext.GetCurrentUserId().Value;

            await _cardsManagementRepo.EditAsync(cardOrder);
            await _cardsManagementRepo.SaveChangesAsync();
            result.Success = true;
            result.Message = "اطلاعات سفارش کارت با موفقیت ویرایش شد.";

        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            //throw;
        }
        return result;
    }

    public async Task<List<CardOrder>> CardOrderSearch(string companyCodeOrCardNo)
    {
        return await _cardsManagementRepo.CardOrderSearch(companyCodeOrCardNo);
    }
    public async Task<List<CardOrder>> CardOrderSearch(bool? isAdmin, int? currentUserCompanyId, int? currentUserId, string companyCodeOrCardNo)
    {
        return await _cardsManagementRepo.CardOrderSearch(isAdmin, currentUserCompanyId, currentUserId, companyCodeOrCardNo);
    }
    public async Task<List<Card>> CardOrderDetailsSearch(int cardOrderId, string cardNoInitials)
    {
        return await _cardsManagementRepo.CardOrderDetailsSearch(cardOrderId, cardNoInitials);
    }

    public async Task<List<CardTransaction>> GetCardtransactions(int cardId)
    {
        return await _cardsManagementRepo.GetCardtransactions(cardId);
    }

    public async Task<List<ComboItemsList>> GetCompanyIDs()
    {
        return await _cardsManagementRepo.GetCompanyIDs();
    }
    public async Task<List<ComboItemsList>> GetCompanyIDs(bool? isAdmin, int? currentUserCompanyId, int? UserId)
    {
        return await _cardsManagementRepo.GetCompanyIDs(isAdmin, currentUserCompanyId, UserId);
    }
    public async Task<List<ComboItemsList>> GetOrganizations()
    {
        return await _cardsManagementRepo.GetOrganizations();
    }
    public async Task<OperationResultDto> DisableCard(int cardId)
    {
        var result = new OperationResultDto();
        try
        {
            if (!_userContext.IsAuthenticated())
                return Fail("هنوز وارد سیستم نشده اید.", "");

            if (!_userContext.HasPermission(UserPermissionsEnum.CardOrderDisableTheCard))
            {
                result.Success = false;
                result.Message = "مجوز ندارید";
                return result;
            }
            var userId = _userContext.GetCurrentUserId().Value;

            var card = await _cardsManagementRepo.GetCardAsync(cardId);

            if (card == null)
            {
                return Fail("کارت وجود ندارد.", "");
                
            }
            if (!card.IsActive)
            {
                return Fail("کارت از قبل غیرفعال بود", "IsActive");
                
            }
            await _cardsManagementRepo.DisableCard(cardId, userId);
            await _cardsManagementRepo.SaveChangesAsync();
            return OperationResultHelper.Success("کارت با موفقیت غیرفعال شد.");

        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;

        }
        return result;
    }
    public async Task<OperationResultDto> EnableCard(int cardId)
    {
        var result = new OperationResultDto();
        if (!_userContext.IsAuthenticated())
            return Fail("هنوز وارد سیستم نشده اید.");
        if (!_userContext.HasPermission(UserPermissionsEnum.CardOrderEnableTheCard))
        {
            result.Success = false;
            result.Message = "مجوز ندارید";
            return result;
        }

        var userId = _userContext.GetCurrentUserId().Value;

        try
        {
            var card = await _cardsManagementRepo.GetCardAsync(cardId);
            if (card == null)
            {
                return Fail("کارت وجود ندارد");
            }
            if (card.IsActive)
            {
                return Fail("کارت از قبل فعال بود", "IsActive");
            }
            await _cardsManagementRepo.EnableCard(cardId, userId);
            await _cardsManagementRepo.SaveChangesAsync();

            result.Success = true;
            result.Message = "کارت با موفقیت فعال شد.";

        }
        catch (Exception ex)
        {
            return Fail(ex.Message + "\n" + " خطا رخ داد");
        }
        return result;
    }

    public async Task<List<CardDisplayDto>> GetCardsAsync(string CardNo, string Serial, int? CompanyId, int? OrganizationId)
    {
        return await _cardsManagementRepo.GetCardsAsync(CardNo, Serial, CompanyId, OrganizationId);
    }
    public async Task<List<CardDisplayDto>> GetCardsAsync(bool? isAdmin, int? currentUserCompanyId, int? currentUserId, string CardNo, string Serial, int? CompanyId, int? OrganizationId)
    {
        return await _cardsManagementRepo.GetCardsAsync(isAdmin, currentUserCompanyId, currentUserId, CardNo, Serial, CompanyId, OrganizationId);
    }

    public async Task<OperationResultDto> SetCardOwner(int cardId, int ownerPersonId)
    {
        var result = new OperationResultDto();
        if(!_userContext.HasPermission(UserPermissionsEnum.CardOrderSetOwnerOfTheCard))
        {
            return Fail("مجوز ندارید ");
        }
        var currentUserId = _userContext.GetCurrentUserId().Value;

        try
        {
            var card = await _cardsManagementRepo.GetCardAsync(cardId);
            if (card == null)
            {
                return Fail("کارت وجود ندارد");
            }
            if (card.OwnerPersonId.HasValue)
            {
                return Fail("مالک کارت از قبل مشخص شده بود ", "OwnerPersonId");
            }
            if (!card.IsActive)
            {
                return Fail($"کارت شماره {card.CardNo} غیر فعال می باشد.", "IsActive");
            }
            if (card.ExpireDate.HasValue && card.ExpireDate < DateTime.Today)
            {
                return Fail($"کارت شماره {card.CardNo} منقضی شده است.", "ExpireDateFa");
            }
            var cardOrder = await _cardsManagementRepo.GetCardOrderAsync(card.CardOrderId, CardNo: "");
            if (!cardOrder.IsActive)
            {
                return Fail($"کارت شماره {card.CardNo} جزء سفارش کارت شماره{cardOrder.Id} می باشد که غیرفعال است.", "IsActive");
            }
            await _cardsManagementRepo.SetCardOwner(cardId, ownerPersonId, currentUserId);
            await _cardsManagementRepo.SaveChangesAsync();

            result.Success = true;
            result.Message = "  مالک کارت با موفقیت تعیین شد.  ";

        }
        catch (Exception ex)
        {
            return Fail(ex.Message + "\n" + " خطا رخ داد");

        }
        return result;
    }
    public async Task<OperationResultDto> SetCardExpireDateFa(int cardId, string expireDateFa)
    {
        var result = new OperationResultDto();
        if (!_userContext.IsAuthenticated())
            return Fail("هنوز وارد سیستم نشده اید.");

        var currentUserId = _userContext.GetCurrentUserId().Value;
        var ExpireDate = expireDateFa.ToMiladi();
        if (ExpireDate == null || !ExpireDate.HasValue || ExpireDate.Value.Date.AddDays(1) < DateTime.Now)
        {
            return Fail("فرمت تاریخ وارد شده نادرست است یا تاریخ آن گذشته است. ");
        }
        try
        {
            var card = await _cardsManagementRepo.GetCardAsync(cardId);
            if (card == null)
            {
                return Fail("کارت وجود ندارد");
            }

            if (!card.IsActive)
            {
                return Fail($"کارت شماره {card.CardNo} غیر فعال می باشد.", "IsActive");
            }

            if (!card.CardOrder.IsActive)
            {
                return Fail($"کارت شماره {card.CardNo} جزء سفارش کارت شماره{card.CardOrder.Id} می باشد که غیرفعال است.", "IsActive");
            }

            await _cardsManagementRepo.SetCardExpireDateFa(cardId, expireDateFa, currentUserId);
            await _cardsManagementRepo.SaveChangesAsync();

            result.Success = true;
            result.Message = $"   تاریخ انقضای کارت شماره {card.CardNo} با موفقیت تعیین شد  ";

        }
        catch (Exception ex)
        {
            return Fail(ex.Message + "\n" + " خطا رخ داد");
        }
        return result;
    }

    public async Task<Card?> GetCardAsync(string cardNo)
    {
        return await _cardsManagementRepo.GetCardAsync(cardNo);
    }
    public async Task<Card?> GetCardAsync(string cardNumber, string password)
    {
        return await _cardsManagementRepo.GetCardAsync(cardNumber, password);
    }
    public async Task<OperationResultDto> IncrementCardCredit(int cardId, decimal amount, string? Description, bool? isAdmin, int? currentCompanyId, int? currentUserId)
    {
        var result = new OperationResultDto();
        try
        {
            if (!_userContext.IsAuthenticated())
                return Fail("شما هنوز وارد سیستم نشده اید.", "");
            var card = await _cardsManagementRepo.GetCardAsync(cardId);
            if (card == null)
            {
                return Fail($"کارت شماره {card.CardNo} پیدا نشد.", "");
            }
            if (!card.IsActive)
            {
                return Fail($"کارت شماره {card.CardNo} غیر فعال می باشد.", "IsActive");
            }
            var cardOrder = await _cardsManagementRepo.GetCardOrderAsync(card.CardOrderId, CardNo: "");
            if (!cardOrder.IsActive)
            {
                return Fail($"کارت شماره {card.CardNo} جزء سفارش کارت شماره{cardOrder.Id} می باشد که غیرفعال است.", "IsActive");
            }
            if (card.ExpireDate.HasValue && card.ExpireDate < DateTime.Today)
            {
                return Fail($"کارت شماره {card.CardNo} منقضی شده است.", "ExpireDateFa");
            }
            //مجوز نداشته باشد، نمی بیند. دیگه داشتن مجوز را اینجا چک نمی کنیم.
            await _cardsManagementRepo.IncrementCardCredit(cardId, amount, Description, currentUserId);
            await _cardsManagementRepo.SaveChangesAsync();
            result.Success = true;
            result.Message = $"اعتبار کارت شماره {card.CardNo} با موفقیت افزایش  یافت.";
        }
        catch (Exception ex)
        {
            return Fail($"حطا رخ داد \n {ex.Message}");
        }
        return result;
    }
    public async Task<OperationResultDto> DecreaseCardCredit(int cardId, decimal amount, string? Description, bool? isAdmin, int? currentCompanyId, int? currentUserId)
    {
        var result = new OperationResultDto();
        try
        {
            if (!_userContext.IsAuthenticated())
                return Fail("شما هنوز وارد سیستم نشده اید.", "");
            var card = await _cardsManagementRepo.GetCardAsync(cardId);
            if (card == null)
            {
                return Fail($"کارت شماره {card.CardNo} پیدا نشد.", "");
            }
            if (!card.IsActive)
            {
                return Fail($"کارت شماره {card.CardNo} غیر فعال می باشد.", "IsActive");
            }
            if (card.ExpireDate.HasValue && card.ExpireDate < DateTime.Today)
            {
                return Fail($"کارت شماره {card.CardNo} منقضی شده است.", "ExpireDateFa");
            }
            var cardOrder = await _cardsManagementRepo.GetCardOrderAsync(card.CardOrderId, CardNo: "");
            if (!cardOrder.IsActive)
            {
                return Fail($"کارت شماره {card.CardNo} جزء سفارش کارت شماره{cardOrder.Id} می باشد که غیرفعال است.", "IsActive");
            }
            if (card.RemainedAmount - amount < 0)
            {
                return Fail($"فقط {card.RemainedAmount} اعتبار دارد", "RemainedAmount");
            }
            //مجوز نداشته باشد، نمی بیند. دیگه داشتن مجوز را اینجا چک نمی کنیم.
            await _cardsManagementRepo.DecreaseCardCredit(cardId, amount, Description, currentUserId);
            await _cardsManagementRepo.SaveChangesAsync();
            result.Success = true;
            result.Message = $"اعتبار کارت شماره {card.CardNo} با موفقیت کاهش  یافت.";

        }
        catch (Exception ex)
        {
            return Fail($"حطا رخ داد \n {ex.Message}");
        }
        return result;
    }

    public async Task<MemoryStream> GetCardsExcel(int cardOrderId, string? cardNo)
    {
        return await _cardsManagementRepo.GetCardsExcel(cardOrderId, cardNo);
       
    }  
    public async Task<MemoryStream> GetCardsCsv(int cardOrderId, string? cardNo)
    {
        return await _cardsManagementRepo.GetCardsCsv( cardOrderId, cardNo);        
    }
    public async Task<OperationResultDto> DisableCardOrder(int cardOrderId)
    {
        var result = new OperationResultDto();
        if (!_userContext.HasPermission(UserPermissionsEnum.CardOrderDisable))
        {
            result.Success = false;
            result.Message = "مجوز ندارید";
            return result;
        }
        try
        {
            await _cardsManagementRepo.DisableCardOrder(cardOrderId);
            await _cardsManagementRepo.SaveChangesAsync();
            result.Success = true;
            result.Message = "سفارش کارت با موفقیت غیرفعال شد";
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
        }
        return result;
    }
    public async Task<OperationResultDto> EnableCardOrder(int cardOrderId)
    {

        var result = new OperationResultDto();
        if (!_userContext.HasPermission(UserPermissionsEnum.CardOrderEnable))
        {
            result.Success = false;
            result.Message = "مجوز ندارید";
            return result;
        }
        try
        {
            await _cardsManagementRepo.EnableCardOrder(cardOrderId);
            await _cardsManagementRepo.SaveChangesAsync();
            result.Success = true;
            result.Message = "سفارش کارت با موفقیت فعال شد";
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
        }
        return result;
    }

    public async Task<OperationResultDto> SetAllCardsExpireDateFa(int cardOrderId, string expireDateFa)
    {
        var result = new OperationResultDto();
        var currentUserId = _userContext.GetCurrentUserId().Value;
        var expireDate = expireDateFa.ToMiladi();
        if (expireDate == null || !expireDate.HasValue || expireDate.Value.Date < DateTime.Now.Date)//DateTime.Now.Date== DateTime.Today
        {
            return Fail("فرمت تاریخ وارد شده نادرست است یا تاریخ آن گذشته است. ");
        }
        if (!_userContext.HasPermission(UserPermissionsEnum.CardOrderSetAllCardsExpireDate))
        {
            return Fail("شما مجوز ندارید");
        }
        try
        {
            await _cardsManagementRepo.SetAllCardsExpireDateFa(cardOrderId, expireDateFa, expireDate.Value, currentUserId);
            await _cardsManagementRepo.SaveChangesAsync();

            result.Success = true;
            result.Message = $" تاریخ انقضای تمام کارت ها با موفقیت تعیین شد  ";
        }
        catch (Exception ex)
        {
            result = Fail(ex.Message + "\n" + " خطا رخ داد");

        }
        return result;
    }
}
