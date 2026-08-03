using SematecCC.Core.Helpers;


namespace SematecCC.Core.Services
{
    public class OrganizationService
    {
        private readonly IOrganizationRepo _organizationRepo;
        private readonly IUserContextService _userContext;


        public OrganizationService(IOrganizationRepo organizationRepo, IUserContextService userContext)
        {
            _organizationRepo = organizationRepo;
            _userContext = userContext;
        }
        public async Task<OperationResultDto> AddAsync(Organization organization)
        {
            var result = new OperationResultDto();
            if (!_userContext.IsAuthenticated())
                return OperationResultHelper.Fail("هنوز وارد سیستم نشده اید.");
            var userId = _userContext.GetCurrentUserId();
            try
            {
                var organ = await _organizationRepo.GetByNameAsync(organization.OrganizationName, organization.Id);
                if (organ != null)
                {
                    return OperationResultHelper.Fail(" سازمان با این نام قبلا تعریف شده است", nameof(Organization.OrganizationName));
                }
                organization.UserIdCreated = userId.Value;
                organization.DateCreated = DateTime.Now;
                await _organizationRepo.AddAsync(organization);
                await _organizationRepo.SaveChangesAsync();
                result = OperationResultHelper.Success("اطلاعات سازمان با موفقیت ثبت شد.");
            }
            catch (Exception ex)
            {
                return OperationResultHelper.Fail($"در ثبت اطلاعات خطا رخ داد.\n {ex.Message}");
            }
            return result;
        }
        public async Task<Organization> GetAsync(int Id)
        {
            return await _organizationRepo.GetAsync(Id);
        }
        public async Task<List<Organization>> GetAllAsync()
        {
            return await _organizationRepo.GetAllAsync();
        }

        public async Task<OperationResultDto> EditAsync(Organization organ)
        {
            /*نکته:
                هترین روش در معماری تمیز(Clean Architecture) این است که منطق تجاری(Business Logic) و بررسی‌های اعتبارسنجی را از داخل بلوک try-catch خارج کنید. بلوک try-catch فقط باید برای پوشش دادن خطاهای غیرمنتظره(مثل قطع ارتباط دیتابیس یا خطاهای سیستمی) استفاده شود.
            */
            var result = new OperationResultDto();
            if(await HasCardOrders(organ.Id))
            {
                return OperationResultHelper.Fail("این سازمان در سفارش کارت ها استفاده شده و قابل ویرایش نمی باشد.");
            }
            if (!_userContext.IsAuthenticated())
                return OperationResultHelper.Fail("هنوز وارد سیستم نشده اید.");
            var userId = _userContext.GetCurrentUserId();

            var organization = await _organizationRepo.GetByNameAsync(organ.OrganizationName, organ.Id);
            if (organization != null)
            {
                return OperationResultHelper.Fail(" سازمان با این نام قبلا تعریف شده است", nameof(Organization.OrganizationName));
            }
            try
            {
                organ.UserIdChanged = userId;
                organ.DateChanged = DateTime.Now;
                await _organizationRepo.EditAsync(organ);
                await _organizationRepo.SaveChangesAsync();
                result.Success = true;
                result.Message = "اطلاعات سازمان با موفقیت ویرایش شد.";

            }
            catch (Exception ex)
            {
                return OperationResultHelper.Fail($"خطا رخ داد \n {ex.Message}", nameof(Organization.OrganizationName));
            }
            return result;
        }

        public async Task<List<Organization>> GetAllFilteredAsync(string searchItems)
        {
            return await _organizationRepo.GetAllFilteredAsync(searchItems);
        }

        public async Task<List<ComboItemsList>> SearchOrganizations(string term)
        {
            return await _organizationRepo.SearchOrganizations(term);
        }

        public async Task<bool> HasCardOrders(int id)
        {
            return await _organizationRepo.HasCardOrders(id);
        }
    }
}
