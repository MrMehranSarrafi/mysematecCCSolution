using CardNoGenerator.Core.Helpers;

namespace CardNoGenerator.Core.Services;

public class PersonService
{
    private readonly IPersonRepo _ownerRepo;
    private readonly IUserContextService _userContext;
    public PersonService(IPersonRepo ownerRepo, IUserContextService userContext)
    {
        _ownerRepo = ownerRepo;
        _userContext = userContext;
    }

    public async Task<OperationResultDto> CreateAsync(Person owner)
    {
        var result = new OperationResultDto();
        if (!_userContext.IsAuthenticated())
            return OperationResultHelper.Fail("هنوز وارد سیستم نشده اید.");

        //owner.UserIdCreated = (_userContext.GetCurrentUserId()).Value;  در کنترلر
        try
        {
            var p = await _ownerRepo.GetByParamsAsync(owner.Mobile, owner.GivId, owner.CompanyId, owner.Id);
            if (p != null)
            {
                return OperationResultHelper.Fail(" شخص با این شماره موبایل و کد گیو در این شرکت، قبلا تعریف شده است", nameof(Person.Mobile));

            }
            //if (!string.IsNullOrWhiteSpace(owner.NationalCode))
            //{
            //    var o2 = await _ownerRepo.GetByNationalCodeAsync(owner.NationalCode, 0);
            //    if (o2 != null)
            //    {
            //        //result.Success = false;
            //        //result.Message = " شخص با این کد ملی قبلا تعریف شده است";
            //        //result.PropertyName = nameof(Owner.NationalCode);
            //        //return result;
            //        return OperationResultHelper.Fail("  شخص با این کد ملی قبلا تعریف شده است", nameof(Person.NationalCode));

            //    }

            //}
            owner.DateCreated = DateTime.Now;
            await _ownerRepo.CreateAsync(owner);
            await _ownerRepo.SaveChangesAsync();
            result.Success = true;
            result.Message = "اطلاعات شخص مالک با موفقیت ثبت شد.";
        }
        catch (Exception ex)
        {
            //result.Success = false;
            //result.Message = $"در ثبت اطلاعات خطا رخ داد.\n {ex.Message}";
            //result.PropertyName = "";
            return Fail($"در ثبت اطلاعات خطا رخ داد.\n {ex.Message}", "");
        }
        return result;
    }

    public async Task<Person> GetByIdAsync(int Id)
    {
        return await _ownerRepo.GetByIdAsync(Id);
    }
    public async Task<List<Person>> GetAllAsync()
    {
        return await _ownerRepo.GetAllAsync();
    }

    public async Task<List<Person>> GetAllAsync(bool? isAdmin, int? currentCompanyId, int? currentUserId)
    {
        if (isAdmin == true)
            return await _ownerRepo.GetAllAsync();
        else if (currentCompanyId.HasValue)
            return await _ownerRepo.GetAllAsyncByCompanyId(currentCompanyId.Value);
        else
            return new List<Person>();

    }
    public async Task<OperationResultDto> EditAsync(Person owner)
    {
        var result = new OperationResultDto();
        if (!_userContext.IsAuthenticated())
            return OperationResultHelper.Fail("هنوز وارد سیستم نشده اید.");

        owner.UserIdChanged = (_userContext.GetCurrentUserId()).Value;
        try
        {

            var ExistingOwner = await _ownerRepo.GetByParamsAsync(owner.Mobile, owner.GivId, owner.CompanyId, owner.Id);
            if (ExistingOwner != null)
            {
                return OperationResultHelper.Fail(" شخص با این شماره موبایل و کد گیو در این شرکت، قبلا تعریف شده است", nameof(Person.Mobile));

            }
            //if (!string.IsNullOrWhiteSpace(owner.NationalCode))
            //{
            //    var ExistingOwner2 = await _ownerRepo.GetByNationalCodeAsync(owner.Mobile, owner.Id);
            //    if (ExistingOwner != null)
            //    {
            //        return Fail(" شخص با این کد ملی قبلا تعریف شده است", nameof(Person.NationalCode));
            //    }
            //}
            owner.DateCreated = DateTime.Now;
            await _ownerRepo.EditAsync(owner);
            await _ownerRepo.SaveChangesAsync();
            result.Success = true;
            result.Message = "اطلاعات شخص با موفقیت ویرایش شد.";

        }
        catch (Exception ex)
        {
            return Fail("خطا رخ داد." + "\n" + ex.Message, "");
            //throw;
        }
        return result;
    }

    public async Task<List<Person>> GetAllFilteredAsync(string searchItems, bool? isAdmin, int? currentCompanyId, int? currentUserId)
    {
        if (isAdmin == true)
            return await _ownerRepo.GetAllFilteredAsync(searchItems);
        else if (currentCompanyId.HasValue)
            return await _ownerRepo.GetAllFilteredAsync(searchItems, currentCompanyId.Value);
        else
            return new List<Person>();
        
    }

    public async Task<List<Person>> GetAllFilteredAsync(string searchItems)
    {
        return await _ownerRepo.GetAllFilteredAsync(searchItems);
    }
    public async Task<List<Person>> GetPersonByMobileAsync(string mobileNO, int companyId)
    {
        return await _ownerRepo.GetPersonByMobileAsync(mobileNO, companyId);
    }
    private OperationResultDto Fail(string message, string properyName)
    {
        return new OperationResultDto()
        {
            Success = false,
            Message = message,
            PropertyName = properyName

        };
    }
}
