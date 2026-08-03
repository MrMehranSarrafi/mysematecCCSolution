namespace CardNoGenerator.Core;

public interface IOrganizationRepo
{
    public void SaveChanges();
    public Task<int> SaveChangesAsync();
    public Task AddAsync(Organization person);
    public Task<List<Organization>> GetAllAsync();
    public Task<Organization> GetAsync(int Id);
    public Task<Organization?> GetByNameAsync(string name , int Id);
    public Task EditAsync(Organization organ);
    public Task<List<Organization>> GetAllFilteredAsync(string searchItems);
    public Task<List<ComboItemsList>> SearchOrganizations(string term);
    public Task<bool> HasCardOrders(int id);
}
