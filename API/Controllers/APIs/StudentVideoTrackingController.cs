using System.Net;
using Application.DTOs.Base;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.Services;
using Application.DTOs.Tracking;
using Microsoft.AspNetCore.Authorization;
using Application.DTOs.Question;
using DocumentFormat.OpenXml.Office2019.Excel.RichData2;
using Application.DTOs.Dropdown;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace RSOS.Controllers.APIs;

[ApiController]
[Route("api/student-video-tracking")]
[IgnoreAntiforgeryToken]
public class StudentVideoTrackingController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly IStudentVideoTrackingService _studentVideoTrackingService;

    public StudentVideoTrackingController(IStudentVideoTrackingService studentVideoTracking, IStudentService studentService)
    {
        _studentVideoTrackingService = studentVideoTracking;
        _studentService = studentService;
    }

    [HttpPost("upsert-student-video-tracking")]
    public async Task<IActionResult> UpsertStudentVideoTracking(StudentVideoTrackingRequestDTO studentVideoTrackingRequest)
    {
        await _studentVideoTrackingService.UpsertStudentVideoTracking(studentVideoTrackingRequest);

        var result = new ResponseDTO<object>()
        {
            Status = "Success",
            Message = "Successfully Inserted / Updated.",
            StatusCode = HttpStatusCode.OK,
            Result = true
        };

        return Ok(result);
    }

    //[Authorize]
    //[HttpPost("upsert-student-video-tracking-authorize")]
    //public async Task<IActionResult> UpsertStudentVideoTrackingAuthorize([FromForm]StudentVideoTrackingRequestDTO studentVideoTrackingRequest)
    //{
    //    var studentIdentifier = _studentService.StudentId;

    //    if (studentVideoTrackingRequest.StudentId != studentIdentifier)
    //    {
    //        var unauthorized = new ResponseDTO<object>()
    //        {
    //            Status = "Unauthorized",
    //            Message = "The logged in student's identifier do not match with the provided student's identifier. ",
    //            StatusCode = HttpStatusCode.Unauthorized,
    //            Result = false
    //        };

    //        return Unauthorized(unauthorized);
    //    }

    //    await _studentVideoTrackingService.UpsertStudentVideoTracking(studentVideoTrackingRequest);

    //    var result = new ResponseDTO<object>()
    //    {
    //        Status = "Success",
    //        Message = "Successfully Inserted / Updated.",
    //        StatusCode = HttpStatusCode.OK,
    //        Result = true
    //    };

    //    return Ok(result);
    //}

    [Authorize]
    [HttpPost("upsert-student-video-tracking-authorize")]
    public IActionResult UpsertStudentVideoTrackingAuthorize([FromForm] StudentVideoTrackingRequestDTO studentVideoTrackingRequest)
    {
        var studentIdentifier = _studentService.StudentId;

        if (studentVideoTrackingRequest.StudentId != studentIdentifier)
        {
            var unauthorized = new ResponseDTO<object>()
            {
                Status = "Unauthorized",
                Message = "The logged in student's identifier do not match with the provided student's identifier. ",
                StatusCode = HttpStatusCode.Unauthorized,
                Result = false
            };

            return Unauthorized(unauthorized);
        }

        //await _studentVideoTrackingService.UpsertStudentVideoTracking(studentVideoTrackingRequest);

        var result = new ResponseDTO<object>()
        {
            Status = "Success",
            Message = "Successfully Inserted / Updated.",
            StatusCode = HttpStatusCode.OK,
            Result = true
        };

        return Ok(result);
    }

    [HttpGet("get-student-video-tracking/{studentId:int}")]
    public async Task<IActionResult> GetStudentVideoTrackingById(int studentId)
    {
        var result = await _studentVideoTrackingService.GetStudentVideoTrackingByStudentId(studentId);

        var response = new ResponseDTO<List<StudentVideoTrackingResponseDTO>>
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }

    [HttpPost("get-student-video-tracking")]
    public async Task<IActionResult> PostGetStudentVideoTrackingById(int studentId)
    {
        var result = await _studentVideoTrackingService.GetStudentVideoTrackingByStudentId(studentId);

        var response = new ResponseDTO<List<StudentVideoTrackingResponseDTO>>
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }
    
    //[Authorize]
    //[HttpPost("get-student-video-tracking-authorize")]
    //public async Task<IActionResult> PostGetStudentVideoTrackingByIdAuthorize([FromForm]int studentId)
    //{
    //    var studentIdentifier = _studentService.StudentId;

    //    if (studentId != studentIdentifier)
    //    {
    //        var unauthorized = new ResponseDTO<object>()
    //        {
    //            Status = "Unauthorized",
    //            Message = "The logged in student's identifier do not match with the provided student's identifier. ",
    //            StatusCode = HttpStatusCode.Unauthorized,
    //            Result = false
    //        };

    //        return Unauthorized(unauthorized);
    //    }
        
    //    var result = await _studentVideoTrackingService.GetStudentVideoTrackingByStudentId(studentId);

    //    var response = new ResponseDTO<List<StudentVideoTrackingResponseDTO>>
    //    {
    //        Status = "Success",
    //        Message = "Successfully Retrieved",
    //        StatusCode = HttpStatusCode.OK,
    //        Result = result
    //    };

    //    return Ok(response);
    //}

    [Authorize]
    [HttpPost("get-student-video-tracking-authorize")]
    public IActionResult PostGetStudentVideoTrackingByIdAuthorize([FromForm] int studentId)
    {
        var studentIdentifier = _studentService.StudentId;

        if (studentId != studentIdentifier)
        {
            var unauthorized = new ResponseDTO<object>()
            {
                Status = "Unauthorized",
                Message = "The logged-in student's identifier does not match the provided student's identifier.",
                StatusCode = HttpStatusCode.Unauthorized,
                Result = false
            };

            return Unauthorized(unauthorized);
        }

        var result = new List<StudentVideoTrackingResponseDTO>();

        //result.Select(x => new StudentVideoTrackingResponseDTO()
        //{
        //    Id = x.Id,
        //    SubjectId = x.SubjectId,
        //    VideoId = x.VideoId,
        //    StudentId = x.StudentId,
        //    Class = x.Class,
        //    IsCompleted = 1,
        //    PlayTimeInSeconds = x.PlayTimeInSeconds,
        //    PercentageCompleted = x.PercentageCompleted,
        //    VideoDurationInSeconds = x.VideoDurationInSeconds
        //}).ToList();

        var response = new ResponseDTO<List<StudentVideoTrackingResponseDTO>>
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }

}

