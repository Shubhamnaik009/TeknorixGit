using Jobs.Models.Response;

namespace jobs.Models.Response
{
    public class LocationResponse : ResponseBase
    {
        public bool? IsSuccess { get; set; }
        public string LocationId { get; set; }  
        public LocationResponse() { }
        public LocationResponse(Exception ex) : base(ex) { }
    }
}
