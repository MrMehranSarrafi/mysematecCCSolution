using Application.DTO;
using Application.DTO.UserDtos;

namespace Core.ServiceContracts;

public interface IUserService
{
    public Task<OperationResultDto> CreateUserAsync(EditUserDto user);
    public  Task<List<EditUserDto>> GetAllUsersAsync();
    public Task<OperationResultDto> Delete(int Id);
     
}
