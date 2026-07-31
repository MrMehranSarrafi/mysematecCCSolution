using System.ComponentModel;
using System.ComponentModel.DataAnnotations;//Reflection

namespace CardNoGenerator.Core;


//public enum ContactType : byte
//{
//    [Display(Name = "تلفن ثابت")]
//    Landline = 1,

//    [Display(Name = "تلفن همراه")]
//    Mobile = 2
//}

// وضعیت کارت
public enum CardStatus : byte
{
    [Display(Name = "اولیه")]
    NewOrInitial = 1,
    [Display(Name = "تایید شده")]
    Verified = 2,
    [Display(Name = "لغو شده")]
    Canceled = 3
}
// وضعیت سفارش کارت
//توسط کاربر(مشتری یا ادمین) درخواست تعدادی کارت با مبلغ اولیه ثبت و سپس تایید می شود
//هنگام تایید ، شماره کارت ها و پسوردشان ایجاد و در وضعیت تایید قرار می گیرد
public enum CardOrderStatus : byte
{
    [Display(Name = "اولیه")]
    NewOrInitial = 1,
    [Display(Name = "تایید شده")]
    Verified = 2,
    [Display(Name = "لغو شده")]
    Canceled = 3
}

/// <summary>
/// وضعیت تراکنش کارت
/// </summary>
public enum CardTransactionsStatus : byte
{
    /// <summary>
    /// اولیه
    /// </summary>
    [Display(Name = "اولیه")]
    NewOrInitial = 1,
    /// <summary>
    /// اولیه
    /// </summary>
    [Display(Name = "تایید شده")]
    Verified = 2,
    /// <summary>
    /// منقضی شده
    /// </summary>
    [Display(Name = "منقضی شده")]
    Canceled_timedout = 3,
    /// <summary>
    /// مرجوع شده
    /// </summary>
    [Display(Name = "مرجوع شده")]
    Canceled_Returned = 4
}
public enum OrderEnum : byte
{
    Ascending=1,
    Descending=2
}


public enum ConfirmCardOrderResult
{
    [Description("عملیات با موفقیت انجام شد")]
    Success = 0,

    [Description("خطای سیستمی - لطفا با پشتیبانی تماس بگیرید")]
    SystemError = -1,

    [Description("خطای قفل در دیتابیس - مجدد تلاش کنید")]
    DeadlockError = -2,

    [Description("خطای اتصال به دیتابیس")]
    DatabaseError = -3,

    [Description("سفارش یافت نشد")]
    OrderNotFound = 1002,

    [Description("سفارش قبلاً تایید شده است")]
    AlreadyConfirmed = 1003,

    [Description("سفارش لغو شده و قابل تایید نیست")]
    OrderCancelled = 1004,

    [Description("وضعیت سفارش نامعتبر است")]
    InvalidStatus = 1005,

    [Description("لیست کارت‌ها خالی است")]
    EmptyCardList = 1006,

    [Description("تعداد کارت‌ها با سفارش مطابقت ندارد")]
    CountMismatch = 1007,

    [Description("داده‌های ورودی نامعتبر است")]
    InvalidInput = 2001,

    [Description("شما مجوز تایید این سفارش را ندارید")]
    UserNotAuthorized = 2002,

    [Description("شرکت مربوطه یافت نشد")]
    CompanyNotFound = 2003,

    [Description("موجودی کافی نیست")]
    InsufficientBalance = 2004
}
public enum LogOperationIdDescription:byte
{
    [Description("تایید سفارش کارت")]
    ConfirmCardOrder=1,
    [Description("لغو سفارش کارت")]
    CancelCardOrder = 2,
    [Description("غیرفعال کردن کارت")]
    DisableCard =3,
    [Description("فعال کردن کارت")]
    EnableCard =4,
    [Description("تعیین مالک کارت")]
    SetCardOwner = 5,
    [Description(" افزایش دستی اعتبار کارت")]
    IncCardCredit = 6,
    [Description(" کاهش دستی اعتبار کارت")]
    DecCardCredit = 7,
    [Description(" افزایش اعتبار کارت از طریق Api")]
    IncrementCardCreditApi = 8,
    [Description("تعیین تاریخ انقضای کارت")]
    SetCardExpirationDate =9,
    [Description("تعیین تاریخ انقضای سفارش کارت")]
    SetOrderCardExpirationDate = 10
}

public enum UserPermissionsEnum
{
    #region Menu
    Home
    ,UserAccount
    ,Company
    ,Organization
    ,Person
    ,CardOrder
    ,CardOrderDetailsView
    ,UserAccountNew
    ,UserAccountList
    ,UserAccountPermissions
    ,CompanyNew
    ,CompanyList
    ,OrganizationNew
    ,OrganizationList
    ,PersonNew
    ,PersonList
    ,CardOrderNew
    ,CardOrderList

    #endregion
        
        , CardOrderEdit
        , OrganizationView
        , OrganizationEdit
        , PersonView
        , PersonEdit
        , CardOrderConfirm
        , CardOrderCancel
        ,CardOrderEnable
        ,CardOrderDisable
        ,CardOrderSendToExcel
        ,CardOrderCSV
        ,CardOrderSetAllCardsExpireDate
        ,CardOrderSetTheCardExpireDate
        ,CardOrderViewTheCardTransactions
        ,CardOrderEnableTheCard
        ,CardOrderDisableTheCard
        ,CardOrderSetOwnerOfTheCard
        ,CardOrderIncreaseCreditOfTheCard
        ,CardOrderDecreaseCreditOfTheCard
        , CompanyEdit
        , UserAccountEdit
}
public enum RoleNamesEnum
{
    admin,
    companyAdmin,
    companyUser
}
