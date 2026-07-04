namespace IMBP.App.Domain.Models
{
    public class ServiceError(string errorCode)
    {
        public string ErrorCode { get; set; } = errorCode;
    }
}
