namespace ProjectATLAS.CloudSave
{
    public struct CloudSaveResult
    {
        public int Status { get; private set; }
        public string Message { get; private set; }
        
        public CloudSaveResult(int status, string message)
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
    
    public static class LoadResult
    {
        public const int Unknown = 100;
        public const int Success = 101;
        public const int GameDataManagerNotAssigned = 102;
        public const int UGSNotInitialized = 103;
        public const int NotSignedIn = 104;
        public const int RequestFailed = 105;
        public const int DataNotFound = 106;
        public const int DeserializationFailed = 107;
    }
    
    public static class SaveResult
    {
        public const int Unknown = 200;
        public const int Success = 201;
        public const int GameDataManagerNotAssigned = 202;
        public const int NotSignedIn = 203;
        public const int UGSNotInitialized = 204;
        public const int RequestFailed = 205;
        public const int AutoSaveDisabled = 206;
    }
    
    public static class DeleteResult
    {
        public const int Unknown = 300;
        public const int Success = 301;
        public const int NotSignedIn = 302;
        public const int UGSNotInitialized = 303;
        public const int RequestFailed = 304;
    }
}
