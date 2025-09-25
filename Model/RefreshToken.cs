namespace User_Management.Model
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public bool IsRevoked { get; set; } = false;
        public DateTime ExpiryDate { get; set; }
    }
}
