using CardNoGenerator.Core.Helpers;


namespace CardNoGenerator.Core.Services;

public class CompanyService
{
    private readonly ICompanyRepo _companyRepo;
    private readonly IUserContextService _userContext;

    public CompanyService(ICompanyRepo companyRepo, IUserContextService userContext)
    {
        _companyRepo = companyRepo;
        _userContext = userContext;
    }
    public async Task<OperationResultDto> AddAsync(Company company)
    {
        var result = new OperationResultDto();
        if (!_userContext.IsAuthenticated())
            return OperationResultHelper.Fail("هنوز وارد سیستم نشده اید.");
        var userId = _userContext.GetCurrentUserId();
        try
        {
            var chekCompany = await _companyRepo.GetByNameAsync(company.CompanyName.Trim(), company.Id);
            if (chekCompany != null)
            {
                return OperationResultHelper.Fail(" شرکت با این نام قبلا تعریف شده است", nameof(Company.CompanyName));
            }
            chekCompany = await _companyRepo.GetByCodeAsync(company.CompanyCode.Trim(), company.Id);
            if (chekCompany != null)
            {
                return OperationResultHelper.Fail(" شرکت با این کد قبلا تعریف شده است", nameof(Company.CompanyCode));
            }
            company.UserIdCreated = userId.Value;
            company.DateCreated = DateTime.Now;

            await _companyRepo.AddAsync(company);
            await _companyRepo.SaveChangesAsync();
            result = OperationResultHelper.Success("اطلاعات شرکت با موفقیت ثبت شد.");

        }
        catch (Exception ex)
        {
            return OperationResultHelper.Fail($"در ثبت اطلاعات خطا رخ داد.\n {ex.Message}");
        }
        return result;
    }
    public async Task<Company> GetByIdAsync(int Id)
    {
        return await _companyRepo.GetByIdAsync(Id);
    }
    public async Task<List<Company>> GetAllAsync()
    {
        return await _companyRepo.GetAllAsync();
    }
    public async Task<List<ComboItemsList>> GetComboCompanies()
    {
        return await _companyRepo.GetComboCompaniesAsync();
    }
    public async Task<List<ComboItemsList>> GetComboAllowedCompanies(bool? IsAdmin, int? CurrentCompanyId, int? CurrentUserId)
    {
        if (IsAdmin==true)
            return  await _companyRepo.GetAllComboAsync();
        else if(IsAdmin.HasValue && CurrentCompanyId.HasValue)
        {
            var com = await _companyRepo.GetByIdComboAsync(CurrentCompanyId.Value);
            return new List<ComboItemsList> { com };
        }
        else return new List<ComboItemsList>();
    }
    public async Task<OperationResultDto> EditAsync(Company company)
    {
        var result = new OperationResultDto();
        if (!_userContext.IsAuthenticated())
            return OperationResultHelper.Fail("هنوز وارد سیستم نشده اید.");
        var userId = _userContext.GetCurrentUserId();
        try
        {
            var chekCompany = await _companyRepo.GetByNameAsync(company.CompanyName.Trim(), company.Id);
            if (chekCompany != null)
            {
                return OperationResultHelper.Fail(" شرکت با این نام قبلا تعریف شده است", nameof(Company.CompanyName));
            }
            var chekCompany2 = await _companyRepo.GetByCodeAsync(company.CompanyCode.Trim(), company.Id);
            if (chekCompany2 != null)
            {
                return OperationResultHelper.Fail(" شرکت با این کد قبلا تعریف شده است", nameof(Company.CompanyCode));
            }

            company.UserIdChanged = userId;
            company.DateChanged = DateTime.Now;
            await _companyRepo.EditAsync(company);
            await _companyRepo.SaveChangesAsync();
            result.Success = true;
            result.Message = "اطلاعات شرکت با موفقیت ویرایش شد.";

        }
        catch (Exception ex)
        {
            return OperationResultHelper.Fail($"خطا رخ داد \n {ex.Message}");

        }
        return result;
    }
    public async Task<List<Company>> GetAllFilteredAsync(string searchItems)
    {
        return await _companyRepo.GetAllFilteredAsync(searchItems);
    }

    public async Task<List<ComboItemsList>> SearchCompanies(string term)
    {
        return await _companyRepo.SearchCompanys(term);
    }
}
