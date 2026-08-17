using Microsoft.EntityFrameworkCore;
using Core.Domain.Entities;
using Core.Domain.RepositoryContracts;
using Application.DTO;
using Persistence.DbContexts;

namespace SematecCC.Infra;

public class OrganizationRepo : IOrganizationRepo
{
    private readonly SematecCCDbContext _db;
    public OrganizationRepo(SematecCCDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Organization organ)
    {
        //organ.DateCreated = DateTime.Now;
        //organ.UserIdCreated = 1;//? بعدا
        await _db.Organizations.AddAsync(organ);
    }

    public async Task<List<Organization>> GetAllAsync()
    {
        int rowNo = 1;
        return await _db.Organizations
           //.AsEnumerable()
           //.Select((org, index) => new
           //{
           //    org.Id,
           //    org.OrganizationName,
           //    org.Mobile,
           //    org.Telephone,
           //    org.Description,
           //})
            .OrderBy(o=>o.OrganizationName)
            .ToListAsync();
    }

    public void SaveChanges()
    {
        _db.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _db.SaveChangesAsync();
    }
    public async Task<Organization> GetAsync(int Id)
    {
        return await _db.Organizations.FirstAsync(o=> o.Id == Id);
    }

    public async Task EditAsync(Organization organ)
    {
        var oldOrgan = await _db.Organizations.FirstAsync(c => c.Id == organ.Id);
        oldOrgan.UserIdChanged =  organ.UserIdChanged;
        oldOrgan.DateChanged = organ.DateChanged;
        oldOrgan.Telephone = organ.Telephone; 
        oldOrgan.Mobile = organ.Mobile;
        oldOrgan.Description = organ.Description;
        oldOrgan.OrganizationName = organ.OrganizationName;
    }

    public async Task<List<Organization>> GetAllFilteredAsync(string searchItems)
    {
        var query = _db.Organizations.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchItems))
        {
            query = query
            .Where
                (o =>
                        o.OrganizationName.Contains(searchItems) ||
                        (o.Telephone != null && o.Telephone.StartsWith(searchItems)) ||
                        (o.Mobile != null && o.Mobile.StartsWith(searchItems)) ||
                        (o.Description != null && o.Description.Contains(searchItems))
                );
        }

        query = query
            .AsNoTracking()
            .OrderBy(o => o.OrganizationName);

        return await query.ToListAsync();
    }

    public async Task<Organization?> GetByNameAsync(string name, int Id)
    {
        return await _db.Organizations.Where(o => o.OrganizationName.Contains(name) && o.Id != Id).FirstOrDefaultAsync();
    }

    public async Task<List<ComboItemsList>> SearchOrganizations(string term)
    {
        var organizations = await _db.Organizations
            .Where(o => o.OrganizationName.Contains(term) )
            .OrderBy(o => o.OrganizationName)
            .Take(30)  // محدودیت تعداد
            .Select(o => new ComboItemsList ()
            {
                Value = o.Id+"",
                Text = o.OrganizationName
            })
            .ToListAsync();
        return organizations;
    }

    public async Task<bool> HasCardOrders(int id)
    {
        return await _db.CardOrders.AnyAsync(co=>co.OrganizationId== id);
    }
}
