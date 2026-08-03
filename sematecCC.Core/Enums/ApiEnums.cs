using System.ComponentModel;

namespace CardNoGenerator.Core;

// وضعیت کارت
//public enum CardStatus1 : byte
//{
//    [Description("کارت در وضعیت اولیه می باشد")]
//    [Display(Name = "اولیه")]
//    NewOrInitial = 1,
//    [Display(Name = "تایید شده")]
//    Verified = 2,
//    [Display(Name = "لغو شده")]
//    Canceled = 3
//}

//Here's your enum with both XML comments and `Description` attributes:


public enum HttpStatusCodeEnum
{
    /// <summary>
    /// درخواست با موفقیت انجام شد
    /// </summary>
    //[Description("درخواست با موفقیت انجام شد")]
    Ok = 200,

    /// <summary>
    /// یک منبع جدید با موفقیت ایجاد شد
    /// </summary>
    //[Description("یک منبع جدید با موفقیت ایجاد شد")]
    Created = 201,

    /// <summary>
    /// درخواست موفقیت‌آمیز بود، اما داده‌ای برای بازگرداندن وجود ندارد. (مثلاً بعد از حذف یک رکورد)
    /// </summary>
    //[Description("درخواست موفقیت‌آمیز بود، اما داده‌ای برای بازگرداندن وجود ندارد.")]
    NoContent = 204,

    /// <summary>
    /// آدرس صفحه به طور دائم تغییر کرده است
    /// </summary>
    //[Description("آدرس صفحه به طور دائم تغییر کرده است")]
    MovedPermanently = 301,

    /// <summary>
    /// کلاینت قبلاً صفحه را کش کرده است و سرور می‌گوید "داده‌ای تغییر نکرده، از کش خودت استفاده کن"
    /// </summary>
    //[Description("کلاینت قبلاً صفحه را کش کرده است و سرور می‌گوید \"داده‌ای تغییر نکرده، از کش خودت استفاده کن\"")]
    NotModified = 304,

    /// <summary>
    /// درخواست از نظر فرمت یا سینتکس اشتباه است // سرور درخواست کاربر را درک‌کند یا نمی‌تواند آن را پردازش کند، اما دلیل آن مشخص نیست.
    /// </summary>
    //[Description("درخواست از نظر فرمت یا سینتکس اشتباه است")]
    BadRequest = 400,

    /// <summary>
    ///  توکن یا کوکی ندارید یا نامعتبر است یا منقضی شده است
    /// </summary>
    [Description(" توکن یا کوکی ندارید یا نامعتبر است یا منقضی شده است")]
    Unauthorized = 401,

    /// <summary>
    /// .کلاینت احراز هویت کرده است، اما اجازه دسترسی به این منبع را ندارد
    /// </summary>
    //[Description("کلاینت احراز هویت کرده است، اما اجازه دسترسی به این منبع را ندارد")]
    Forbidden = 403,

    /// <summary>
    /// منبعی که درخواست کرده‌اید وجود ندارد // (مثلاً آدرس API اشتباه تایپ شده یا رکوردی در دیتابیس حذف شده است
    /// </summary>
    //[Description("منبعی که درخواست کرده‌اید وجود ندارد")]
    NotFound = 404,

    /// <summary>
    /// تعارض با وضعیت فعلی منبع // مثلا کارت غیرفعال است
    /// </summary>
    //[Description("تعارض با وضعیت فعلی منبع")]
    Conflict = 409,

    /// <summary>
    /// درخواست کاربر قابل پردازش نیست زیرا داده‌های ارسالی ناقص یا نامعتبر هستند // رور درخواست کاربر را درک می‌کند اما به دلیل وجود خطا در داده‌های ارسالی، نمی‌تواند آن را پردازش کند
    /// </summary>
    //[Description("درخواست کاربر قابل پردازش نیست زیرا داده‌های ارسالی ناقص یا نامعتبر هستند")]
    UnprocessableEntity = 422,

    /// <summary>
    /// یک خطای عمومی و غیرمنتظره در سرور رخ داده است
    /// </summary>
    //[Description("یک خطای عمومی و غیرمنتظره در سرور رخ داده است")]
    InternalServerError = 500,
}


/// <summary>
/// کدهای خطا برای APIهای مرتبط با عملیات پرداخت، خرید و وضعیت کارت.
/// </summary>
public enum PaymentApiErrorCodes : byte//PaymentApiErrorCodesEnum
{
    /// <summary>
    /// ورودی های نامعتبر.
    /// </summary>
    [Description("ورودی های نامعتبر")]
    InvalidInput = 0,

    [Description("کارت با این مشخصات یافت نشد")]
    /// <summary>
    /// کارتی با مشخصات ارسالی یافت نشد.
    /// </summary>
    CardNotFound = 1,

    /// <summary>
    /// کارت ارسالی غیرفعال است و امکان استفاده ندارد.
    /// </summary>
    [Description("کارت غیرفعال است و امکان استفاده ندارد")]
    CardIsDisabled = 2,

    /// <summary>
    /// کاربر به کارت‌های متعلق به سایر شرکت‌ها دسترسی ندارد.
    /// </summary>
    [Description("شما به کارت های شرکت های دیگر دسترسی ندارید")]
    CardForbidden = 3,

    /// <summary>
    /// کارت ارسالی منقضی شده است و قابل استفاده نیست.
    /// </summary>
    [Description("کارت منقضی شده است و قابل استفاده نیست")]
    CardExpired = 4,

    /// <summary>
    /// مالک کارت قبلا تعیین شده است.
    /// </summary>
    [Description("مالک کارت قبلا تعیین شده است.")]
    CardOwnerAlreadySet = 5,

    /// <summary>
    /// وضعیت فعلی خرید یا عملیات، اجازه اجرای درخواست را نمی‌دهد.
    /// </summary>
    [Description("وضعیت خرید برای عمل فراخوانی شده مناسب نیست")]
    OperationNotAllowed = 100,

    /// <summary>
    /// خرید با شناسه ارسالی یافت نشد.
    /// </summary>
    [Description("خرید پیدا نشد.")]
    PurchaseNotFound = 101,

    /// <summary>
    /// کاربر به خریدهای متعلق به سایر شرکت‌ها دسترسی ندارد.
    /// </summary>
    [Description("شما به خرید های شرکت های دیگر دسترسی ندارید")]
    PurchaseForbidden = 102,

    /// <summary>
    /// خطای داخلی در سیستم رخ داده است.
    /// </summary>
    [Description("خطای داخلی رخ داد")]
    InternalError = 103,

    /// <summary>
    /// خرید با این شناسه وجود دارد.
    /// </summary>
    [Description("خرید با این شناسه وجود دارد")]
    SpendAlreadyExists = 104,

    /// <summary>
    /// تراکنش خرید با این شناسه قبلاً انجام شده است.
    /// </summary>
    [Description("شما یکبار خرید با این شناسه انجام دادید")]
    RepeatedSpend = 105,

    /// <summary>
    /// اعتبار کافی در کارت (یا حساب کاربر) برای انجام خرید وجود ندارد.
    /// </summary>
    [Description("درخواست خرید بیشتر از اعتبار کاربر می باشد")]
    InsufficientBalance = 106


    //[Description("شناسه خرید ارسالی تکراری است")]
    //PROVIDER_ID_ALREADY_EXISTS = 11//مهم نیست. برای خود مشتری که تکراری نیست. Id  جدول فاکتور فروششه

}
