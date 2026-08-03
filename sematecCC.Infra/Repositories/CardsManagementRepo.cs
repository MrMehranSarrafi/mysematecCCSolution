using SematecCC.Core;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace SematecCC.Infra;

public class CardsManagementRepo : ICardsManagementRepo
{
    private readonly SematecCCDbContext _db;
    public CardsManagementRepo(SematecCCDbContext db)
    {
        _db = db;
    }

    public async Task CreateAsync(CardOrder cardOrder/*, int userId*/)
    {
        //اینها را در لایه بالاتر سرویس ست نموده ایم:
        //cardOrder.DateCreated = DateTime.Now;
        //cardOrder.Status = CardOrderStatus.NewOrInitial;
        //cardOrder.UserIdCreated = ????   اینجا بخوانم یا در کنترلر
        //cardOrder.UserIdCreated = cardOrder.UserIdCreated;//userId;
        await _db.CardOrders.AddAsync(cardOrder);//اینجا فقط ذخیره می کنیم

    }
    public async Task CreateAsync(Card card)
    {
        await _db.Cards.AddAsync(card);

    }

    public void SaveChanges()
    {
        _db.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {

        return await _db.SaveChangesAsync();
    }

    public async Task<List<CardOrder>> GetAllCardOrdersAsync(OrderEnum order)
    {
        var list = _db.CardOrders.Include(c => c.Organization).Include(c => c.Company);
        var list2 = order == OrderEnum.Ascending ? list.OrderBy(o => o.Id) : list.OrderByDescending(o => o.Id);

        return await list2.ToListAsync();

    }

    public async Task Delete(int Id)
    {
        var cardOrder = await _db.CardOrders.FindAsync(Id);
        if (cardOrder != null)
        {
            _db.CardOrders.Remove(cardOrder);
        }
    }

    public async Task<CardOrder> GetCardOrderAsync(int cardOrderId, string CardNo)
    {
        var q = _db.CardOrders
            .AsNoTracking()
            .Include(co => co.Company)
            .Include(co => co.Cards.Where(c => string.IsNullOrWhiteSpace(CardNo) || c.CardNo.Contains(CardNo))).ThenInclude(c => c.Owner);
        //.AsQueryable();
        //var select = q.ToQueryString();
        /*Note:
        var q = _db.CardOrders; //outputs: IQueryable<CardOrder>
        var q = _db.CardOrders .Include(c => c.Company); // OUTPUTS THE SAME
        3:همه این متدها نیز IQueryable<T> برمی‌گردانند:
            .Where(...), .Include(...), .ThenInclude(...), .OrderBy(...), .Select(...), .AsNoTracking(...)            
         */
        return await q.SingleAsync(co => co.Id == cardOrderId);//چون از نظر منطقی فقط یک رکورد باید وجود داشته باشد. => Not FirstAsync

    }
    public async Task<CardOrder?> GetCardOrderAsync2(int cardOrderId)
    {
        // ۱. خواندن خود سفارش به صورت Tracked (برای آپدیت)
        var cardOrder = await _db.CardOrders
            .Include(co => co.Company) // اگر برای نمایش نیاز دارید
            .FirstOrDefaultAsync(co => co.Id == cardOrderId);

        if (cardOrder == null) return null;

        // ۲. خواندن لیست کارت‌ها به صورت NoTracking (جداگانه)
        // این کوئری فقط برای خواندن است و در حافظه ردیابی نمی‌شود
        cardOrder.Cards = await _db.Cards
            .Where(c => c.CardOrderId == cardOrderId)
            .AsNoTracking()
            .ToListAsync();

        return cardOrder;
    }

    public async Task<int> ConfirmCardOrder(CardOrder cardOrder, List<CardDto> cards)
    {

        // اعتبارسنجی اولیه
        if (cardOrder == null)
            throw new ArgumentNullException(nameof(cardOrder));

        if (cards == null || cards.Count == 0)
            return (int)ConfirmCardOrderResult.EmptyCardList;//1006; // کد خطای سفارشی برای لیست خالی
        //LogOperationIdDescription.CancelCardOrder
        int result = (int)ConfirmCardOrderResult.SystemError;// -1;//خطای سیستمی

        //Configuration.GetConnectionString("DefaultConnection")

        // await  _db.Database.ExecuteSqlInterpolatedAsync($" execute dbo.usp_sp1 {param1}  ");
        //await _db.u($" execute dbo.usp_sp1 {param1} @param2");
        DataTable dt = new DataTable();
        dt.Columns.Add("CardNo", typeof(string));
        dt.Columns.Add("SerialNo", typeof(string));
        dt.Columns.Add("Password", typeof(string));
        dt.Columns.Add("RowNo", typeof(int));
        for (int i = 1; i <= cards.Count; i++)
        {
            dt.Rows.Add(new object[] {
                cards[i-1].CardNo,
                cards[i-1].SerialNo,
                cards[i-1].Password,
                cards[i-1].RowNo
            });

        }
        //_db.Database.ExecuteSqlInterpolatedAsync
        //_db.Database.ExecuteSqlRawAsync()
        //execute usp_ConfirmCardOrder @cardOrderid, @Tedad, @Amount, @CompanyID, @UserId, @list
        //string command = $" execute usp_ConfirmCardOrder {cardOrder.Id}, {cards.Count},{cardOrder.Amount}, {cardOrder.CompanyID}, {cardOrder.UserIdCreated}, {dt} ";
        //await _db.Database.ExecuteSqlRawAsync(command);
        //?? بعدا UserId
        string connString = _db.Database.GetConnectionString();
        /*SqlConnection */
        using (SqlConnection conn = new SqlConnection(connString))
        {
            using (SqlCommand cmd = new SqlCommand("usp_ConfirmCardOrder", conn))
            {
                await conn.OpenAsync();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@cardOrderid", cardOrder.Id);
                cmd.Parameters.Add("@Tedad", SqlDbType.Int).Value = cards.Count;
                cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = cardOrder.Amount;
                cmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = cardOrder.CompanyId;
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = cardOrder.UserIdChanged;
                cmd.Parameters.Add("@ResultInt", SqlDbType.Int);
                cmd.Parameters["@ResultInt"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@OperationId", SqlDbType.TinyInt).Value = (byte)LogOperationIdDescription.ConfirmCardOrder;
                cmd.Parameters.Add("@ExpireDayNumber", SqlDbType.Int).Value = cardOrder.ExpireDayNumber;
                cmd.Parameters.Add("@ExpireDateFa", SqlDbType.VarChar).Value = cardOrder.ExpireDateFa;
                cmd.Parameters.Add("@list", SqlDbType.Structured).Value = dt;
                await cmd.ExecuteNonQueryAsync();
                if (cmd.Parameters["@ResultInt"].Value != DBNull.Value && cmd.Parameters["@ResultInt"].Value != null)
                {
                    result = int.Parse(cmd.Parameters["@ResultInt"].Value.ToString());
                }

                await conn.CloseAsync();
            }

        }
        return result;

    }
    public async Task CancelCardOrder(CardOrder order)
    {
        var now = DateTime.Now;
        var cardOrder = await _db.CardOrders.FirstAsync(o => o.Id == order.Id);
        cardOrder.Status = CardOrderStatus.Canceled;
        cardOrder.DateChanged = now;
        cardOrder.UserIdChanged = order.UserIdChanged;
        //var user = await _db.Users.FirstAsync(u => u.Id == cardOrder.UserIdCreated);//? بعدا اصلاح شود
        var log = new Log()
        {
            ObjectName = "CardOrder",
            Operation = LogOperationIdDescription.CancelCardOrder.GetDescriptionAttributeValue(),
            RecordId = order.Id,
            UserId = order.UserIdChanged.Value,
            //UserName = user.UserName,
            DateDone = now,
            OperationId = (byte)(LogOperationIdDescription.CancelCardOrder)
        };
        _db.Logs.Add(log);

    }
    private async Task Log(int UserId, Log log)
    {
        //var user = await _db.Users.FirstAsync(u => u.Id == UserId);//? بعدا اصلاح شود
        //var log = new Log()
        //{
        //    ObjectName = "CardOrder",
        //    Operation = "لغو سفارش کارت",
        //    RecordId = order.Id,
        //    UserId = cardOrder.UserIdCreated,
        //    UserName = user.UserName,
        //    DateDone = now
        //};
        //_db.Logs.Add(log);
    }

    public async Task<string> GetCardMaxSerialNo(string companyCode)//int cardOrderId, 
    {
        //return await _db.Cards.Where(card => card.CompanyID == companyID).MaxAsync(c => c.SerialNo);
        return await _db.Cards.Where(card => card.Company.CompanyCode == companyCode).MaxAsync(c => c.SerialNo);
    }

    public async Task EditAsync(CardOrder cardOrder)
    {
        //_db.CardOrders.Update(cardOrder);
        //_db.SaveChanges();

        var oldCardOrder = await _db.CardOrders.FirstAsync(c => c.Id == cardOrder.Id);
        oldCardOrder.UserIdChanged = cardOrder.UserIdChanged;
        oldCardOrder.DateChanged = DateTime.Now;
        oldCardOrder.Tedad = cardOrder.Tedad;
        oldCardOrder.Amount = cardOrder.Amount;
        oldCardOrder.Description = cardOrder.Description;
        //oldCardOrder.Organization = cardOrder.Organization;
        oldCardOrder.OrganizationId = cardOrder.OrganizationId;
        oldCardOrder.CompanyId = cardOrder.CompanyId;
        oldCardOrder.ExpireDayNumber = cardOrder.ExpireDayNumber;
    }

    public async Task<List<CardOrder>> CardOrderSearch(string companyCodeOrCardNo)
    {
        var list = _db.CardOrders.Include(co => co.Company).AsNoTracking().OrderByDescending(c => c.Id).AsQueryable();

        if (!string.IsNullOrWhiteSpace(companyCodeOrCardNo))
        {
            if (companyCodeOrCardNo.Length <= 8)
            {
                //list = list.Where(c => c.CompanyID.Contains(companyCode));
                list = list.Where(c => c.Company.CompanyCode.Contains(companyCodeOrCardNo));
            }
            else
            {
                // سرچ بر اساس CardNo کارت‌ها
                list = list.Where(o =>
                    o.Cards.Any(c => c.CardNo.StartsWith(companyCodeOrCardNo)));
            }
        }
        //WHERE EXISTS(
        //SELECT 1
        //FROM Cards c
        //WHERE c.CardOrderId = o.Id
        //  AND c.CardNo LIKE @searchItem + '%'

        return await list.ToListAsync();
    }
    
    public async Task<List<CardOrder>> CardOrderSearch(bool? isAdmin, int? cuttentUserCompanyId, int? cuttentUserId, string companyCodeOrCardNo)
    {
        var list = _db.CardOrders.Include(co=>co.Company)
            .Include(co=>co.Organization)
            .AsNoTracking().OrderByDescending(c => c.Id).AsQueryable();

        if (!string.IsNullOrWhiteSpace(companyCodeOrCardNo))
        {
            if (companyCodeOrCardNo.Length <= 8)
            {
                list = list.Where(c => c.Company.CompanyCode.Contains(companyCodeOrCardNo));
            }
            else
            {
                // سرچ بر اساس CardNo کارت‌ها
                list = list.Where(o =>
                    o.Cards.Any(c => c.CardNo.StartsWith(companyCodeOrCardNo)));
            }
        }
        if(isAdmin== false && cuttentUserCompanyId.HasValue)
        {
            list = list.Where(c => c.CompanyId == cuttentUserCompanyId);
        }

        return await list.ToListAsync();
    }
    
    public async Task<List<Card>> CardOrderDetailsSearch(int cardOrderId, string cardNoInitials)
    {
        var cards =  _db.Cards
            .Include(c=>c.Owner)
            .Where(c => c.CardOrderId == cardOrderId);
        if (!string.IsNullOrWhiteSpace(cardNoInitials))
            cards = cards.Where(c => c.CardNo.Contains(cardNoInitials));

        //Get CardOrder:
         //cardOrder = _db.CardOrders.FirstOrDefault(co => co.Id == cardOrderId);

         return await cards.ToListAsync();
    }

    public async Task<List<CardTransaction>> GetCardtransactions(int cardId)
    {
        //test:
        var list = _db.CardTransactions
        .AsNoTracking()
            .Where(t => t.CardId == cardId)
            .Include(t => t.CardTransactionType)
            .OrderByDescending(ct=>ct.Id);
        //var test = list.ToQueryString();
        return await list.ToListAsync();
    }

    public async Task<List<ComboItemsList>> GetCompanyIDs()
    {
        //var list = _db.Users.Select(user =>
        //new ComboItemsList()
        //{
        //    Text = user.CompanyName +" - "+ user.CompanyID,
        //    Value = user.CompanyID
        //}).Distinct();
        var list = _db.Companies.Select(c =>
        new ComboItemsList()
        {
            Text = c.CompanyName + " - " + c.CompanyCode,
            Text2 = c.CompanyCode,
            Value = c.Id.ToString()
        }).Distinct();

        return await list.ToListAsync();
    }
    public async Task<List<ComboItemsList>> GetOrganizations()
    {
        var list = _db.Organizations.Select(o =>
        new ComboItemsList() { Text = o.OrganizationName, Value = o.Id.ToString() }
        );
        return await list.ToListAsync();
    }

    public async Task DisableCard(int cardId, int userId)
    {
        var card = await _db.Cards.FirstAsync(c => c.Id == cardId);
        card.IsActive = false;
        var now = DateTime.Now;

        card.DateChanged = now;
        card.UserIdChanged = userId;// 

        //var user = await _db.Users.FirstAsync(u => u.Id == userId);// 
        var log = new Log()
        {
            ObjectName = "Card",
            //Operation = "غیرفعال کردن کارت" + $" {card.CardNo}",
            Operation = $"{(LogOperationIdDescription.DisableCard).GetDescriptionAttributeValue()} : {card.CardNo}",
            RecordId = cardId,
            UserId = userId,//  
            //UserName = user.UserName,
            DateDone = now,
            OperationId = (byte)LogOperationIdDescription.DisableCard
        };
        await _db.Logs.AddAsync(log);

    }
    public async Task IncrementCardCredit(int cardId, decimal amount, string? description, int? currentUserId)
    {
        var now = DateTime.Now;
        var card = await _db.Cards.FirstAsync(c => c.Id == cardId);
        var remainedAmount =  card.RemainedAmount + amount;
        card.RemainedAmount = remainedAmount;
        card.UserIdChanged = currentUserId;
        card.DateChanged = now;
        //cardTransaction:
        var ct = new CardTransaction()
        {
            CardTransactionTypeId=3,
            Status = CardTransactionsStatus.Verified,//همان اول تایید شده هست
            CardId = cardId,
            Amount = amount,
            RemainedAmount = remainedAmount,
            Description = description,
            UserIdCreated = currentUserId.Value,
            DateCreated = now
        };
        await _db.CardTransactions.AddAsync(ct);
        //LOG:
        var log = new Log()
        {
            DateDone = now,
            ObjectName = "Card",
            OperationId = (byte)LogOperationIdDescription.IncCardCredit,
            Operation = $"{LogOperationIdDescription.IncCardCredit.GetDescriptionAttributeValue()} {card.CardNo} به مبلغ {amount}",
            RecordId = cardId,
            UserId = currentUserId.Value
        };
        await _db.Logs.AddAsync(log);

    }
    public async Task DecreaseCardCredit(int cardId, decimal amount, string? description, int? currentUserId)
    {
        var now = DateTime.Now;
        var card = await _db.Cards.FirstAsync(c => c.Id == cardId);
        var remainedAmount = card.RemainedAmount - amount;
        card.RemainedAmount = remainedAmount;
        card.UserIdChanged = currentUserId;
        card.DateChanged = now;
        //cardTransaction:
        var ct = new CardTransaction()
        {
            CardTransactionTypeId = 4,
            Status = CardTransactionsStatus.Verified,//همان اول تایید شده هست
            CardId = cardId,
            Amount = amount,
            RemainedAmount = remainedAmount,
            Description = description,
            UserIdCreated = currentUserId.Value,
            DateCreated = now
        };
        _db.CardTransactions.Add(ct);
        //LOG:
        var log = new Log()
        {
            DateDone = now,
            ObjectName = "Card",
            OperationId = (byte)LogOperationIdDescription.DecCardCredit,
            Operation = $"{LogOperationIdDescription.DecCardCredit.GetDescriptionAttributeValue()} {card.CardNo} به مبلغ {amount}",
            RecordId = cardId,
            UserId = currentUserId.Value
        };
        _db.Logs.Add(log);

    }
    public async Task EnableCard(int cardId, int userId)
    {
        var card = await  _db.Cards.FirstAsync(c => c.Id == cardId);
        card.IsActive = true;
        var now = DateTime.Now;

        card.DateChanged = now;
        card.UserIdChanged = userId;//

        //var user = await _db.Users.FirstAsync(u => u.Id == userId);// 
        var log = new Log()
        {
            ObjectName = "Card",
            //Operation = "فعال کردن کارت" + $" {card.CardNo}",
            Operation = $"{(LogOperationIdDescription.EnableCard).GetDescriptionAttributeValue()} : {card.CardNo}",
            RecordId = cardId,
            UserId = userId,
            DateDone = now,
            OperationId = (byte)LogOperationIdDescription.EnableCard                
           
        };
       await  _db.Logs.AddAsync(log);

    }

    public async Task<Card> GetCardAsync(int cardId)
    {
        return await _db.Cards.Include(c=>c.CardOrder).AsNoTracking().FirstOrDefaultAsync(co => co.Id == cardId);
    }

    //public async Task<List<Card>> GetCardsAsync(string CardNo, string SerialNo, int? CompanyId, int? OrganizationId)
    //{
    //    var list = _db.Cards.Include(c=>c.Company).Include(c=>c.Organization)
    //   .AsNoTracking()
    //   .OrderBy(c => c.Id )
    //   .AsQueryable();

    //    if(!string.IsNullOrWhiteSpace(CardNo))
    //    {
    //        list = list.Where(c => c.CardNo.Contains(CardNo));
    //    }
    //    if (!string.IsNullOrWhiteSpace(SerialNo))
    //    {
    //        list = list.Where(c => c.CardNo.Contains(SerialNo));
    //    }
    //    if (CompanyId.HasValue)
    //    {

    //        list = list.Where(c => c.CardOrder.CompanyId == CompanyId);
    //    }
    //    if (OrganizationId  != null && OrganizationId > 0)
    //    {
    //        list = list.Where(c => c.CardOrder.OrganizationId == OrganizationId);
    //    }

    //    return await list.ToListAsync();
    //}
    public async Task<List<CardDisplayDto>> GetCardsAsync(string CardNo, string SerialNo, int? CompanyId, int? OrganizationId)
    {
        var query = _db.Cards.Include(c => c.Company)
                             .Include(c => c.Organization)
                             .AsNoTracking()
                             .OrderBy(c => c.Id)
                             .AsQueryable();

        if (!string.IsNullOrWhiteSpace(CardNo))
            query = query.Where(c => c.CardNo.Contains(CardNo));
        if (!string.IsNullOrWhiteSpace(SerialNo))
            query = query.Where(c => c.SerialNo.Contains(SerialNo));
        if (CompanyId.HasValue && CompanyId > 0)
            query = query.Where(c => c.CardOrder.CompanyId == CompanyId);
        if (OrganizationId.HasValue && OrganizationId > 0)
            query = query.Where(c => c.CardOrder.OrganizationId == OrganizationId);

        var result = await query.Select(c => new CardDisplayDto
        {
            CardNo = c.CardNo,
            SerialNo = c.SerialNo,
            CompanyName = c.Company != null ? c.Company.CompanyName : "",
            CompanyCode = c.Company != null ? c.Company.CompanyCode : "",
            OrganizationName = c.Organization != null ? c.Organization.OrganizationName : "",
            Amount = c.Amount,
            RemainedAmount = c.RemainedAmount,
            CardOrderId = c.CardOrderId
        }).ToListAsync();

        return result;
    }

    public async Task<List<CardOrder>> GetAllCardOrdersAsync(OrderEnum order, bool? isAdmin, int? companyId, int? userId)
    {
        // ساخت کوئری
        var query = _db.CardOrders
            .AsNoTracking()
            .Include(x => x.Company)
            .Include(x => x.Organization)
            .AsQueryable();

        // اعمال فیلتر در سطح دیتابیس (SQL)
        if (isAdmin== false && companyId.HasValue)
        {
            query = query.Where(x => x.CompanyId == companyId);
        }
        // اگر companyId نال باشد، شرط Where اعمال نمی‌شود و همه رکوردها انتخاب می‌شوند

        // اعمال مرتب‌سازی
        query = order == OrderEnum.Ascending
            ? query.OrderBy(x => x.Id)
            : query.OrderByDescending(x => x.Id);

        return await query.ToListAsync();
    }

    public async Task<List<ComboItemsList>> GetCompanyIDs(bool? isAdmin, int? currentUserCompanyId, int? currentUserId)
    {
        //if (!CompanyId.HasValue)//یعنی لاگین نکرده
        // return new List<ComboItemsList>();//در لایه سرویس
        var list = _db.Companies.AsNoTracking().Distinct().AsQueryable();
        if (isAdmin == false && currentUserCompanyId.HasValue)
        {
            list = list.Where(c => c.Id == currentUserCompanyId);
        }

        return await list.Select(c =>
        new ComboItemsList()
        {
            Text = c.CompanyName + " - " + c.CompanyCode,
            Text2 = c.CompanyCode,
            Value = c.Id.ToString()
        }).ToListAsync();
    }

    public async Task<List<CardDisplayDto>> GetCardsAsync(bool? isAdmin, int? currentUserCompanyId, int? UserId, string CardNo, string Serial, int? CompanyId, int? OrganizationId)
    {
        var query = _db.Cards.Include(c => c.Company)
                            .Include(c => c.Organization)
                            .AsNoTracking()
                            .OrderBy(c => c.Id)
                            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(CardNo))
            query = query.Where(c => c.CardNo.Contains(CardNo));
        if (!string.IsNullOrWhiteSpace(Serial))
            query = query.Where(c => c.SerialNo.Contains(Serial));
        if (CompanyId.HasValue && CompanyId > 0)
            query = query.Where(c => c.CardOrder.CompanyId == CompanyId);
        if (OrganizationId.HasValue && OrganizationId > 0)
            query = query.Where(c => c.CardOrder.OrganizationId == OrganizationId);

        if (isAdmin==false && currentUserCompanyId.HasValue)
        {
            query = query.Where(c => c.CardOrder.CompanyId == currentUserCompanyId);
        }

        var result = await query.Select(c => new CardDisplayDto
        {
            CardNo = c.CardNo,
            SerialNo = c.SerialNo,
            CompanyName = c.Company != null ? c.Company.CompanyName : "",
            CompanyCode = c.Company != null ? c.Company.CompanyCode : "",
            OrganizationName = c.Organization != null ? c.Organization.OrganizationName : "",
            Amount = c.Amount,
            RemainedAmount = c.RemainedAmount,
            CardOrderId = c.CardOrderId
        }).ToListAsync();
        

        return result;
    }      

    public async Task SetCardOwner(int cardId, int ownerPersonId, int currentUserId)
    {
        var card = await _db.Cards.FirstAsync(c => c.Id == cardId);
        card.OwnerPersonId  = ownerPersonId;
        var now = DateTime.Now;
        card.DateChanged = now;
        card.UserIdChanged = currentUserId;
        var log = new Log()
        {
            ObjectName = "Card",
            Operation = $"{ (LogOperationIdDescription.SetCardOwner).GetDescriptionAttributeValue()} : {card.CardNo}" ,
            RecordId = cardId,
            UserId = currentUserId,
            DateDone = now,
            OperationId = (byte)LogOperationIdDescription.SetCardOwner
        };
       await _db.Logs.AddAsync(log);
    }

    public async Task<Card?> GetCardAsync(string cardNumber)
    {
        return await _db.Cards.AsNoTracking().FirstOrDefaultAsync(c => c.CardNo == cardNumber);
    }
    public async Task<Card?> GetCardAsync(string cardNumber, string password)
    {
        return await _db.Cards.AsNoTracking()
            .Include(c=>c.Owner)
            .Include(c=>c.CardOrder)
            .FirstOrDefaultAsync(c => c.CardNo == cardNumber && c.Password == password);
    }

    

    public async Task<decimal> GetRemainedAmount(int cardId)
    {
        var cardTransaction=await _db.CardTransactions.AsNoTracking().Where(ct => ct.CardId == cardId).AsNoTracking().OrderByDescending(ct => ct.Id)
            //.Take(1)
            .FirstAsync();
        return cardTransaction.RemainedAmount;

    }
    public async Task DisableCardOrder(int cardOrderId)
    {
        var cardOrder = await _db.CardOrders.FirstOrDefaultAsync(c=>c.Id == cardOrderId);
        //var cardOrder = await _db.CardOrders.Where(c => c.Id == cardOrderId).FirstOrDefaultAsync();
        if (cardOrder == null)
            throw new Exception("سفارش کارت یافت نشد.");
        cardOrder.IsActive = false;
    }
    public async Task EnableCardOrder(int cardOrderId)
    {
        var cardOrder = await _db.CardOrders.FirstOrDefaultAsync(c => c.Id == cardOrderId);
        //var cardOrder = await _db.CardOrders.Where(c => c.Id == cardOrderId).FirstOrDefaultAsync();
        if (cardOrder == null)
            throw new Exception("سفارش کارت یافت نشد.");
        cardOrder.IsActive = true;
    }

    public async Task SetCardExpireDateFa(int cardId, string expireDateFa, int currentUserId)
    {
        var card = await _db.Cards.FirstAsync(c => c.Id == cardId);
        card.ExpireDateFa = expireDateFa;
        card.ExpireDate = expireDateFa.ToMiladi();
        card.UserIdChanged = currentUserId;
        card.DateChanged = DateTime.Now;
    }
    public async Task SetAllCardsExpireDateFa(int cardOrderId, string expireDateFa, DateTime expireDate, int currentUserId)
    {
        //var cards = await _db.Cards.Where(c => c.CardOrderId == cardOrderId).ToListAsync(); // .ToListAsync() is important to execute the query and get results

        //foreach (var card in cards)
        //{
        //    card.SomeProperty = newValue; // Modify properties
        //                                  // ... other modifications
        //}
        //await _db.SaveChangesAsync(); // Save changes to the database
        //-----------------------------------------------------------------------------------------------------------
        //راه 2: bulk update ,  بدون خواندن:
        // To update a single property
        //int updatedCount = await _db.Cards
        //    .Where(c => c.CardOrderId == cardOrderId)
        //    .ExecuteUpdateAsync(setters => setters.SetProperty(c => c.SomeProperty, newValue));

        //// To update multiple properties
        //int updatedCount = await _db.Cards
        //    .Where(c => c.CardOrderId == cardOrderId)
        //    .ExecuteUpdateAsync(setters => setters
        //        .SetProperty(c => c.SomeProperty, newValue)
        //        .SetProperty(c => c.AnotherProperty, anotherValue)
        //    );
        // You might want to commit these changes within a transaction
        // await _db.Database.BeginTransactionAsync(); // If needed
        // await _db.SaveChangesAsync(); // This is implicitly handled by ExecuteUpdateAsync, but good for transaction context
        var now = DateTime.Now;
        await _db.Cards.Where(c => c.CardOrderId == cardOrderId).ExecuteUpdateAsync(
            setters=>setters
            .SetProperty(c=>c.ExpireDateFa, expireDateFa)
            .SetProperty(c=>c.ExpireDate , expireDate)
            .SetProperty(c=>c.DateChanged, now)
            .SetProperty(c=>c.UserIdChanged , currentUserId)
            );
        await _db.Database.BeginTransactionAsync();
    }
    public async Task SetAllCardsExpireDateFa2(int cardOrderId, string expireDateFa, DateTime expireDate, int currentUserId)
    {
        // Start a transaction
        using (var transaction = await _db.Database.BeginTransactionAsync())
        {
            try
            {
                var now = DateTime.Now;
                await _db.Cards.Where(c => c.CardOrderId == cardOrderId).ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(c => c.ExpireDateFa, expireDateFa)
                        .SetProperty(c => c.ExpireDate, expireDate)
                        .SetProperty(c => c.DateChanged, now)
                        .SetProperty(c => c.UserIdChanged, currentUserId)
                );

                // If ExecuteUpdateAsync succeeds, commit the transaction
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // If any exception occurs, roll back the transaction
                await transaction.RollbackAsync();
                throw; // Re-throw the exception so the calling method can handle it
            }
        }
        // Note: No need for _db.SaveChangesAsync() here if you are managing the transaction this way,
        // as ExecuteUpdateAsync commits implicitly if no exception is thrown within its scope in some EF Core versions,
        // or it can be considered part of the transactionally managed operation.
        // However, explicitly managing the transaction with Begin/Commit/Rollback is more robust.
    }

}
