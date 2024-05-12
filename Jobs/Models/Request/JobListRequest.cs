using System.ComponentModel.DataAnnotations;

namespace jobs.Models.Request
{
    public class JobListRequest
    {
        [Required(ErrorMessage = "Query is required")]
        public string q { get; set; }

        [Required(ErrorMessage = "pageNo is required")]
        public string? pageNo { get; set; }

        [Required(ErrorMessage = "pageSize is required")]
        public string? pageSize { get; set; }
        public string? locationId { get; set; }
        public string? departmentId { get; set; }
    }
}
