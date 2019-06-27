namespace JwtRefresh.Models
{
    public class Account : ModelBase
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
    }
}
