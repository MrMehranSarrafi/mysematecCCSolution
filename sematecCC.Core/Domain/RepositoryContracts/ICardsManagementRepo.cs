namespace SematecCC.Core;

public interface ICardsManagementRepo
{
    public Task CreateAsync(CardOrder cardOrder/*, int userId*/);
    public Task<int> ConfirmCardOrder(CardOrder cardOrder, List<CardDto> cards);
    public Task CancelCardOrder(CardOrder cardOrder);
    public Task CreateAsync(Card card);
    public void SaveChanges();
    public Task<int> SaveChangesAsync();
    //public Task<List<CardOrder>> GetAllAsync();

    public Task Delete(int Id);
    public Task<List<CardOrder>> GetAllCardOrdersAsync(OrderEnum order);
    public Task<List<CardOrder>> GetAllCardOrdersAsync(OrderEnum order, bool? isAdmin, int? companyId, int? userId);
    public Task<CardOrder> GetCardOrderAsync(int cardOrderId, string CardNo);
    public Task<string> GetCardMaxSerialNo( string companyID);//int cardOrderId,
    public Task EditAsync(CardOrder cardOrder);
    public Task<List<CardOrder>> CardOrderSearch(string companyCodeOrCardNo);
    
    public Task<List<CardOrder>> CardOrderSearch(bool? isAdmin, int? currentUserCompanyId, int? currentUserId, string companyCodeOrCardNo);
    //public Task<List<CardOrder>> CardOrderSearch(string companyCodeOrCardNo, bool?  isAdmin, int? companyId, int? userId);
    public Task<List<Card>> CardOrderDetailsSearch(int cardOrderId, string cardNoInitials);
    public Task<List<CardTransaction>> GetCardtransactions(int cardId);
    public Task<List<ComboItemsList>> GetCompanyIDs();
    public Task<List<ComboItemsList>> GetCompanyIDs(bool? IsAdmin, int? currentUserCompanyId, int? UserId);
    
    public Task<List<ComboItemsList>> GetOrganizations();
    public Task DisableCard(int cardId, int userId);
    public Task EnableCard(int cardId, int userId);
    public Task<Card> GetCardAsync(int cardId);
    //public Task<List<Card>> GetCardsAsync(string CardNo, string Serial, int? CompanyId, int? OrganizationId);
    
    //use projection for network traffic lowage, instead of including all properties of navigations:
    public Task<List<CardDisplayDto>> GetCardsAsync(string CardNo, string Serial, int? CompanyId, int? OrganizationId);
    public Task<List<CardDisplayDto>> GetCardsAsync(bool? isAdmin, int? currentUserCompanyId, int? UserId, string CardNo, string Serial, int? CompanyId, int? OrganizationId);
    public Task SetCardOwner(int cardId, int ownerPersonId, int currentUserId);
    public Task<Card?> GetCardAsync(string cardNumber);
    public Task<Card?> GetCardAsync(string cardNumber, string password);
    
    public Task<decimal> GetRemainedAmount(int cardId);
    public Task IncrementCardCredit(int cardId, decimal amount, string? Description, int? currentUserId);
    public Task DecreaseCardCredit(int cardId, decimal amount, string? Description,  int? currentUserId);
    //دسترسی همیشه public  //( نمی‌توان private کرد)
    //بنابراین نوشتن public در اینترفیس خطا نیست ولی اضافه است. اکثر برنامه‌نویسان نمی‌نویسند.
    public Task DisableCardOrder(int cardOrderId);
    public Task EnableCardOrder(int cardOrderId);
    public Task SetCardExpireDateFa(int cardId, string expireDateFa, int currentUserId);
    public Task SetAllCardsExpireDateFa(int cardOrderId, string expireDateFa,DateTime expireDate, int currentUserId);
}
