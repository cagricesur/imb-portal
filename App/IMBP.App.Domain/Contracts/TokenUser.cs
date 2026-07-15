namespace IMBP.App.Domain.Contracts
{
    public class TokenUser
    {
        public required Guid UID { get; set; }
        public required string UserName { get; set; }
        public required string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public required string LastName { get; set; }
        public required byte Role { get; set; }
    }
}
