namespace SematecCC.Core;

public interface IUserService
{
    public Task<OperationResultDto> CreateUserAsync(EditUserDto user);
    public  Task<List<EditUserDto>> GetAllUsersAsync();
    public Task<OperationResultDto> Delete(int Id);
     
}
