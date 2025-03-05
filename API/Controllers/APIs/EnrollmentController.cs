using System.Net;
using Application.DTOs.Base;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Enrollment;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Application.DTOs.Authentication.Session;

namespace RSOS.Controllers.APIs;

[ApiController]
[Route("api/enrollments")]
[IgnoreAntiforgeryToken]
public class EnrollmentController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    [HttpGet("get-enrollment-status/{enrollmentId:int}")]
    public async Task<IActionResult> GetEnrollmentStatus(int enrollmentId)
    {
        var result = await _enrollmentService.GetEnrollmentStatus(enrollmentId);

        if (result.EnrollmentId == 0)
        {
            var invalidResponse = new ResponseDTO<EnrollmentResponseDTO>()
            {
                Status = "Not Found",
                Message = "Invalid Enrollment Identifier",
                StatusCode = HttpStatusCode.NotFound,
                Result = result
            };
            
            return NotFound(invalidResponse);
        }
        
        var response = new ResponseDTO<EnrollmentResponseDTO>()
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };
        
        return Ok(response);
    }

    [HttpPost("get-enrollment-status")]
    public async Task<IActionResult> PostEnrollmentStatus(int? enrollmentId)
    {
        if (!enrollmentId.HasValue)
        {
            var invalidResponse = new ResponseDTO<object>()
            {
                Status = "Not Found",
                Message = "Invalid Enrollment Identifier",
                StatusCode = HttpStatusCode.NotFound,
                Result = false
            };

            return NotFound(invalidResponse);
        }
        
        var result = await _enrollmentService.GetEnrollmentStatus((int)enrollmentId);
        
        if (result.EnrollmentId == 0)
        {
            var invalidResponse = new ResponseDTO<object>()
            {
                Status = "Not Found",
                Message = "Invalid Enrollment Identifier",
                StatusCode = HttpStatusCode.NotFound,
                Result = false
            };

            return NotFound(invalidResponse);
        }

        var response = new ResponseDTO<EnrollmentResponseDTO>()
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }

    [Authorize]
    [HttpPost("get-enrollment-status-authorize")]
    public async Task<IActionResult> PostEnrollmentStatusAuthorize([FromForm]int? enrollmentId)
    {
        if (!enrollmentId.HasValue)
        {
            var invalidResponse = new ResponseDTO<object>()
            {
                Status = "Not Found",
                Message = "Invalid Enrollment Identifier",
                StatusCode = HttpStatusCode.NotFound,
                Result = false
            };

            return NotFound(invalidResponse);
        }

        var result = await _enrollmentService.GetEnrollmentStatus((int)enrollmentId);

        if (result.EnrollmentId == 0)
        {
            var invalidResponse = new ResponseDTO<object>()
            {
                Status = "Not Found",
                Message = "Invalid Enrollment Identifier",
                StatusCode = HttpStatusCode.NotFound,
                Result = false
            };

            return NotFound(invalidResponse);
        }

        var response = new ResponseDTO<EnrollmentResponseDTO>()
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }

    [Authorize]
    [HttpPost("get-all-session")]
    public async Task<IActionResult> GetAllSession()
    {
        var result = await _enrollmentService.GetAllSession();

        var response = new ResponseDTO<List<SessionResponseDTO>>()
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }
}