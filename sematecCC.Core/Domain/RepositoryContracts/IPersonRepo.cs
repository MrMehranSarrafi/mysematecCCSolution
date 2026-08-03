using System.ComponentModel.Design;

namespace SematecCC.Core;

public interface IPersonRepo
{
    public int SaveChanges();
    public Task<int> SaveChangesAsync();
    Task CreateAsync(Person owner);
    public Task<List<Person>> GetAllAsync();
    //public Task<List<Person>> GetAllAsync(bool? isAdmin, int? currentCompanyId, int? currentUserId);
    public Task<Person?> GetByIdAsync(int Id);
    public Task<Person?> GetByNationalCodeAsync(string NationalCode, int Id);
    public Task EditAsync(Person owner);
    public Task<Person?> GetByParamsAsync(string mobile, long givId, int companyId, int personId);
    public Task<Person?> GetByMobileAsync(string mobile, int personId);
    public Task<List<Person>> GetAllFilteredAsync(string searchItems);
    public Task<List<Person>> GetAllFilteredAsync(string searchItems, int currentCompanyId);
    public Task<List<Person>> GetPersonByMobileAsync(string mobileNO, int companyId);
    public Task<List<Person>> GetAllAsyncByCompanyId( int currentCompanyId);
}
