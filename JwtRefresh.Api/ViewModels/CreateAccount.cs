using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;

namespace JwtRefresh.Api.ViewModels
{
    public class CreateAccountRequest
    {
        [Required]
        [MinLength(5)]
        public string Username { get; set; }
        [Required]
        [MinLength(8)]
        public string Password { get; set; }
    }

    public class CreateAccountResponse
    {
        public ObjectId Id { get; set; }
        public string Username { get; set; }
    }
}
