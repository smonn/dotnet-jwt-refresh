using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace JwtRefresh.Services.Accounts
{
    public class AccountPublic
    {
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
    }

    public class CreateAccountRequest
    {
        [Required]
        [MinLength(5)]
        public string Username { get; set; }
        [Required]
        [MinLength(8)]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
        [Required]
        public string GivenName { get; set; }
        [Required]
        public string Surname { get; set; }
    }

    public class CreateAccountResponse
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public AccountPublic Account { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Error { get; set; }
    }

    public class GetAccountRequest
    {
        public ObjectId AccountId { get; set; }
    }

    public class GetAccountResponse
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public AccountPublic Account { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Error { get; set; }
    }
}
