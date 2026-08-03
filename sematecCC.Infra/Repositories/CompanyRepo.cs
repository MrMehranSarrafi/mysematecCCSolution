using CardNoGenerator.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CardNoGenerator.Infra;

public class CompanyRepo : ICompanyRepo
{
    private readonly CardNoGeneratorDbContext _db;
    public CompanyRepo(CardNoGeneratorDbContext db)
    {
        _db = db;
    }
    public void SaveChanges()
    {
        _db.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _db.SaveChangesAsync();
    }

    public async Task AddAsync(Company company)
    {
        //var hasher = new PasswordHasher<Company>();
        //if (!string.IsNullOrWhiteSpace(company.ApiPassword))
        //{
            //company.ApiPassword = $"{hasher.HashPassword(company, company.ApiPassword)}";
        //}
        //if (!string.IsNullOrWhiteSpace(company.ClientSecret))
        //{
            //company.ClientSecret = $"{hasher.HashPassword(company, company.ClientSecret)}";
        //}
        await _db.Companies.AddAsync(company);
    }
    public async Task EditAsync(Company company)
    {
        //Test();
        var oldCompany = await _db.Companies.FirstAsync(c => c.Id == company.Id);
        oldCompany.UserIdChanged = company.UserIdChanged;
        oldCompany.DateChanged = company.DateChanged;
        oldCompany.CompanyCode = company.CompanyCode;
        oldCompany.CompanyName = company.CompanyName;
        oldCompany.ApiUsername = company.ApiUsername;
        oldCompany.ClientID = company.ClientID;
        var hasher = new PasswordHasher<Company>();
        if (!string.IsNullOrWhiteSpace(company.ApiPassword))
        {
           // oldCompany.ApiPassword = $"{hasher.HashPassword(oldCompany, company.ApiPassword)}";
            oldCompany.ApiPassword =  company.ApiPassword;
        }
        if(!string.IsNullOrWhiteSpace(company.ClientSecret))
        {
            //oldCompany.ClientSecret = $"{hasher.HashPassword(oldCompany, company.ClientID)}";
            oldCompany.ClientSecret = company.ClientSecret;
        }
    }

    public async Task<List<Company>> GetAllAsync()
    {
        return await _db.Companies.AsNoTracking().OrderBy(c => c.CompanyCode).ToListAsync();
    }
    public async Task<Company> GetByIdAsync(int Id)
    {
        return await _db.Companies.AsNoTracking().FirstAsync(o => o.Id == Id);
    }
    
    public async Task<List<ComboItemsList>> GetComboCompaniesAsync()
    {
        var list = _db.Companies.Select(c =>
        new ComboItemsList()
        {
            Text = c.CompanyName + " - " + c.CompanyCode,
            Value = c.Id.ToString()
        }).Distinct();
        return await list.ToListAsync();
    }
    public async Task<List<ComboItemsList>> GetAllComboAsync()
    {
        return await _db.Companies.OrderBy(c => c.CompanyCode).Select(c =>
        new ComboItemsList()
        {
            Text = c.CompanyName + " - " + c.CompanyCode,
            Value = c.Id.ToString()
        }).ToListAsync();
    }
    public async Task<ComboItemsList> GetByIdComboAsync(int Id)
    {
        return await _db.Companies
            .Select(c =>
                              new ComboItemsList()
                              {
                                  Text = c.CompanyName + " - " + c.CompanyCode,
                                  Value = c.Id.ToString()
                              })
            .FirstAsync(combo => combo.Value == Id.ToString());
            
    }


    public async Task<List<Company>> GetAllFilteredAsync(string searchItems)
    {
        var query = _db.Companies.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchItems))
        {
            query = query
            .Where
                (c =>
                        c.CompanyName.Contains(searchItems) ||
                        (c.CompanyCode != null && c.CompanyCode.StartsWith(searchItems))
                );
        }

        query = query
            .AsNoTracking()
            .OrderBy(c => c.CompanyCode);

        return await query.ToListAsync();
    }

    public async Task<Company?> GetByNameAsync(string name, int Id)
    {
        return await _db.Companies.Where(c => c.CompanyName == name && c.Id != Id).FirstOrDefaultAsync();
    }
    public async Task<Company?> GetByCodeAsync(string companyCode, int Id)
    {
        return await _db.Companies.Where(c => c.CompanyCode == companyCode && c.Id != Id).FirstOrDefaultAsync();
    }
    public async Task<List<ComboItemsList>> SearchCompanys(string term)
    {
        var companies = await _db.Companies
            .Where(c => c.CompanyName.Contains(term))
            .OrderBy(o => o.CompanyCode)
            .Take(30)  // محدودیت تعداد
            .Select(c => new ComboItemsList()
            {
                Value = c.Id + "",
                Text = c.CompanyCode + " - " + c.CompanyName
            })
            .ToListAsync();
        return companies;
    }
    private bool Test()
    {
        string hashedPass = "AQAAAAIAAYagAAAAEDmW6OWGJvbnmbP3wP4SqQ4J7Q0petFx6EbNWSGiKxNp1hpY8709wmtcucdwh2r1hA==";
        string inputString = "Sarrafi2026Pro";
        Company co = new Company() { ApiPassword = hashedPass };
        var hasher = new PasswordHasher<Company>();
        var resultOfCompare = hasher.VerifyHashedPassword(co, hashedPass, inputString);

        return resultOfCompare == PasswordVerificationResult.Success;
    }



}
