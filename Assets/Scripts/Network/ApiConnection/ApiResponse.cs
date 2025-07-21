using System.Collections.Generic;

namespace Network
{
    public class ApiError
    {
        public string code { get; set; }
        public string message { get; set; }
        public string field { get; set; }
    }
    
    public class ApiResponse
    {
        public const string ResponseKey = "res";
        
        public bool ok;
        public ApiError error { get; set; }
        public Dictionary<string, object> data;
    }
}
