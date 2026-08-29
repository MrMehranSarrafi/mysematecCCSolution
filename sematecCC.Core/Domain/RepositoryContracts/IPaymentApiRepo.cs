using Application.DTO.ApiDTOs;
using Application.DTO.ApiDTOs.Responses;
using Application.DTO.UserDtos;
using Core.Domain.Entities;

namespace Core.Domain.RepositoryContracts;

public interface IPaymentApiRepo
{
     void SaveChanges();
     Task<int> SaveChangesAsync();
     Task<Card?> GetCardAsync(string cardNumber);
     Task<Card?> GetCardAsync(string cardNumber, string password);
     Task<Person?> GetPerson(long givId, int companyId);
     Task<SpendApiResponseDto> Spend(SpendApiRequestDto request , int currentUserId);
     Task<bool> GetApiUserAsync(string apiUsername, string apiPassword, string clientID, string clientSecret, ApiUserDto user);
     Task<CardTransaction?> GetCardTransaction(int transactionId);
     Task<CardTransaction?> GetCardTransaction(int transactionId, string providerId);
     Task ConfirmSpend(int transactionId, int userId);
     Task CancelSpend(int transactionId, int userId);
     Task CancelAllTimedoutSpends();
     Task<CardTransaction?> GetCardTransaction(string providerId);
     Task<IncrementCardCreditApiResponseDto> IncrementCardCreditApi(SpendApiRequestDto request, int userId);
     Task<Person?> CreatePerson(Person person);
     Task UpdateMobile(int id, string mobile, int userId);
     Task SetCardOwner(int cardId, int ownerId);
     Task<List<CardResponse>> GetPersonCards(long? givId, string? mobileNo, int companyId);
}
