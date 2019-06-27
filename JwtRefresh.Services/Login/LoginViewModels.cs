using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace JwtRefresh.Services.Login
{
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AccessToken { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string RefreshToken { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Error { get; set; }
    }

    public class RefreshTokenRequest
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}
