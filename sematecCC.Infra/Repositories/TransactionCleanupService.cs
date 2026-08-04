using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Core.Domain.Entities;
using Domain.Enums;

namespace SematecCC.Infra;

public class TransactionCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TransactionCleanupService> _logger;
    public TransactionCleanupService(IServiceScopeFactory scopeFactory, ILogger<TransactionCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(598), stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("TransactionCleanupService started at {time}", DateTime.Now);

            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SematecCCDbContext>();
            var cardTransactions = await dbContext.CardTransactions
                            .Include(ct => ct.Card)
                            .Where(ct => ct.Status == CardTransactionsStatus.NewOrInitial
                                        && ct.DateCreated.AddMinutes(10) < DateTime.Now)
                            .OrderBy(ct => ct.Id).ToListAsync();
            foreach (var cardtr in cardTransactions)
            {
                try
                {
                    if (cardtr.Status == CardTransactionsStatus.NewOrInitial)
                    {
                        await CancelSpendJob(dbContext, cardtr.Id);
                        //await dbContext.SaveChangesAsync();
                        _logger.LogInformation($"Transaction {cardtr.Id} ProviderId {cardtr.ProviderId} canceled.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error canceling transaction {cardtr.Id} ProviderId {cardtr.ProviderId}.");
                }
            }


            _logger.LogInformation("Waiting 30 seconds...");

            //یک عملیات غیر مسدود کننده (Non-Blocking) است که باعث می‌شود اجرای کد برای مدت مشخصی (در اینجا 30 ثانیه) متوقف شود بدون اینکه به صورت فعال از CPU استفاده کند
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // اجرای هر 30 ثانیه
        }
    }
    /*توضیحات:
     await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); یک عملیات غیر مسدود کننده (Non-Blocking) است که باعث می‌شود اجرای کد برای مدت مشخصی (در اینجا 30 ثانیه) متوقف شود بدون اینکه به صورت فعال از CPU استفاده کند.

چگونگی کار:
ثبت درخواست تأخیر: وقتی شما Task.Delay را فراخوانی می‌کنید، به سیستم عامل اطلاع می‌دهید که می‌خواهید اجرای کد را برای مدت مشخصی به تأخیر بیندازید.
تحویل کنترل به سیستم عامل: در این حالت، کنترل اجرای کد به سیستم عامل تحویل داده می‌شود و سیستم عامل می‌تواند عملیات دیگری را انجام دهد یا برنامه‌های دیگر را اجرا کند.
برگشت به اجرای کد: پس از گذشت زمان مشخص شده (30 ثانیه در اینجا)، سیستم عامل دوباره کنترل را به برنامه شما برمی‌گرداند و اجرای کد از نقطه‌ای که متوقف شده بود، ادامه پیدا می‌کند.
مصرف CPU:
بدون اشغال CPU: این روش CPU را به صورت فعال اشغال نمی‌کند. در عوض، سیستم عامل از منابع سی پی یو برای اجرای سایر برنامه‌ها یا وظایف سیستم استفاده می‌کند.
کاهش مصرف انرژی: از آنجایی که CPU به صورت بیکار (Idle) است، مصرف انرژی نیز کاهش می‌یابد.
استفاده از stoppingToken:
stoppingToken به شما امکان می‌دهد تا اجرای کد را در صورت نیاز به سرعت متوقف کنید (مثلاً در صورت خاموش شدن برنامه یا درخواست کاربر).
مقایسه با روش‌های دیگر:
حلقه بیکران (Busy-Waiting): استفاده از حلقه‌های بیکران که در آن دائماً شرایطی را چک می‌کنید، می‌تواند CPU را به صورت غیرضروری اشغال کند و مصرف انرژی را افزایش دهد.
بهترین روش:
استفاده از Task.Delay همراه با await بهترین روش برای ایجاد تأخیر در کدهای async/await است، زیرا این روش غیر مسدود کننده است و به صورت کارآمد از منابع سیستم استفاده می‌کند.
     */
    //private async Task CancelSpendJob(SematecCCDbContext dbContext, int transactionId)
    //{
    //    using (var transaction = dbContext.Database.BeginTransaction())
    //    {
    //        try
    //        {
    //            var cardTran = await dbContext.CardTransactions
    //                .Include(ct => ct.Card)
    //                .FirstAsync(ct => ct.Id == transactionId);
    //            var dateTime = DateTime.Now;
    //            cardTran.DateChanged = dateTime;
    //            cardTran.UserIdChanged = 1;
    //            cardTran.Status = CardTransactionsStatus.Canceled_timedout;
    //            cardTran.RemainedAmount += cardTran.Amount;
    //            cardTran.Card.RemainedAmount += cardTran.Amount;
    //            cardTran.Card.DateChanged = dateTime;
    //            await dbContext.SaveChangesAsync();
    //            await transaction.CommitAsync();
    //        }
    //        catch (Exception ex)
    //        {
    //            await transaction.RollbackAsync();
    //            //throw;
    //        }
    //    }
    //}
    private async Task CancelSpendJob(SematecCCDbContext dbContext, int transactionId)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            // Load + lock
            var cardTran = await dbContext.CardTransactions
                .Include(ct => ct.Card)
                .FirstAsync(ct => ct.Id == transactionId);

            // اگر قبلاً لغو شده باشد، دوباره کاری نکن
            //if (cardTran.Status == CardTransactionsStatus.Canceled_timedout ||
            //    cardTran.Status == CardTransactionsStatus.Canceled_Returned)
            //{
            //    return;
            //}
            if (cardTran.Status != CardTransactionsStatus.NewOrInitial)
            {
                return;
            }

            var now = DateTime.Now;
            var amount = cardTran.Amount;
            var card = cardTran.Card;

            var newBalance = card.RemainedAmount + amount;//به موجودی حال حاضر کارت، اضافه می شود.

            // 1. تغییر وضعیت تراکنش اصلی
            cardTran.Status = CardTransactionsStatus.Canceled_timedout;
            cardTran.DateChanged = now;
            cardTran.UserIdChanged = 1;

            // 2. ثبت تراکنش جدید (بازگشت به علت Timeout)
            var returnTransaction = new CardTransaction
            {
                CardId = cardTran.CardId,
                DateCreated = now,
                UserIdCreated = 1,
                Status = CardTransactionsStatus.Verified,   // بلافاصله تأیید شده
                CardTransactionTypeId = 6,                  // نوع تراکنش برگشتی تایم‌اوت (Customize)
                Amount = amount,
                RemainedAmount = newBalance,

                BranchId = cardTran.BranchId,
                ProviderId = cardTran.ProviderId,
                TerminalId = cardTran.TerminalId,
                Description = cardTran.Description+"\n"+ "بازگشت وجه (Timeout) Job"
            };

            // 3. بروزرسانی کارت
            card.RemainedAmount = newBalance;
            card.DateChanged = now;
            card.UserIdChanged = 1;

            dbContext.CardTransactions.Add(returnTransaction);

            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            // logging...
            //throw;
        }
    }


}
