using Application.DTO.ApiDTOs;
using Application.DTO.ApiDTOs.Responses;
using Application.DTO.UserDtos;
using Core.Domain.Entities;
using Core.Domain.RepositoryContracts;
using Domain.Enums;
using Domain.Helpers;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;

namespace SematecCC.Infra;

public class PaymentApiRepo : IPaymentApiRepo
{
    private readonly SematecCCDbContext _db;
    public PaymentApiRepo(SematecCCDbContext db)
    {
        _db = db;
    }

    public async Task<Card?> GetCardAsync(string cardNumber)
    {
        return await _db.Cards
            .AsNoTracking()
            .Include(c => c.CardOrder)
            .Include(c => c.Owner)
            .FirstOrDefaultAsync(c => c.CardNo == cardNumber);
    }
    public void SaveChanges()
    {
        _db.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {

        return await _db.SaveChangesAsync();
    }

    public async Task<SpendApiResponseDto> Spend(SpendApiRequestDto request, int currentUserId)
    {
        SpendApiResponseDto result = new SpendApiResponseDto();
        var dateTime = DateTime.Now;

        var card = await _db.Cards.FirstAsync(c => c.CardNo == request.CardNumber);
        card.RemainedAmount -= request.Amount;
        card.DateChanged = dateTime;
        card.UserIdChanged = currentUserId;
        var cardTransaction = new CardTransaction()
        {
            CardId = card.Id,
            UserIdCreated = currentUserId,
            DateCreated = dateTime,
            Amount = request.Amount,
            RemainedAmount = card.RemainedAmount,//- request.Amount,
            Status = CardTransactionsStatus.NewOrInitial,//?? بعد از 10 دقیقه باید وضعیت آن تایید شود یا مرجوع شود؟؟؟؟
            CardTransactionTypeId = 2
           ,
            Description = request.Description
           ,
            TerminalId = request.TerminalId
           ,
            ProviderId = request.ProviderId//شناسه خرید
           ,
            BranchId = request.BranchId
        };

        _db.CardTransactions.Add(cardTransaction);
        //result.referenceId = cardTransaction.Id;//فعلا 0 است// من اینو میدم به مشتری که از طریق اون به من رفرنس بده تا بتونم به جدول خودم رجوع کنم 
        await _db.SaveChangesAsync();
        result.TrackingCode = cardTransaction.Id;
        result.Amount = cardTransaction.Amount;
        return result;

    }
    public async Task ConfirmSpend(int transactionId, int userId)
    {
        var cardtransaction = await _db.CardTransactions.FirstAsync(ct => ct.Id == transactionId);
        cardtransaction.DateChanged = DateTime.Now;
        cardtransaction.UserIdChanged = userId;//!چیزی اضافه نمی کنه
        cardtransaction.Status = CardTransactionsStatus.Verified;
    }
    //public async Task CancelSpend(int transactionId, int userId)
    //{
    //    //using (var transaction = _db.Database.BeginTransactionAsync())
    //    //{
    //    //    var cardtransaction = await _db.CardTransactions.Include(ct => ct.Card).FirstAsync(ct => ct.Id == transactionId);
    //    //    cardtransaction.DateChanged = DateTime.Now;
    //    //    cardtransaction.UserIdChanged = userId;
    //    //    cardtransaction.Status = CardTransactionsStatus.Canceled_Returned;
    //    //    cardtransaction.RemainedAmount += cardtransaction.Amount;//مبلغ به موجودی قبلی بر می گرده
    //    //    cardtransaction.Card.RemainedAmount += cardtransaction.Amount;
    //    //}
    //    using (var transaction =await _db.Database.BeginTransactionAsync())
    //    {
    //        //try
    //        //{
    //        var cardtransaction = await _db.CardTransactions
    //            .Include(ct => ct.Card)
    //            .FirstAsync(ct => ct.Id == transactionId);
    //        if (cardtransaction.Status == CardTransactionsStatus.Canceled_Returned)
    //            throw new InvalidOperationException("این تراکنش قبلاً لغو شده است");
    //        var now = DateTime.Now;
    //        var newBalance = cardtransaction.Card.RemainedAmount + cardtransaction.Amount;//موجودی فعلی کارت بعد از لغو
    //        cardtransaction.DateChanged = now;
    //        cardtransaction.UserIdChanged = userId;
    //        cardtransaction.Status = CardTransactionsStatus.Canceled_Returned;
    //        //رکورد جدید می زنم
    //        var ctNew = new CardTransaction();
    //        ctNew.CardId = cardtransaction.CardId;
    //        ctNew.DateCreated = now;
    //        ctNew.UserIdCreated = userId;
    //        ctNew.Status = CardTransactionsStatus.Verified;//توجه: رکورد جدید همان ابتدا تایید شده می خورد
    //        ctNew.RemainedAmount = newBalance;// ملاک، موجودی نهایی کارت است-که بروز می باشد، نه موجودی تراکنش    
    //        ctNew.CardTransactionTypeId = 5;//افزایش تراکنش لغو شده 
    //        ctNew.Amount = cardtransaction.Amount;
    //        ctNew.BranchId = cardtransaction.BranchId;
    //        ctNew.ProviderId = cardtransaction.ProviderId;
    //        ctNew.TerminalId = cardtransaction.TerminalId;
    //        ctNew.Description = cardtransaction.Description;

    //        cardtransaction.Card.RemainedAmount  = newBalance;//بدیهی است که مبلغ به موجودی فعلی کارت اضافه می شود.
    //        cardtransaction.Card.DateChanged = now;
    //        cardtransaction.Card.UserIdChanged = userId;

    //        _db.CardTransactions.Add(ctNew);
    //        await _db.SaveChangesAsync();
    //        await transaction.CommitAsync();
    //        //}
    //        //catch (Exception ex)
    //        //{
    //        //await transaction.RollbackAsync();
    //        // مدیریت خطا
    //        //}
    //    }

    //}

    public async Task CancelSpend(int transactionId, int userId)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            var cardTransaction = await _db.CardTransactions
                .Include(ct => ct.Card)
                .FirstAsync(ct => ct.Id == transactionId);

            if (cardTransaction.Status == CardTransactionsStatus.Canceled_Returned)
                throw new InvalidOperationException("این تراکنش قبلاً لغو شده است");

            var now = DateTime.Now;
            var amount = cardTransaction.Amount;
            var newBalance = cardTransaction.Card.RemainedAmount + amount;

            // تغییر وضعیت تراکنش قبلی
            cardTransaction.Status = CardTransactionsStatus.Canceled_Returned;
            cardTransaction.DateChanged = now;
            cardTransaction.UserIdChanged = userId;

            // ثبت تراکنش جدید (افزایش موجودی)
            var ctNew = new CardTransaction
            {
                CardId = cardTransaction.CardId,
                DateCreated = now,
                UserIdCreated = userId,
                Status = CardTransactionsStatus.Verified,
                CardTransactionTypeId = 5,
                Amount = amount,
                RemainedAmount = newBalance,
                BranchId = cardTransaction.BranchId,
                ProviderId = cardTransaction.ProviderId,
                TerminalId = cardTransaction.TerminalId,
                Description = cardTransaction.Description + "\n" + "بازگشت وجه لغو خرید شده"
            };

            // بروزرسانی کارت
            cardTransaction.Card.RemainedAmount = newBalance;
            cardTransaction.Card.DateChanged = now;
            cardTransaction.Card.UserIdChanged = userId;

            _db.CardTransactions.Add(ctNew);

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    //public async Task CancelAllTimedoutSpends()
    //{
    //    var timedoutTransactions = await _db.CardTransactions
    //        .Include(ct => ct.Card)
    //        .Where(ct => ct.Status == CardTransactionsStatus.NewOrInitial
    //                    && ct.DateCreated.AddMinutes(10) < DateTime.Now)
    //        .OrderBy(ct => ct.Id).ToListAsync();
    //    foreach (var cardTransactions in timedoutTransactions)
    //    {
    //        if (cardTransactions.Status == CardTransactionsStatus.NewOrInitial)
    //            await CancelTiedoutSpend(cardTransactions);
    //    }
    //}
    //public async Task CancelTiedoutSpend(CardTransaction cardTransaction)
    //{
    //    using (var transaction = _db.Database.BeginTransaction())
    //    {
    //        try
    //        {
    //            var datetime = DateTime.Now;
    //            cardTransaction.DateChanged = datetime;
    //            cardTransaction.UserIdChanged = null;
    //            cardTransaction.Status = CardTransactionsStatus.Canceled_timedout;
    //            cardTransaction.RemainedAmount += cardTransaction.Amount;
    //            cardTransaction.Card.RemainedAmount += cardTransaction.Amount;
    //            cardTransaction.Card.DateChanged = datetime;
    //            await _db.SaveChangesAsync();
    //            await transaction.CommitAsync();
    //        }
    //        catch (Exception ex)
    //        {
    //            await transaction.RollbackAsync();
    //            // مدیریت خطا
    //        }
    //    }
    //}
    public async Task CancelAllTimedoutSpends()
    {
        var now = DateTime.Now;

        var timedoutTransactions = await _db.CardTransactions
            .Include(ct => ct.Card)
            .Where(ct =>
                ct.Status == CardTransactionsStatus.NewOrInitial &&
                ct.DateCreated.AddMinutes(10) < now)
            .OrderBy(ct => ct.Id)
            .ToListAsync();

        foreach (var cardTransaction in timedoutTransactions)
        {
            if (cardTransaction.Status == CardTransactionsStatus.NewOrInitial)
            {
                await CancelTimedoutSpendAsync(cardTransaction);
            }
        }
    }

    public async Task CancelTimedoutSpendAsync(CardTransaction cardTransaction)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            // Reload برای جلوگیری از race condition
            await _db.Entry(cardTransaction).ReloadAsync();

            if (cardTransaction.Status != CardTransactionsStatus.NewOrInitial)
                return;

            var now = DateTime.Now;
            var amount = cardTransaction.Amount;
            var card = cardTransaction.Card;

            var newBalance = card.RemainedAmount + amount;

            // 1. لغو تراکنش اصلی
            cardTransaction.Status = CardTransactionsStatus.Canceled_timedout;
            cardTransaction.DateChanged = now;
            cardTransaction.UserIdChanged = null;

            // 2. ثبت تراکنش بازگشت
            var returnTransaction = new CardTransaction
            {
                CardId = cardTransaction.CardId,
                DateCreated = now,
                UserIdCreated = 1,//سیستمی است ؟؟ بعدا یک یوزر تعریف کن
                Status = CardTransactionsStatus.Verified,
                CardTransactionTypeId = 6, // افزایش ناشی از تایم‌اوت
                Amount = amount,
                RemainedAmount = newBalance,
                BranchId = cardTransaction.BranchId,
                ProviderId = cardTransaction.ProviderId,
                TerminalId = cardTransaction.TerminalId,
                Description = cardTransaction.Description + "\n" + "بازگشت وجه به دلیل اتمام زمان"
            };

            // 3. بروزرسانی کارت
            card.RemainedAmount = newBalance;
            card.DateChanged = now;
            card.UserIdChanged = null;

            _db.CardTransactions.Add(returnTransaction);

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }



    public async Task<bool> GetApiUserAsync(string apiUsername, string apiPassword, string clientID, string clientSecret, ApiUserDto user)
    {
        var exists = await _db.Companies.FirstOrDefaultAsync(c => c.ApiUsername == apiUsername && c.ApiPassword == apiPassword && c.ClientID == clientID && c.ClientSecret == clientSecret);
        if (exists == null)
        {
            return false;//
        }
        var apiUser = await (from u in _db.Users.AsNoTracking()
                             where u.CompanyId == exists.Id  // ✅ شرط فیلتر اینجا قرار می‌گیرد
                             orderby u.Id                    // ✅ مرتب‌سازی
                             join ur in _db.UserRoles on u.Id equals ur.UserId into userRoles
                             from ur in userRoles.DefaultIfEmpty()//left join

                             select new ApiUserDto
                             {
                                 UserId = u.Id,
                                 RoleId = ur.RoleId,
                                 CompanyId = u.CompanyId,
                                 Mobile = u.PhoneNumber,
                                 UserName = u.UserName

                             })
                      .FirstOrDefaultAsync();
        if (apiUser == null)
            return false;
        user.UserId = apiUser.UserId;
        user.RoleId = apiUser.RoleId;
        user.CompanyId = apiUser.CompanyId;
        user.UserName = apiUser.UserName;
        user.Mobile = apiUser.Mobile;
        return true;
    }
    public async Task<Card?> GetCardAsync(string cardNumber, string password)
    {
        return await _db.Cards.AsNoTracking()
            .Include(c => c.Owner)
            .Include(c => c.CardOrder)
            .FirstOrDefaultAsync(c => c.CardNo == cardNumber && c.Password == password);
    }
    public async Task<CardTransaction?> GetCardTransaction(int transactionId)
    {
        return await _db.CardTransactions
            .AsNoTracking()
            .Include(ct => ct.Card)
            .FirstOrDefaultAsync(ct => ct.Id == transactionId);
    }
    public async Task<CardTransaction?> GetCardTransaction(int transactionId, string providerId)
    {
        return await _db.CardTransactions
            .AsNoTracking()
            .Include(ct => ct.Card)
            .FirstOrDefaultAsync(ct => ct.Id == transactionId && ct.ProviderId == providerId);
    }

    public async Task<CardTransaction?> GetCardTransaction(string providerId)
    {
        return await _db.CardTransactions.Include(ct => ct.Card)
            .ThenInclude(c => c.Company)
            .AsNoTracking()
     .FirstOrDefaultAsync(ct => ct.ProviderId == providerId);
    }

    public async Task<IncrementCardCreditApiResponseDto> IncrementCardCreditApi(SpendApiRequestDto request, int userId)
    {
        var result = new IncrementCardCreditApiResponseDto();
        var now = DateTime.Now;

        var card = await _db.Cards.FirstAsync(c => c.CardNo == request.CardNumber && c.Password == request.CardPassword);
        var newCredit = card.RemainedAmount + request.Amount;
        card.RemainedAmount = newCredit;
        card.DateChanged = now;
        card.UserIdChanged = userId;
        var cardTransaction = new CardTransaction()
        {
            CardId = card.Id,
            Amount = request.Amount,
            Status = CardTransactionsStatus.Verified,
            CardTransactionTypeId = 7,
            UserIdCreated = userId,
            DateCreated = now,
            RemainedAmount = newCredit,
            Description = request.Description,
            TerminalId = request.TerminalId,
            ProviderId = request.ProviderId,//شناسه(من:) مثلا سند 
            BranchId = request.BranchId
        };
        await _db.CardTransactions.AddAsync(cardTransaction);
        ////LOG:
        var log = new Log()
        {
            DateDone = now,
            ObjectName = "Card",
            OperationId = (byte)LogOperationIdDescription.IncrementCardCreditApi,
            Operation = $"{LogOperationIdDescription.IncrementCardCreditApi.GetDescriptionAttributeValue()} {card.CardNo} به مبلغ {request.Amount}",
            RecordId = card.Id,
            UserId = userId
        };
        await _db.Logs.AddAsync(log);
        await _db.SaveChangesAsync();

        //Fill response:
        result.IncrementDateFa = now.ToPersian();
        result.CurrentCredit = newCredit;
        result.TrackingCode = cardTransaction.Id;
        result.CardNumber = request.CardNumber;
        result.Amount = request.Amount;
        return result;
    }

    public async Task<Person?> CreatePerson(Person person)//(PersonRequestDto person)  NOTE:Do Mapping in service layer. Data layer should ONLY work with entity
    {
        await _db.Persons.AddAsync(person);
        //await _db.SaveChangesAsync();//؟
        return person;
    }

    public async Task UpdateMobile(int id, string mobile, int userId)
    {
        var person = await _db.Persons.FirstAsync();
        person.Mobile = mobile;
        person.DateChanged = DateTime.Now;
        person.UserIdChanged = userId;
        await _db.SaveChangesAsync();
    }

    public async Task<Person?> GetPerson(long givId, int companyId)
    {
        return await _db.Persons.FirstOrDefaultAsync(p => p.CompanyId == companyId && p.GivId == givId);
    }
    public async Task SetCardOwner(int cardId, int ownerId)
    {
        var card = await _db.Cards.FirstAsync(c => c.Id == cardId);
        card.OwnerPersonId = ownerId;

    }
    public async Task<List<CardResponse>> GetPersonCards(long? givId, string?  mobileNo,int companyId)
    {
        IQueryable<Person> query = _db.Persons.Where(p => p.CompanyId == companyId);

        if (givId.HasValue)
        {
            query = query.Where(p => p.GivId == givId.Value);
        }

        if (!string.IsNullOrWhiteSpace(mobileNo))
        {
            query = query.Where(p => p.Mobile == mobileNo);
        }

        var person = await query.FirstOrDefaultAsync();
        if (person != null)
        {
            return await _db.Cards.Where(c => c.OwnerPersonId == person.Id && c.CompanyId == companyId)
                .Select(c=>new CardResponse
                {
                    Id = c.Id,
                    CardNo = c.CardNo,
                    CardOrderId = c.CardOrderId,
                    ExpireDate = c.ExpireDate,
                    ExpireDateFa = c.ExpireDateFa,
                    Amount = c.Amount,
                    RemainedAmount = c.RemainedAmount,
                    SerialNo = c.SerialNo      
                })
                .ToListAsync();
        }
        else
            throw new Exception("شخصی پیدا نشد");

    }
}
