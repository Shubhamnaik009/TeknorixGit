using Jobs.Models.Request;
using Jobs.Models.Response;
using System.ComponentModel.DataAnnotations;

namespace jobs.Models.Response
{
    public class JobListResponse : ResponseBase
    {
        public JobListResponse() { }
        public JobListResponse(Exception ex) : base(ex) { }
        public List<JobList> Jobs { get; set; }
        public int TotalCount { get; set; }
        public bool IsSuccess {get; set;}
   
    }

    public class JobList
    {
        [Key]
        public int JobId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string LocationName { get; set; }
        public string DepartmentName { get; set; }
        public string LocationAdress {  get; set; }
        public DateTime ClosingDate { get; set; }
    }
}
