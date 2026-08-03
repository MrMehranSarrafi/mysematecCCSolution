using Application.DTO.ApiDTOs;
using Application.DTO.ApiDTOs.Responses;
using Application.DTO.UserDtos;
using Core.Domain.Entities;
using Core.Domain.RepositoryContracts;
using Core.Enums;
using Core.Helpers;

namespace Core.Services;

public class PaymentApiService//: MyServicesBase
{
    private readonly IPaymentApiRepo _paymentApiRepo;
    //private readonly CardsManagementService _cardManagementService;
    public PaymentApiService(IPaymentApiRepo paymentApiRepo/*, CardsManagementService cardManagementService*/)
    {
        _paymentApiRepo = paymentApiRepo;
        // _cardManagementService = cardManagementService;
    }

    public async Task<Card?> GetCardAsync(string cardNumber, string password)
    {
        return await _paymentApiRepo.GetCardAsync(cardNumber, password);
    }
    //public async Task<ApiOperationResultDto<decimal>> GetRemainedAmount(string cardNumber, string password, int roleId, int companyId)
    public async Task<ApiResponseDto<CardCreditApiResponseDto>> GetRemainedAmount(string cardNumber, int roleId, int companyId)
    {
        //var result = new ApiOperationResultDto<decimal>();
        var result = new ApiResponseDto<CardCreditApiResponseDto>();
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            result.Response.Succeeded = false;
            result.Response.StatusCode = (int)HttpStatusCodeEnum.UnprocessableEntity;
            result.Response.Error = new Error()
            {
                Code = (byte)PaymentApiErrorCodes.InvalidInput,
                ErrorMessage = ".شماره کارت وارد نشده است"
            };
            return result;
        }
        try
        {
            //var card = await _cardManagementService.GetCardAsync(cardNumber, password);
            var card = await _paymentApiRepo.GetCardAsync(cardNumber);
            if (card == null)
            {
                result.Response.Succeeded = false;
                result.Response.StatusCode = (int)HttpStatusCodeEnum.NotFound;//404;
                result.Response.Error = new Error()
                {
                    Code = (byte)PaymentApiErrorCodes.CardNotFound,
                    ErrorMessage = PaymentApiErrorCodes.CardNotFound.GetDescriptionAttributeValue()
                };
                return result;
            }
            if (roleId != 1 && card.CompanyId != companyId)
            {
                result.Response.Succeeded = false;
                result.Response.StatusCode = (int)HttpStatusCodeEnum.Forbidden;//403;
                result.Response.Error = new Error()
                {
                    Code = (byte)PaymentApiErrorCodes.CardForbidden,
                    ErrorMessage = PaymentApiErrorCodes.CardForbidden.GetDescriptionAttributeValue()
                };
                return result;
            }
            if (card.IsActive == false || card.CardOrder.IsActive == false)
            {
                result.Response.Succeeded = false;
                result.Response.StatusCode = (int)HttpStatusCodeEnum.Conflict;//409;
                result.Response.Error = new Error()
                {
                    Code = (byte)PaymentApiErrorCodes.CardIsDisabled,
                    ErrorMessage = PaymentApiErrorCodes.CardIsDisabled.GetDescriptionAttributeValue()
                };
                return result;
            }

            if (card.ExpireDate.HasValue)
            {
                if (card.ExpireDate.Value.Date < DateTime.Now.Date)
                {
                    result.Response.Succeeded = false;
                    result.Response.Error = new Error
                    {
                        Code = (int)PaymentApiErrorCodes.CardExpired,
                        ErrorMessage = PaymentApiErrorCodes.CardExpired.GetDescriptionAttributeValue()
                    };
                    result.Response.StatusCode = (int)HttpStatusCodeEnum.Conflict;
                    return result;
                }
            }

            result.Response.Succeeded = true;
            result.Response.Message = "Ok";
            result.Response.StatusCode = 200;
            result.Data = new CardCreditApiResponseDto()
            {
                Credit = card.RemainedAmount,
                HasCredit = card.RemainedAmount > 0,
                IsActive = true
            };
        }
        catch (Exception ex)
        {
            result.Response.Succeeded = false;
            result.Response.StatusCode = 500;
            result.Response.Error = new Error()
            {
                Code = (byte)PaymentApiErrorCodes.InternalError,
                ErrorMessage = ex.Message + "\n" + " خطا رخ داد"
            };
        }
        return result;
    }
    #region Spend
    public async Task<ApiResponseDto<SpendApiResponseDto>> Spend(SpendApiRequestDto request, int roleId, int companyId, int userId)
    {
        var result = new ApiResponseDto<SpendApiResponseDto>();
        try
        {
            //In JWT:  اصلا ما با کوکی کاری نداریم و بنابراین با _userContext کاری نداریم 
            //از توکن  موارد را می خوانیم
            //var card = await _cardManagementService.GetCardAsync(request.CardNumber, request.CardPassword);
            if (string.IsNullOrWhiteSpace(request.CardNumber) || request.CardPassword.Length != 5)
            {
                result.Response.Succeeded = false;
                result.Response.StatusCode = (int)HttpStatusCodeEnum.UnprocessableEntity;
                result.Response.Error = new Error()
                {
                    Code = (byte)PaymentApiErrorCodes.InvalidInput,
                    ErrorMessage = "شماره کارت و پسورد را به درستی وارد نمایید."
                };
                return result;
            }

            var card = await _paymentApiRepo.GetCardAsync(request.CardNumber, request.CardPassword);
            if (card == null)
            {
                result.Response.Succeeded = false;
                result.Response.StatusCode = (int)HttpStatusCodeEnum.NotFound;//404;
                result.Response.Error = new Error()
                {
                    Code = (byte)PaymentApiErrorCodes.CardNotFound,
                    ErrorMessage = PaymentApiErrorCodes.CardNotFound.GetDescriptionAttributeValue()
                };
                return result;
            }
            if (card.IsActive == false || card.CardOrder.IsActive == false)
            {
                result.Response.Succeeded = false;
                result.Response.StatusCode = (int)HttpStatusCodeEnum.Conflict;//409;
                result.Response.Error = new Error()
                {
                    Code = (byte)PaymentApiErrorCodes.CardIsDisabled,
                    ErrorMessage = PaymentApiErrorCodes.CardIsDisabled.GetDescriptionAttributeValue()
                };
                return result;
            }
            if (roleId != 1 && card.CompanyId != companyId)
            {
                result.Response.Succeeded = false;
                result.Response.StatusCode = (int)HttpStatusCodeEnum.Forbidden;//403;
                result.Response.Error = new Error()
                {
                    Code = (byte)PaymentApiErrorCodes.CardForbidden,
                    ErrorMessage = PaymentApiErrorCodes.CardForbidden.GetDescriptionAttributeValue()
                };
                return result;
            }

            if (card.RemainedAmount < request.Amount)
            {
                result.Response.Succeeded = false;
                result.Response.Error = new Error()
                {
                    Code = (int)PaymentApiErrorCodes.InsufficientBalance,
                    ErrorMessage = PaymentApiErrorCodes.InsufficientBalance.GetDescriptionAttributeValue()//"اعتبار کارت کافی نیست"
                };
                result.Response.StatusCode = (int)HttpStatusCodeEnum.Conflict;
                return result;
            }
            var cardTransaction = await _paymentApiRepo.GetCardTransaction(request.ProviderId);//ما فقط همین شرکت را بررسی می کنیم
            if (cardTransaction != null && cardTransaction.CardId == card.Id && cardTransaction.Status != CardTransactionsStatus.NewOrInitial)
            {
                // خرید با این شناسه وجود دارد
                result.Response.Succeeded = false;
                result.Response.Error = new Error()
                {
                    Code = (int)PaymentApiErrorCodes.SpendAlreadyExists,
                    ErrorMessage = PaymentApiErrorCodes.SpendAlreadyExists.GetDescriptionAttributeValue(),//خرید موجود
                    CurrentStatusFa = cardTransaction.Status.GetDisplayAttributeValue(),
                    CurrentStatus = cardTransaction.Status.GetEnumName()
                };
                result.Response.StatusCode = (int)HttpStatusCodeEnum.Conflict;
                result.Data = new SpendApiResponseDto()
                {
                    Amount = cardTransaction.Card.Amount,
                    TrackingCode = cardTransaction.Id,
                    CompanyInfo = cardTransaction.Card.Company?.CompanyInfo
                };
                return result;

            }
            if (cardTransaction != null && cardTransaction.CardId == card.Id && cardTransaction.Status == CardTransactionsStatus.NewOrInitial)
            {
                result.Response.Succeeded = false;
                result.Response.Error = new Error()
                {
                    Code = (int)PaymentApiErrorCodes.RepeatedSpend,
                    ErrorMessage = PaymentApiErrorCodes.RepeatedSpend.GetDescriptionAttributeValue(),//خرید تکراری
                    CurrentStatusFa = cardTransaction.Status.GetDisplayAttributeValue(),
                    CurrentStatus = cardTransaction.Status.GetEnumName()
                };
                result.Response.StatusCode = (int)HttpStatusCodeEnum.Conflict;
                result.Data = new SpendApiResponseDto()
                {
                    Amount = cardTransaction.Card.Amount,
                    TrackingCode = cardTransaction.Id,
                    CompanyInfo = cardTransaction.Card.Company?.CompanyInfo
                };
                return result;

            }
            if (card.ExpireDate.HasValue)
            {
                if (card.ExpireDate.Value.Date < DateTime.Now.Date)
                {
                    result.Response.Succeeded = false;
                    result.Response.Error = new Error
                    {
                        Code = (int)PaymentApiErrorCodes.CardExpired,
                        ErrorMessage = PaymentApiErrorCodes.CardExpired.GetDescriptionAttributeValue()
                    };
                    result.Response.StatusCode = (int)HttpStatusCodeEnum.Conflict;
                    return result;
                }
            }
            result.Data = await _paymentApiRepo.Spend(request, userId);//referenceId is set within it. ?
            //await _paymentApiRepo.SaveChangesAsync();


            result.Response.Succeeded = true;
            result.Response.Message = " مبلغ مذکور با موفقیت خرج شد.  ";
            //result.Data.Amount = request.Amount;
            //result.Data.TrackingCode= // داخل متد ذخیره لایه دیتا مقدار داده ام.
            result.Response.StatusCode = 200;
        }
        catch (Exception ex)
        {
            result.Response.Succeeded = false;
            result.Response.StatusCode = 500;
            result.Response.Error = new Error()
            {
                Code = (byte)PaymentApiErrorCodes.InternalError,
                ErrorMessage = ex.Message + "\n" + " خطا رخ داد"
            };
        }
        return result;
    }
    #endregion
    #region ConfirmSpend
    public async Task<ApiResponseDto<ConfirmSpendResponseDto>> ConfirmSpend(int transactionId, string providerId, int roleId, int companyId, int userId)
    {
        var result = new ApiResponseDto<ConfirmSpendResponseDto>();
        CardTransaction cardTransaction = null;
        cardTransaction = await _paymentApiRepo.GetCardTransaction(transactionId, providerId);

        if (cardTransaction == null)
        {
            result.Response.Error = new Error()
            {
                Code = (int)PaymentApiErrorCodes.PurchaseNotFound,
                ErrorMessage = PaymentApiErrorCodes.PurchaseNotFound.GetDescriptionAttributeValue()
            };
            result.Response.StatusCode = (int)HttpStatusCodeEnum.NotFound;
            result.Response.Succeeded = false;
            return result;
        }
        if (roleId != 1 && cardTransaction.Card.CompanyId != companyId)
        {
            result.Response.Succeeded = false;
            result.Response.Error = new Error()
            {
                Code = (int)PaymentApiErrorCodes.CardForbidden,
                ErrorMessage = PaymentApiErrorCodes.CardForbidden.GetDescriptionAttributeValue()
            };
            result.Response.StatusCode = (int)HttpStatusCodeEnum.Forbidden;
            return result;
        }
        if (cardTransaction.Status != CardTransactionsStatus.NewOrInitial)
        {
            result.Response.Succeeded = false;
            result.Response.Error = new Error()
            {
                Code = (int)PaymentApiErrorCodes.OperationNotAllowed,
                ErrorMessage = PaymentApiErrorCodes.OperationNotAllowed.GetDescriptionAttributeValue()
                ,
                CurrentStatus = cardTransaction.Status.ToString()
                ,
                CurrentStatusFa = cardTransaction.Status.GetDisplayAttributeValue()
            };
            result.Response.StatusCode = (int)HttpStatusCodeEnum.Conflict;
            return result;
        }
        if (cardTransaction.Card.ExpireDate.HasValue)
        {
            if (cardTransaction.Card.ExpireDate.Value.Date < DateTime.Now.Date)
            {
                result.Response.Succeeded = false;
                result.Response.Error = new Error
                {
                    Code = (int)PaymentApiErrorCodes.CardExpired,
                    ErrorMessage = PaymentApiErrorCodes.CardExpired.GetDescriptionAttributeValue()
                };
                result.Response.StatusCode = (int)HttpStatusCodeEnum.Conflict;
                return result;
            }
        }
        try
        {
            await _paymentApiRepo.ConfirmSpend(transactionId, userId);
            await _paymentApiRepo.SaveChangesAsync();
            result.Response.Succeeded = true;
            result.Response.Message = $"شناسه خرید {cardTransaction.ProviderId} با موفقیت تایید و نهایی شد.";
            result.Response.StatusCode = (int)HttpStatusCodeEnum.Ok;
            result.Data = new ConfirmSpendResponseDto()
            {
                ConfirmDate = DateTime.Now.ToPersian(),
                TrackingCode = transactionId,
                ProviderId = providerId
            };

        }
        catch (Exception)
        {
            result.Response.Error = new Error()
            {
                Code = (int)PaymentApiErrorCodes.InternalError,
                ErrorMessage = PaymentApiErrorCodes.InternalError.GetDescriptionAttributeValue()
            };
            result.Response.StatusCode = (int)HttpStatusCodeEnum.InternalServerError;
            result.Response.Succeeded = false;
        }
        return result;
    }
    #endregion ConfirmSpend
    #region CancelSpend
    public async /*Task<ApiCommonResponseDto> برای یکنواخت کردن خروجی Api ها*/ Task<ApiResponseDto<object>> CancelSpend(int transactionId, string providerId, int roleId, int companyId, int userId)
    {
        var result = new ApiResponseDto<object>();
        CardTransaction? cardTransaction = null;
        if (!string.IsNullOrWhiteSpace(providerId))
            cardTransaction = await _paymentApiRepo.GetCardTransaction(transactionId, providerId.Trim());
        else
            cardTransaction = await _paymentApiRepo.GetCardTransaction(transactionId);
        if (cardTransaction == null)
        {
            result.Response.Error = new Error()
            {
                Code = (int)PaymentApiErrorCodes.PurchaseNotFound,
                ErrorMessage = PaymentApiErrorCodes.PurchaseNotFound.GetDescriptionAttributeValue()
            };
            result.Response.StatusCode = (int)HttpStatusCodeEnum.NotFound;
            result.Response.Succeeded = false;
            return result;
        }
        if (roleId != 1 && cardTransaction.Card.CompanyId != companyId)
        {
            result.Response.Succeeded = false;
            result.Response.Error = new Error()
            {
                Code = (int)PaymentApiErrorCodes.CardForbidden,
                ErrorMessage = PaymentApiErrorCodes.CardForbidden.GetDescriptionAttributeValue()
            };
            result.Response.StatusCode = (int)HttpStatusCodeEnum.Forbidden;
            return result;
        }
        if (cardTransaction.Status != CardTransactionsStatus.NewOrInitial)
        {
            result.Response.Succeeded = false;
            result.Response.Error = new Error()
            {
                Code = (int)PaymentApiErrorCodes.OperationNotAllowed,
                ErrorMessage = PaymentApiErrorCodes.OperationNotAllowed.GetDescriptionAttributeValue()
                ,
                CurrentStatus = cardTransaction.Status.ToString()
                ,
                CurrentStatusFa = cardTransaction.Status.GetDisplayAttributeValue()
            };
            result.Response.StatusCode = (int)HttpStatusCodeEnum.Conflict;
            return result;
        }
        if (cardTransaction.Card.ExpireDate.HasValue)
        {
            if (cardTransaction.Card.ExpireDate.Value.Date < DateTime.Now.Date)
            {
                result.Response.Succeeded = false;
                result.Response.Error = new Error
                {
                    Code = (int)PaymentApiErrorCodes.CardExpired,
                    ErrorMessage = PaymentApiErrorCodes.CardExpired.GetDescriptionAttributeValue()
                };
                result.Response.StatusCode = (int)HttpStatusCodeEnum.Conflict;
                return result;
            }
        }
        try
        {
            await _paymentApiRepo.CancelSpend(transactionId, userId);
            //await _paymentApiRepo.SaveChangesAsync();
            result.Response.Succeeded = true;
            result.Response.Message = $"شناسه خرید {cardTransaction.ProviderId} با موفقیت لغو شد.";
            result.Response.StatusCode = (int)HttpStatusCodeEnum.Ok;
        }
        catch (Exception)
        {
            result.Response.Error = new Error()
            {
                Code = (int)PaymentApiErrorCodes.InternalError,
                ErrorMessage = PaymentApiErrorCodes.InternalError.GetDescriptionAttributeValue()
            };
            result.Response.StatusCode = (int)HttpStatusCodeEnum.InternalServerError;
            result.Response.Succeeded = false;
        }
        return result;
    }
    #endregion CancelSpend
    public async Task<ApiResponseDto<IncrementCardCreditApiResponseDto>> IncrementCardCreditApi(SpendApiRequestDto request, int roleId, int companyId, int userId)
    {
        var result = new ApiResponseDto<IncrementCardCreditApiResponseDto>();
        try
        {
            if (string.IsNullOrWhiteSpace(request.CardNumber) || string.IsNullOrWhiteSpace(request.CardPassword) || request.CardPassword.Length != 5 || string.IsNullOrWhiteSpace(request.Description))
            {
                result.Response.Succeeded = false;
                result.Response.StatusCode = (int)HttpStatusCodeEnum.UnprocessableEntity;
                result.Response.Error = new Error()
                {
                    Code = (byte)PaymentApiErrorCodes.InvalidInput,
                    ErrorMessage = "شماره کارت و پسورد و توضیحات را به درستی وارد نمایید."
                };
                return result;
            }

            var card = await _paymentApiRepo.GetCardAsync(request.CardNumber, request.CardPassword);
            if (card == null)
            {
                result.Response.Succeeded = false;
                result.Response.StatusCode = (int)HttpStatusCodeEnum.NotFound;//404;
                result.Response.Error = new Error()
                {
                    Code = (byte)PaymentApiErrorCodes.CardNotFound,
                    ErrorMessage = PaymentApiErrorCodes.CardNotFound.GetDescriptionAttributeValue()
                };
                return result;
            }
            if (card.IsActive == false || card.CardOrder.IsActive == false)
            {
                result.Response.Succeeded = false;
                result.Response.StatusCode = (int)HttpStatusCodeEnum.Conflict;//409;
                result.Response.Error = new Error()
                {
                    Code = (byte)PaymentApiErrorCodes.CardIsDisabled,
                    ErrorMessage = PaymentApiErrorCodes.CardIsDisabled.GetDescriptionAttributeValue()
                };
                return result;
            }
            if (roleId != 1 && card.CompanyId != companyId)
            {
                result.Response.Succeeded = false;
                result.Response.StatusCode = (int)HttpStatusCodeEnum.Forbidden;//403;
                result.Response.Error = new Error()
                {
                    Code = (byte)PaymentApiErrorCodes.CardForbidden,
                    ErrorMessage = PaymentApiErrorCodes.CardForbidden.GetDescriptionAttributeValue()
                };
                return result;
            }

            if (card.ExpireDate.HasValue)
            {
                if (card.ExpireDate.Value.Date < DateTime.Now.Date)
                {
                    result.Response.Succeeded = false;
                    result.Response.Error = new Error
                    {
                        Code = (int)PaymentApiErrorCodes.CardExpired,
                        ErrorMessage = PaymentApiErrorCodes.CardExpired.GetDescriptionAttributeValue()
                    };
                    result.Response.StatusCode = (int)HttpStatusCodeEnum.Conflict;
                    return result;
                }
            }
            result.Data = await _paymentApiRepo.IncrementCardCreditApi(request, userId);

            result.Response.Succeeded = true;
            result.Response.Message = $" کارت شماره {card.CardNo} با موفقیت، مبلغ {request.Amount} ،افزایش اعتبار یافت.";
            result.Response.StatusCode = 200;
        }
        catch (Exception ex)
        {
            result.Response.Succeeded = false;
            result.Response.StatusCode = 500;
            result.Response.Error = new Error()
            {
                Code = (byte)PaymentApiErrorCodes.InternalError,
                ErrorMessage = ex.Message + "\n" + " خطا رخ داد"
            };
        }
        return result;
    }

    public async Task<bool> GetApiUser(string apiUsername, string apiPassword, string clientID, string clientSecret, ApiUserDto user)
    {
        return await _paymentApiRepo.GetApiUserAsync(apiUsername, apiPassword, clientID, clientSecret, user);
    }
    public async Task<ApiResponseDto<CardStatusApiResponseDto>> CardStatusApi(string cardNumber, int roleId, int companyId)
    {
        var result = new ApiResponseDto<CardStatusApiResponseDto>();
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            result.Response.Succeeded = false;
            result.Response.StatusCode = (int)HttpStatusCodeEnum.UnprocessableEntity;
            result.Response.Error = new Error()
            {
                Code = (byte)PaymentApiErrorCodes.InvalidInput,
                ErrorMessage = ".شماره کارت وارد نشده است"
            };
            return result;
        }
        try
        {
            //var card = await _cardManagementService.GetCardAsync(cardNumber, password);
            var card = await _paymentApiRepo.GetCardAsync(cardNumber);
            if (card == null)
            {
                result.Response.Succeeded = false;
                result.Response.StatusCode = (int)HttpStatusCodeEnum.NotFound;//404;
                //result.Response.Message = "کارت با این مشخصات یافت نشد";
                //result.Data = null;
                result.Response.Error = new Error()
                {
                    Code = (byte)PaymentApiErrorCodes.CardNotFound,
                    ErrorMessage = PaymentApiErrorCodes.CardNotFound.GetDescriptionAttributeValue()
                };
                return result;
            }
            //از توکن  این مقادیر را به دست آورده ام-- نه از کوکی: 
            if (roleId != 1 && card.CompanyId != companyId)
            {
                result.Response.Succeeded = false;
                result.Response.StatusCode = (int)HttpStatusCodeEnum.Forbidden;//403;
                result.Response.Error = new Error()
                {
                    Code = (byte)PaymentApiErrorCodes.CardForbidden,
                    ErrorMessage = PaymentApiErrorCodes.CardForbidden.GetDescriptionAttributeValue()
                };
                return result;
            }
            ////
            result.Response.Succeeded = true;
            result.Response.Message = "Ok";
            result.Response.StatusCode = 200;
            result.Data = new CardStatusApiResponseDto();
            result.Data.Credit = card.RemainedAmount;
            result.Data.IsActive = card.IsActive && card.CardOrder.IsActive;
            result.Data.Owner = card.Owner == null ? null : $"{card.Owner?.Mobile} - {card.Owner?.FullName}";
            result.Data.ExpireDateFa = card.ExpireDateFa;
            result.Data.IsExpired = card.IsExpired;

        }
        catch (Exception ex)
        {
            result.Response.Succeeded = false;
            result.Response.StatusCode = 500;
            result.Response.Error = new Error()
            {
                Code = (byte)PaymentApiErrorCodes.InternalError,
                ErrorMessage = ex.Message + "\n" + " خطا رخ داد"
            };
        }
        return result;
    }

    public async Task<CardTransaction?> GetCardTransaction(int transactionId)
    {
        return await _paymentApiRepo.GetCardTransaction(transactionId);
    }


    public (string token, string refresh_token) GenerateToken(ApiUserDto user)
    {
        throw new NotImplementedException();
    }

    public async Task CancelAllTimedoutSpends()
    {
        await _paymentApiRepo.CancelAllTimedoutSpends();
    }

    #region SetCardOwner
    public async Task<ApiResponseDto<object>> SetCardOwner(SetCardOwnerRequest request, int roleId, int companyId, int userId)
    {
        var result = new ApiResponseDto<object>();
        try
        {
            if (string.IsNullOrWhiteSpace(request.CardNumber) || string.IsNullOrWhiteSpace(request.Password) || request.Password.Length != 5)
            {
                result.Response.Succeeded = false;
                result.Response.StatusCode = (int)HttpStatusCodeEnum.UnprocessableEntity;
                result.Response.Error = new Error()
                {
                    Code = (byte)PaymentApiErrorCodes.InvalidInput,
                    ErrorMessage = "شماره کارت و پسورد را وارد نمایید."
                };
                return result;
            }

            var card = await _paymentApiRepo.GetCardAsync(request.CardNumber, request.Password);
            if (card == null)
            {
                result.Response.Succeeded = false;
                result.Response.StatusCode = (int)HttpStatusCodeEnum.NotFound;//404;
                result.Response.Error = new Error()
                {
                    Code = (byte)PaymentApiErrorCodes.CardNotFound,
                    ErrorMessage = PaymentApiErrorCodes.CardNotFound.GetDescriptionAttributeValue()
                };
                return result;
            }
            if (card.IsActive == false || card.CardOrder.IsActive == false)
            {
                result.Response.Succeeded = false;
                result.Response.StatusCode = (int)HttpStatusCodeEnum.Conflict;//409;
                result.Response.Error = new Error()
                {
                    Code = (byte)PaymentApiErrorCodes.CardIsDisabled,
                    ErrorMessage = PaymentApiErrorCodes.CardIsDisabled.GetDescriptionAttributeValue()
                };
                return result;
            }
            if (roleId != 1 && card.CompanyId != companyId)
            {
                result.Response.Succeeded = false;
                result.Response.StatusCode = (int)HttpStatusCodeEnum.Forbidden;//403;
                result.Response.Error = new Error()
                {
                    Code = (byte)PaymentApiErrorCodes.CardForbidden,
                    ErrorMessage = PaymentApiErrorCodes.CardForbidden.GetDescriptionAttributeValue()
                };
                return result;
            }

            if (card.ExpireDate.HasValue)
            {
                if (card.ExpireDate.Value.Date < DateTime.Now.Date)
                {
                    result.Response.Succeeded = false;
                    result.Response.Error = new Error
                    {
                        Code = (int)PaymentApiErrorCodes.CardExpired,
                        ErrorMessage = PaymentApiErrorCodes.CardExpired.GetDescriptionAttributeValue()
                    };
                    result.Response.StatusCode = (int)HttpStatusCodeEnum.Conflict;
                    return result;
                }
            }
            //اگر از قبل مالک دارد، پیغام خطا بدهد:
            if (card.Owner != null)
            {
                result.Response.Succeeded = false;
                result.Response.Error = new Error
                {
                    Code = (int)PaymentApiErrorCodes.CardOwnerAlreadySet,
                    ErrorMessage = PaymentApiErrorCodes.CardOwnerAlreadySet.GetDescriptionAttributeValue()
                };
                result.Response.StatusCode = (int)HttpStatusCodeEnum.Conflict;
                return result;
            }

            //Gets or creates the person:
            Person? owner = await _paymentApiRepo.GetPerson(request.Person.GivId, request.Person.CompanyId);
            if (owner == null)//اگر پیدا نکرد ایجاد کرده و برگردان شناسه آن را
            {
                owner = new Person()
                {
                    BirthDateFa = request.Person.BirthDateFa,
                    BirthDate = request.Person.BirthDateFa.ToMiladi(),
                    FirstName = request.Person.FirstName,
                    LastName = request.Person.LastName,
                    GivId = request.Person.GivId,
                    CompanyId = request.Person.CompanyId,
                    JobPlace = request.Person.JobPlace,
                    DateCreated = DateTime.Now,
                    Mobile = request.Person.Mobile,
                    NationalCode = request.Person.NationalCode,
                    Phone = request.Person.Phone,
                    UserIdCreated = userId

                };
                owner = await _paymentApiRepo.CreatePerson(owner);//creates and returns it(with its id filled).
                await _paymentApiRepo.SaveChangesAsync();//Id is set after this line, اینو داخل متد قبلی هم می توانستیم بنویسیم
                var ownerId = owner.Id;//Ok, id is set in this line
            }
            else if (owner.Mobile.Trim() != request.Person.Mobile.Trim()) //if FOUND And MObiles not equal, then Update mobile
            {
                await _paymentApiRepo.UpdateMobile(owner.Id, request.Person.Mobile, userId);
            }
            //Set card Owner NOW:
            await _paymentApiRepo.SetCardOwner(card.Id, owner.Id);
            await _paymentApiRepo.SaveChangesAsync();

            result.Response.Succeeded = true;
            result.Response.Message = $" Ok  ";
            result.Response.StatusCode = 200;
        }
        catch (Exception ex)
        {
            //? حتما لاگ کن با تمام اطلاعات شخص
            result.Response.Succeeded = false;
            result.Response.StatusCode = 500;
            result.Response.Error = new Error()
            {
                Code = (byte)PaymentApiErrorCodes.InternalError,
                ErrorMessage = ex.Message + "\n" + " خطا رخ داد"
            };
        }
        return result;
    }
    #endregion SetCardOwner
    public async Task<ApiResponseDto<List<CardResponse>>> GetPersonCards(long? givId, string? mobileNo, int companyId)
    {
        var result = new ApiResponseDto<List<CardResponse>>();
        
        try
        {
            var list = await _paymentApiRepo.GetPersonCards(givId, mobileNo, companyId);
            result.Data =list;
            result.Response.StatusCode = 200;
            result.Response.Succeeded = true;
            result.Response.Message = "لیست کارت های فرد در این شرکت";

        }
        catch (Exception ex)
        {
            result.Response.Error = new Error() { Code = (byte)PaymentApiErrorCodes.InternalError, ErrorMessage = ex.Message + "\n" + "خطا رخ داد" };
            result.Response.StatusCode = 500;
            result.Response.Succeeded = false;
        }
        return result;
    }
}
