namespace TwoStepAuthentication.Models
{
    public class ResponseData<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public ResponseData()
        {
        }

        public ResponseData(bool success, string message, T data)
        {
            Success = success;
            Message = message;
            Data = data;
        }
    }

}
