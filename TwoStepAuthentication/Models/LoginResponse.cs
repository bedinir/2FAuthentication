namespace TwoStepAuthentication.Models
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool Is2FAEnabled { get; set; }
    }
}
