namespace SocialNetwork.Models
{
    public class LoginRequestModel
    {
        public string Id { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponseModel
    {
        public string Token { get; set; }
    }
}
