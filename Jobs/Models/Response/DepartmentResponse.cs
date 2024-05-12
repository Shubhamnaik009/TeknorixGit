using Jobs.Models.Response;

namespace jobs.Models.Response
{
    public class DepartmentResponse : ResponseBase
    {
        public bool? IsSuccess { get; set; }
        public DepartmentResponse() { }
        public DepartmentResponse(Exception ex) : base(ex) { }
    }
}
