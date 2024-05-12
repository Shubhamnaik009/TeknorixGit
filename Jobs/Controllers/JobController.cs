using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Jobs.Models.Request;
using Jobs.Models.Response;
using Jobs.Repository;
using jobs.Models.Response;
using jobs.Models.Request;
using Microsoft.AspNetCore.Authorization;

namespace Job.Controllers
{
    [ApiController]
 
    [Route("api/v1/[controller]")]
    public class JobController : ControllerBase
    {
        private readonly DbDataFetching _objDb;

        public JobController(IConfiguration configuration)
        {   
            _objDb = new DbDataFetching(configuration);
        }

        [Authorize]
        [HttpPost("Addjobs")]
        public async Task<ActionResult<JobResponse>> InsertJobs([FromBody] JobDetails request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                int jobId = await _objDb.InsertUser(request);
                var response = new JobResponse
                {
                    Message = "Job inserted successfully.",
                    JobId = jobId.ToString(),
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new JobResponse(ex);
                return StatusCode(500, response);
            }
        }

        [HttpPost("Updatejobs")]
        public async Task<ActionResult<JobResponse>> UpdateJobs([FromBody] JobDetailsUpdate request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (!string.IsNullOrEmpty(request.ClosingDate))
                {
                    if (DateTime.TryParse(request.ClosingDate, out DateTime parsedDate))
                    {
                        request.ClosingDate = parsedDate.ToString();
                    }
                    else
                    {
                        
                        throw new ArgumentException("Invalid date format for ClosingDateString");
                    }  
                }
                    var response = await _objDb.UpdateJob(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new JobResponse(ex);
                return StatusCode(500, response);
            }
        }

        [HttpPost("SearchJobById")]
        public async Task<ActionResult<JobResponse>> GetJobById(string JobId)
        {
            try
            {
                if(string.IsNullOrEmpty(JobId))
                {
                    return new JobResponse
                    {
                        IsSuccess = false,
                        Message = "job Id required"
                    };
                }    
                var response = await _objDb.GetJobById(Convert.ToInt32(JobId));
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new JobResponse(ex);
                return StatusCode(500, response);
            }
        }

        [HttpPost("ListJobs")]
        public async Task<ActionResult<JobResponse>> GetJobListAsync([FromBody] JobListRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var response = await _objDb.GetJobListAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new JobResponse(ex);
                return StatusCode(500, response);
            }
        }

        [HttpPost("InsertLocation")]
        public async Task<ActionResult<LocationResponse>> InsertLocation([FromBody] LocationDetails request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var response = await _objDb.InsertLocation(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = new LocationResponse(ex);
                return StatusCode(500, errorResponse);
            }
        }

        [HttpPost("UpdateLocation")]
        public async Task<ActionResult<LocationResponse>> UpdateLocation([FromBody] LocationDetailsUpdate request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var response = await _objDb.UpdateLocation(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = new LocationResponse(ex);
                return StatusCode(500, errorResponse);
            }
        }

        [HttpGet("ListLocations")]
        public async Task<ActionResult<List<Location>>> ListLocations()
        {
            try
            {
                var locations = await _objDb.ListLocations();
                return Ok(locations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("InsertDepartment")]
        public async Task<ActionResult<DepartmentResponse>> InsertDepartment([FromBody] DepartmentDetails request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var response = await _objDb.InsertDepartment(request);
                return Ok(response) ;
            }
            catch (Exception ex)
            {
                var errorResponse = new DepartmentResponse(ex);
                return errorResponse;
            }
        }

        [HttpPost("UpdateDepartment")]
        public async Task<ActionResult<DepartmentResponse>> UpdateDepartment([FromBody] DepartmentDetailsUpdate request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var response = await _objDb.UpdateDepartment(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = new DepartmentResponse(ex);
              return errorResponse;
            }
        }

        [HttpGet("ListDepartments")]
        public async Task<ActionResult<List<Department>>> ListDepartments()
        {
            try
            {
                var departments = await _objDb.ListDepartments();
                return Ok(departments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
