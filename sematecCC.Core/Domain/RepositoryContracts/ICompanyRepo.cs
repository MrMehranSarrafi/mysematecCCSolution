namespace CardNoGenerator.Core;

public interface ICompanyRepo
{
    public void SaveChanges();
    public Task<int> SaveChangesAsync();
    public Task AddAsync(Company company);
    public Task<List<Company>> GetAllAsync();
    public Task<Company> GetByIdAsync(int Id);
    public Task<Company?> GetByNameAsync(string name, int Id);
    public Task EditAsync(Company company);
    public Task<List<Company>> GetAllFilteredAsync(string searchItems);
    public Task<List<ComboItemsList>> SearchCompanys(string term);
    public Task<Company?> GetByCodeAsync(string companyCode, int Id);
    public Task<List<ComboItemsList>> GetComboCompaniesAsync();
    public Task<List<ComboItemsList>> GetAllComboAsync();

    public Task<ComboItemsList> GetByIdComboAsync(int Id);
}
