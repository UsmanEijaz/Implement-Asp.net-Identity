namespace User_Management.ViewModel
{
    public class ResponseModel
    {
        public string? status { get; set; }
        public string? message { get; set; }
    }

    public class LoginResponseModel
    {
        public string? status { get; set; }
        public string? message { get; set; }
        public TokenResponseModel obj { get; set; }
    }

    //public class LoginTokenModel
    //{
    //    public string? token { get; set; }
    //    public DateTime? expiration { get; set; }
    //}

    public class TokenResponseModel
    {
        public string AccessToken { get; set;}
        public string RefreshToken { get; set; }
        public DateTime Expiration { get; set; }
    }
}
