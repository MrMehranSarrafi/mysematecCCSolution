namespace Core.Domain.Entities;
public class UserPermission
{
     

    public int Id { get; set; }
    public int UserId { get; set; }
    public int PermissionId { get; set; }


    //public virtual ICollection<CardOrder> CardOrders { get; set; } = new HashSet<CardOrder>();
   
    // Navigation property فقط به Permission (چون هر دو در Core هستند)
    public virtual Permission Permission { get; set; }
}
