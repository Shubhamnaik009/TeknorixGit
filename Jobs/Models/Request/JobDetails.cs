
using System.ComponentModel.DataAnnotations;

namespace Jobs.Models.Request
{
    public class JobDetails
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "LocationId is required")]
        public string LocationId { get; set; }

        [Required(ErrorMessage = "DepartmentId is required")]
        public string DepartmentId { get; set; }
        [Required(ErrorMessage = "ClosingDate is required")]
        public DateTime? ClosingDate { get; set; }
    }

    public class JobDetailsUpdate
    {
        [Required(ErrorMessage = "JobId is required")]
        public string JobId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string LocationId { get; set; }
        public string DepartmentId { get; set; }
        public string ClosingDate { get; set; }
    }
    public class LocationDetails
    {
        [Required(ErrorMessage = "LocationName is required")]
        public string LocationName { get; set; }
        [Required(ErrorMessage = "LocationAddress is required")]
        public string LcoationAddress { get; set; }
    }

    public class DepartmentDetails
    {
        [Required(ErrorMessage = "DepartmentName is required")]
        public string DepartmentName { get; set; }
    }


    public class LocationDetailsUpdate
    {
        [Required(ErrorMessage = "LocId is required")]
        public int LocId { get; set; }
        public string LocationName { get; set; }
        public string LcoationAddress { get; set; }
    }

    public class DepartmentDetailsUpdate
    {
        [Required(ErrorMessage = "DeptId is required")]
        public int DeptId { get; set; }

        [Required(ErrorMessage = "DepartmentName is required")]
        public string DepartmentName { get; set; }
    }


}
