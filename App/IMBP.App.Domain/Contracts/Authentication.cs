using IMBP.App.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace IMBP.App.Domain.Contracts
{
    public class AuthenticationRequest
    {
        [Required(ErrorMessage = "Validation.AuthenticationRequest.UserName.Required")]
        [StringLength(100, ErrorMessage = "Validation.AuthenticationRequest.UserName.MaxLength")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "Validation.AuthenticationRequest.Password.Required")]
        [StringLength(100, ErrorMessage = "Validation.AuthenticationRequest.Password.MaxLength")]
        public required string Password { get; set; }
    }

    public class AuthenticationResponse : ServiceResponse
    {
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }

        public string? FullName
        {
            get
            {
                var name = string.Join(" ", new string?[] { FirstName, MiddleName, LastName }.Where(entity => !string.IsNullOrWhiteSpace(entity))).Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    return null;
                }
                return name;
            }
        }

        /// <summary>
        /// JWT for the auth cookie only — never serialized to the client.
        /// </summary>
        [JsonIgnore]
        public string? Token { get; set; }
    }
}
