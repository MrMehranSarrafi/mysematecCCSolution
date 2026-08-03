using Application.DTO;

namespace Core.Helpers;

public static class OperationResultHelper
{
    //OprResHelper
    public static OperationResultDto Fail(string message, string propertyName = "")
    {
        return new OperationResultDto
        {
            Success = false,
            Message = message,
            PropertyName = propertyName
        };
    }

    public static OperationResultDto Success(string message, string propertyName = "")
    {
        return new OperationResultDto
        {
            Success = true,
            Message = message,
            PropertyName = propertyName
        };
    }
}
