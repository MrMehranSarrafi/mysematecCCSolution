using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardNoGenerator.Core;

public class CardTransaction: AuditingBaseEntity
{
   
    [Column(TypeName ="decimal(18,3)")]
    public decimal Amount { get; set; }
    [Column(TypeName = "decimal(18,3)")]
    public decimal RemainedAmount { get; set; }
   
    //tinyint = byte
    public CardTransactionsStatus Status { get; set; }
    [Column(TypeName ="nvarchar(4000)")]
    public string? Description { get; set; }
    [Column(TypeName = "nvarchar(100)")]
    public string? ProviderId { get; set; }//شناسه خرید =i.e. Id of Factor foroosh Id of my customers' table (ProviderId of digipay help page 8 of 11)
    //مرجع من هستم. این را به عنوان رفرنس میدم به مشتری، اطلاعات فروش خودشو پیدا میکنه
    //او آیدی جدول فروش خودشو میده به من و من به او میتونم رجوع کنم   referenceId
    //در پاسخ من به او آیدی جدول خودمو میدم تا او بهش رجوع کنه
    [Column(TypeName = "nvarchar(100)")]
    public string? BranchId { get; set; }
    [Column(TypeName = "nvarchar(100)")]
    public string? TerminalId { get; set; }

    //fk default is cascade in sql server; but i wanna Restrict or NoAction. So i use program.cs
    [Required]
    public int CardId { get; set; }
    [Required]
    public virtual Card Card { get; set; }
    public int CardTransactionTypeId { get; set; } = 1;

    // Navigation property for foreign key relationship
    public virtual CardTransactionType CardTransactionType { get; set; }
}
