namespace Jobs.Models.Response
{
    public class ResponseBase
    {
        public ResponseBase()
        {
            StatusCode = 200;
        }

        public ResponseBase(Exception ex)
        {
            StatusCode = 401;
            ErrorMessage = ex.Message.ToString();
        }

        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public string Message { get; set; }

        public string StackTrace { get; set; }

    }
}
