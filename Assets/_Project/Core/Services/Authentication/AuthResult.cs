namespace ProjectATLAS.Authentication
{
    /// <summary> Authentication result. </summary>
    public struct AuthResult
    {
        public int Status { get; private set; }
        public string Message { get; private set; }
        
        public AuthResult(int status, string message)
        {
            Status = status;
            Message = message;
        }
    }
    
    public static class NetworkResult
    {
        public const int Offline = 000;
        public const int Online = 001;
        public const int NoInternetAccess = 002;
    }
    
    public static class RegistrationResult
    {
        public const int Unknown = 100;
        public const int Success = 101;
        public const int InvalidCredentials = 102;
        public const int AuthenticationFailed = 103;
        public const int RequestFailed = 104;
    }
    
    public static class LoginResult
    {
        public const int Unknown = 200;
        public const int Success = 201;
        public const int InvalidUsernameOrPassword = 202;
        public const int AuthenticationFailed = 203;
        public const int RequestFailed = 204;
        public const int NoSessionToken = 205;
    }
    
    public static class LogoutResult
    {
        public const int Unknown = 300;
        public const int Success = 301;
    }
    
    public static class UpdatePasswordResult
    {
        public const int Unknown = 400;
        public const int Success = 401;
        public const int InvalidPassword = 402;
        public const int SamePassword = 403;
        public const int AuthenticationFailed = 404;
        public const int RequestFailed = 405;
    }
    
    public static class UpdatePlayerNameResult
    {
        public const int Unknown = 500;
        public const int Success = 501;
        public const int InvalidPlayerName = 502;
        public const int AuthenticationFailed = 503;
        public const int RequestFailed = 504;
    }
    
    public static class DeleteAccountResult
    {
        public const int Unknown = 600;
        public const int Success = 601;
        public const int AuthenticationFailed = 602;
        public const int RequestFailed = 603;
    }
}
