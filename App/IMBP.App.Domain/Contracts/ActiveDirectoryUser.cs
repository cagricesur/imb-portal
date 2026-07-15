namespace IMBP.App.Domain.Contracts
{
    public class ActiveDirectoryUser
    {
        public required string UserName { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? MiddleName { get; set; }
        public string? EmployeeId { get; set; }
    }
}
