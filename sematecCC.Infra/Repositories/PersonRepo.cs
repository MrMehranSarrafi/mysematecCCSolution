using SematecCC.Core;
using Microsoft.EntityFrameworkCore;

namespace SematecCC.Infra;

public class PersonRepo : IPersonRepo
{
    private readonly SematecCCDbContext _db;
    public PersonRepo(SematecCCDbContext db)
    {
        _db = db;
    }

    public async Task CreateAsync(Person owner)
    {
        //owner.DateCreated = DateTime.Now;
        //owner.UserIdCreated = owner.UserIdCreated;
        await _db.Persons.AddAsync(owner);
    }

    public async Task EditAsync(Person owner)
    {
        var o = await _db.Persons.FirstAsync(o=>o.Id == owner.Id);
        o.DateChanged = DateTime.Now;
        o.UserIdChanged = owner.UserIdChanged;
        o.FirstName = owner.FirstName;
        o.LastName = owner.LastName;
        o.BirthDate = owner.BirthDate;
        o.BirthDateFa = owner.BirthDateFa;
        o.Phone= owner.Phone;
        o.JobPlace = owner.JobPlace;
        o.NationalCode = owner.NationalCode;
        o.Mobile = owner.Mobile;
        o.GivId = owner.GivId;
        o.CompanyId= owner.CompanyId;
    }

    public async Task<List<Person>> GetAllAsync()
    {
        return await _db.Persons.Include(c=>c.Company).ToListAsync();
    }
    public async Task<List<Person>> GetAllAsyncByCompanyId(int currentCompanyId)
    {
        return await _db.Persons.Where(p=>p.CompanyId == currentCompanyId).Include(c => c.Company).ToListAsync();
    }

    public async Task<List<Person>> GetAllFilteredAsync(string searchItems)
    {
        var q= _db.Persons.AsQueryable();
        if(!string.IsNullOrWhiteSpace(searchItems))
        {
            q = q.Where
                (o =>
                    o.LastName.Contains(searchItems) ||
                    o.Mobile.Contains(searchItems) ||
                    o.NationalCode == searchItems ||
                    (o.BirthDateFa != null && o.BirthDateFa.StartsWith(searchItems))
                 );
            
        }
        q = q.AsNoTracking()
            .OrderBy(o => o.LastName);
        return await q.ToListAsync();
    }
    public async Task<List<Person>> GetAllFilteredAsync(string searchItems, int currentCompanyId)
    {
        var q = _db.Persons.Where(p=>p.CompanyId == currentCompanyId).AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchItems))
        {
            q = q.Where
                (o =>
                    o.LastName.Contains(searchItems) ||
                    o.Mobile.Contains(searchItems) ||
                    o.NationalCode == searchItems ||
                    (o.BirthDateFa != null && o.BirthDateFa.StartsWith(searchItems))
                 );

        }
        q = q.AsNoTracking()
            .OrderBy(o => o.LastName);
        return await q.ToListAsync();
    }

    public async Task<Person?> GetByIdAsync(int Id)
    {
        return await _db.Persons
            .Include(c=>c.Company).AsNoTracking().FirstAsync(o => o.Id == Id);
    }

    public async Task<Person?> GetByMobileAsync(string mobile, int id)
    {
        return await _db.Persons.Where(o => o.Mobile==mobile && o.Id != id).FirstOrDefaultAsync();
    }
    public async Task<Person?> GetByParamsAsync(string mobile, long givId, int companyId, int personId)
    {
        return await _db.Persons.
            Where(p => p.Mobile == mobile 
            && p.GivId == givId
            && p.CompanyId == companyId
            && p.Id != personId).FirstOrDefaultAsync();
    }


    public int SaveChanges()
    {
       return _db.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _db.SaveChangesAsync();
    }

    

    public async Task<Person?> GetByNationalCodeAsync(string NationalCode, int Id)
    {
        return await _db.Persons.FirstOrDefaultAsync(o => o.NationalCode == NationalCode & o.Id != Id);
    }

    public async Task<List<Person>> GetPersonByMobileAsync(string mobileNO, int companyId)
    {

        var query = _db.Persons.AsNoTracking().Where(p=>p.CompanyId==companyId).AsQueryable();
        if (!string.IsNullOrWhiteSpace(mobileNO))
        {
            query = query.Where(p=>p.Mobile  == mobileNO );

        }
        
        return await query.ToListAsync();
    }
}
