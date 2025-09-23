namespace User_Management.Model
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
        public LoginTokenModel obj { get; set; }
    }

    public class LoginTokenModel 
    {
        public string? token { get; set; }
        public DateTime? expiration { get; set; }
    }
}
