namespace SematecCC.Core.Services;

public class MyServicesBase
{
    protected OperationResultDto Fail(string message, string propertyName = "", int statusCode=-1)//200 ok
    {
        return new OperationResultDto()
        {
            Message = message,
            PropertyName = propertyName,
            Success = false
        };
    }

}
