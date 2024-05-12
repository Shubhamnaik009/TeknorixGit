using Jobs.Models.Response;

namespace jobs.Models.Response
{

    public class JobDetailsResponse : ResponseBase
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Location Location { get; set; }
        public Department Department { get; set; }
        public DateTime PostedDate { get; set; }
        public DateTime ClosingDate { get; set; }
        public JobDetailsResponse() { }
        public JobDetailsResponse(Exception ex) : base(ex) { }
    }

    public class Location
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class Department
    {
        public int Id { get; set; }
        public string DepartmentName { get; set; }
    }
}

