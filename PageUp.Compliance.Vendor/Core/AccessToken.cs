namespace PageUp.Compliance.Vendor.Core
{    
    public class AccessToken
    {
        public string ProtectedTicket { get; set; }
        public int ExpiresIn { get; set; }
        public string TokenType { get; set; }
        public string RefreshToken { get; set; }
    }
}