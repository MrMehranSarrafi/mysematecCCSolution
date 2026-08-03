using System.ComponentModel.DataAnnotations;

namespace CardNoGenerator.Core;

public class CardTransactionType : AuditingBaseEntity
{
    //public int Id { get; set; }
    [StringLength(50)]
    public string Title { get; set; }
    public short Sign { get; set; } = 1;
    //public string SignTitle { get; set; } = "افزاینده";

    // Extended property descriptions
    // You can handle descriptions and other metadata in the model if needed
    // or store them as annotations or comments in the code.
    public virtual ICollection<CardTransaction> CardTransactions { get; set; }= new HashSet<CardTransaction>();
}
