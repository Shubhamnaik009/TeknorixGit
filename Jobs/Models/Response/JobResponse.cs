using Jobs.Models.Request;

namespace Jobs.Models.Response
{
    public class JobResponse : ResponseBase
    {
        public JobResponse() { }
        public JobResponse(Exception ex) : base(ex) { }

        public string JobId { get; set; }  
        public bool IsSuccess { get; set; }
    }

   
}
