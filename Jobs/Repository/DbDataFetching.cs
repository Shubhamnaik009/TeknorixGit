using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Azure;
using jobs.Models.Request;
using jobs.Models.Response;
using Jobs.Models.Request;
using Jobs.Models.Response;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Jobs.Repository
{
    public class DbDataFetching
    {

        private readonly string _connectionString = string.Empty;
        private readonly string _SPSInsertJob = string.Empty;
        private readonly string _SPUUpdateJob = string.Empty;
        private readonly string _SPGetJobList = string.Empty;
        public DbDataFetching(IConfiguration AppSettings)
        {
            _connectionString = AppSettings.GetConnectionString("DefaultConnection");
            _SPSInsertJob = AppSettings.GetValue<string>("StoredProcedures:SPSInsertJob");
            _SPUUpdateJob = AppSettings.GetValue<string>("StoredProcedures:SPUUpdateJob");
            _SPGetJobList = AppSettings.GetValue<string>("StoredProcedures:SPGetJobList");
        }

        public async Task<int> InsertUser(JobDetails user)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(_SPSInsertJob, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Title", user.Title);
                        command.Parameters.AddWithValue("@Description", user.Description);
                        command.Parameters.AddWithValue("@LocationId", user.LocationId);
                        command.Parameters.AddWithValue("@DepartmentId", user.DepartmentId);
                        command.Parameters.AddWithValue("@ClosingDate", user.ClosingDate);
                        var jobIdParam = new SqlParameter("@JobId", SqlDbType.Int);
                        jobIdParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(jobIdParam);
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            var jobId = Convert.ToInt32(jobIdParam.Value);
                            return jobId;
                        }
                        else
                        {
                            throw new Exception("failed to insert into Jobs");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<JobResponse> UpdateJob(JobDetailsUpdate Request)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("SPUUpdateJob", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@JobId", Request.JobId);
                        command.Parameters.AddWithValue("@Title", string.IsNullOrEmpty(Request.Title) ? DBNull.Value : (object)Request.Title);
                        command.Parameters.AddWithValue("@Description", string.IsNullOrEmpty(Request.Description) ? DBNull.Value : (object)Request.Description);
                        command.Parameters.AddWithValue("@LocationId", string.IsNullOrEmpty(Request.LocationId) ? DBNull.Value : (object)Request.LocationId);
                        command.Parameters.AddWithValue("@DepartmentId", string.IsNullOrEmpty(Request.DepartmentId) ? DBNull.Value : (object)Request.DepartmentId);
                        command.Parameters.AddWithValue("@ClosingDate", string.IsNullOrEmpty(Request.ClosingDate.ToString()) ? (object)Request.ClosingDate : DBNull.Value);
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected < 0)
                        {
                            var response = new JobResponse
                            {
                                IsSuccess = true,
                                JobId = Request.JobId,
                                Message = "Job details updated Failed"
                            };
                            return response;
                        }
                        else
                        {
                            var response = new JobResponse
                            {
                                IsSuccess = true,
                                JobId = Request.JobId,
                                Message = "Job details updated successfully"
                            };
                            return response;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<JobListResponse> GetJobListAsync(JobListRequest request)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("SPGetJobList", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@SearchString", request.q);
                        command.Parameters.AddWithValue("@PageNumber", Convert.ToInt32(request.pageNo));
                        command.Parameters.AddWithValue("@PageSize", Convert.ToInt32(request.pageSize));
                        if (!string.IsNullOrEmpty(request.locationId))
                        {
                            command.Parameters.AddWithValue("@LocationId", Convert.ToInt32(request.locationId));
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@LocationId", DBNull.Value);
                        }

                        if (!string.IsNullOrEmpty(request.departmentId))
                        {
                            command.Parameters.AddWithValue("@DepartmentId", Convert.ToInt32(request.departmentId));
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@DepartmentId", DBNull.Value);
                        }

                        var jobList = new List<JobList>();
                        var totalCount = 0;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                var job = new JobList
                                {
                                    JobId = reader.GetInt32(reader.GetOrdinal("JobId")),
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    Description = reader.GetString(reader.GetOrdinal("Description")),
                                    LocationName = reader.GetString(reader.GetOrdinal("LocationName")),
                                    LocationAdress = reader.GetString(reader.GetOrdinal("LocationAddress")),
                                    DepartmentName = reader.GetString(reader.GetOrdinal("DepartmentName")),
                                    ClosingDate = reader.GetDateTime(reader.GetOrdinal("ClosingDate"))
                                };
                                jobList.Add(job);
                            }


                            if (reader.NextResult() && reader.Read())
                            {
                                totalCount = reader.GetInt32(0);
                            }
                        }

                        var response = new JobListResponse
                        {
                            IsSuccess = true,
                            Message = "Job list retrieved successfully",
                            Jobs = jobList,
                            TotalCount = totalCount
                        };

                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<JobDetailsResponse> GetJobById(int jobId)
        {
            JobDetailsResponse jobDetails = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SPGetJobById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@JobId", jobId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            jobDetails = new JobDetailsResponse
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("JobId")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                Location = new Location
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("LocationId")),
                                    Name = reader.GetString(reader.GetOrdinal("LocationName")),
                                    Address = reader.GetString(reader.GetOrdinal("LocationAddress")),
                                },
                                Department = new Department
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                    DepartmentName = reader.GetString(reader.GetOrdinal("DepartmentName"))
                                },
                                ClosingDate = reader.GetDateTime(reader.GetOrdinal("ClosingDate"))
                            };
                        }
                        else
                        {
                            jobDetails = new JobDetailsResponse
                            {
                                Message = "No records found"
                            };
                        }
                    }
                }

                return jobDetails;
            }
        }

        public async Task<List<Department>> ListDepartments()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("ListDepartments", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        var departments = new List<Department>();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                var department = new Department
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                    DepartmentName = reader.GetString(reader.GetOrdinal("DepartmentName"))
                                };
                                departments.Add(department);
                            }
                        }

                        return departments;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<DepartmentResponse> InsertDepartment(DepartmentDetails departmentName)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("InsertDepartment", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@DepartmentName", departmentName.DepartmentName);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        return new DepartmentResponse
                        {
                            IsSuccess = true,
                            Message = $"{rowsAffected} department(s) inserted successfully."
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<DepartmentResponse> UpdateDepartment(DepartmentDetailsUpdate request)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("UpdateDepartment", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@DepartmentId", request.DeptId);
                        command.Parameters.AddWithValue("@DepartmentName", request.DepartmentName);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected == 0)
                        {
                            return new DepartmentResponse
                            {
                                IsSuccess = false,
                                Message = $"Department with ID {request.DeptId} not found."
                            };
                        }

                        return new DepartmentResponse
                        {
                            IsSuccess = true,
                            Message = $"department updated successfully."
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<Location>> ListLocations()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("ListLocations", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        var locations = new List<Location>();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                var location = new Location
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("LocationId")),
                                    Name = reader.GetString(reader.GetOrdinal("LocationName")),
                                    Address = reader.GetString(reader.GetOrdinal("LocationAddress"))
                                };
                                locations.Add(location);
                            }
                        }

                        return locations;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<LocationResponse> InsertLocation(LocationDetails locationDetails)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("InsertLocation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@LocationName", locationDetails.LocationName);
                        command.Parameters.AddWithValue("@LocationAddress", locationDetails.LcoationAddress);

                        var locationIdParam = new SqlParameter("@LocationId", SqlDbType.Int);
                        locationIdParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(locationIdParam);

                        int rowCount = await command.ExecuteNonQueryAsync();

                        var newLocationId = (int)locationIdParam.Value;
                        if (!string.IsNullOrEmpty(newLocationId.ToString()))
                        {
                            var response = new LocationResponse
                            {
                                LocationId = newLocationId.ToString(),
                                Message = "Successfully added location",
                                IsSuccess = true
                            };
                            return response;
                        }
                        else
                        {
                            var response = new LocationResponse
                            {
                                Message = "Failed to add location",
                                IsSuccess = false
                            };
                            return response;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw; 
            }
        }

        public async Task<LocationResponse> UpdateLocation(LocationDetailsUpdate request)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("UpdateLocation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@LocationId", SqlDbType.Int).Value = request.LocId;
                        command.Parameters.Add("@LocationName", SqlDbType.NVarChar, 50).Value = string.IsNullOrEmpty(request.LocationName) ? DBNull.Value : (object)request.LocationName;
                        command.Parameters.Add("@LocationAddress", SqlDbType.NVarChar, 255).Value = string.IsNullOrEmpty(request.LcoationAddress) ? DBNull.Value : (object)request.LcoationAddress;
                        var rowsAffectedParam = new SqlParameter("@RowsAffected", SqlDbType.Int);
                        rowsAffectedParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(rowsAffectedParam);

                        await command.ExecuteNonQueryAsync();

                        int rowsAffected = (int)rowsAffectedParam.Value;

                        if (rowsAffected > 0)
                        {
                            return new LocationResponse
                            {
                                IsSuccess = true,
                                Message = $"Location updated successfully."
                            };
                        }
                        else
                        {
                            return new LocationResponse
                            {
                                IsSuccess = false,
                                Message = "LocationId not found."
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw; 
            }
        }


    }
}
