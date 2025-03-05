using System.Net;
using System.Text;
using System.Text.Json;
using Application.DTOs.Authentication;
using Application.DTOs.Base;
using Application.DTOs.Student;
using Application.Interfaces.Services;
using Azure;
using Common.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
//using Newtonsoft.Json;
using static Google.Apis.Requests.BatchRequest;

namespace RSOS.Controllers.APIs;

[ApiController]
[Route("api/students")]
[IgnoreAntiforgeryToken]
public class StudentController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly RsosSettings _rsosSettings;
    private readonly IAuthenticationService _authenticationService;

    public StudentController(IStudentService studentService, IOptions<RsosSettings> rsosSettings, IAuthenticationService authenticationService)
    {
        _studentService = studentService;
        _rsosSettings = rsosSettings.Value;
        _authenticationService = authenticationService;
    }
    
    [HttpGet("get-student-responses/{studentId:int}")]
    public async Task<IActionResult> GetStudentResponses(int studentId)
    {
        var result = await _studentService.GetStudentRecords(studentId);
        
        var response = new ResponseDTO<StudentResponseDTO>
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };
        
        return Ok(response);
    }
    
    [HttpPost("get-student-responses")]
    public async Task<IActionResult> GetStudentResponsesResult(int studentId)
    {
        var result = await _studentService.GetStudentRecords(studentId);
        
        var response = new ResponseDTO<StudentResponseDTO>
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };
        
        return Ok(response);
    }

    //[Authorize]
    //[HttpPost("get-student-responses-authorize")]
    //public async Task<IActionResult> GetStudentResponsesResultAuthorize([FromForm]int studentId)
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

    //    var result = await _studentService.GetStudentRecords(studentId);

    //    var response = new ResponseDTO<StudentResponseDTO>
    //    {
    //        Status = "Success",
    //        Message = "Successfully Retrieved",
    //        StatusCode = HttpStatusCode.OK,
    //        Result = result
    //    };

    //    return Ok(response);
    //}

    //[Authorize]
    [HttpPost("get-student-responses-authorize")]
    public IActionResult GetStudentResponsesResultAuthorize([FromForm] int studentId)
    {
        var studentIdentifier = _studentService.StudentId;

        if (studentId != studentIdentifier)
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

        //var result = await _studentService.GetStudentRecords(studentId);
        var result = new StudentResponseDTO();

        var response = new ResponseDTO<StudentResponseDTO>
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }

    [HttpPost("insert-student-records")]
    public async Task<IActionResult> InsertStudentResponse(StudentTransactionRequestDTO studentResponse)
    {
        await _studentService.InsertStudentResponse(studentResponse.StudentResponse);

        await _studentService.InsertStudentScore(studentResponse.StudentScores);

        var result = new ResponseDTO<object>()
        {
            Status = "Success",
            Message = "Successfully Inserted / Updated.",
            StatusCode = HttpStatusCode.OK,
            Result = true
        };

        return Ok(result);
    }

    [Authorize]
    [HttpPost("insert-student-records-authorize")]
    public async Task<IActionResult> InsertStudentResponseAuthorize([FromForm]string? studentResponse, [FromForm]string? studentScore)
    {
        var studentIdentifier = _studentService.StudentId;

        var unauthorizedResponse = new ResponseDTO<object>()
        {
            Status = "Unauthorized",
            Message = "The logged in student's identifier do not match with the provided student's identifier. ",
            StatusCode = HttpStatusCode.Unauthorized,
            Result = false
        };

        if (studentResponse != null)
        {
            var studentResponses = JsonSerializer.Deserialize<List<StudentResponseRequestDTO>>(studentResponse);
            
            if(studentResponses != null && studentResponses.Any(x => x.StudentId != studentIdentifier))
            {
                return Unauthorized(unauthorizedResponse);
            }
        }

        if (studentScore != null)
        {
            var studentScores = JsonSerializer.Deserialize<List<StudentScoreRequestDTO>>(studentScore);
            
            if (studentScores != null && studentScores.Any(x => x.StudentId != studentIdentifier))
            {
                return Unauthorized(unauthorizedResponse);
            }
        }

        if (studentResponse != null && studentScore != null)
        {
            var studentResponses = JsonSerializer.Deserialize<List<StudentResponseRequestDTO>>(studentResponse);
            var studentScores = JsonSerializer.Deserialize<List<StudentScoreRequestDTO>>(studentScore);

            //await _studentService.InsertStudentResponse(studentResponses!);
            //await _studentService.InsertStudentScore(studentScores!);

            bool IsSuccessResponce = false;
            bool IsSuccessScores = false;
            foreach (var score in studentScores!)
            {
                if (score.TopicId == 0)
                {
                    IsSuccessResponce = await _studentService.InsertStudentResponse(studentResponses!);
                    IsSuccessScores = await _studentService.InsertStudentScore(studentScores!);
                }
            }
       

            //if (DateTime.Now.Hour <= 7 && DateTime.Now.Hour >= 22)
            //{
            //    await _studentService.UploadStudentScores();
            //}

            if(IsSuccessResponce && IsSuccessScores)
            {
                var successResponse = new ResponseDTO<object>()
                {
                    Status = "Success",
                    Message = "Successfully Inserted / Updated.",
                    StatusCode = HttpStatusCode.OK,
                    Result = true
                };

                return Ok(successResponse);
            }
            else
            {
                var successResponse = new ResponseDTO<object>()
                {
                    Status = "Success",
                    Message = "Successfully Inserted / Updated.",
                    StatusCode = HttpStatusCode.OK,
                    Result = true
                };

                return Ok(successResponse);
            }

            
        }

        var errorResponse = new ResponseDTO<object>()
        {
            Status = "Error",
            Message = "Invalid data provided.",
            StatusCode = HttpStatusCode.BadRequest,
            Result = false
        };

        return BadRequest(errorResponse);
    }
    
    [Authorize]
    [HttpPost("get-student-exam-details-authorize")]
    public async Task<IActionResult> GetStudentExamDetailsAuthorize([FromForm]string secureToken, [FromForm] string enrollment)
    {
        var details = await _studentService.GetStudentExamSubjects(secureToken,enrollment);

        var result = new ResponseDTO<StudentExamResponseDTO>()
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = details
        };

        return Ok(result);
    }

    [HttpPost("upload-student-scores")]
    public async Task<IActionResult> UploadStudentScores()
    {
        await _studentService.UploadStudentScores();

        var result = new ResponseDTO<object>()
        {
            Status = "Success",
            Message = "Successfully Uploaded",
            StatusCode = HttpStatusCode.OK,
            Result = true
        };

        return Ok(result);
    }

    //[Authorize]
    [HttpPost("check-IsScore-Uploaded")]
    public async Task<IActionResult> CheckIsScoreUploaded(int classId, int subjectId, int studentId)
    {
        try
        {
            await _studentService.UploadStudentScores();
            int result = await _studentService.CheckIsScoreUploaded(classId, subjectId, studentId);

            var responce = new ResponseDTO<object>()
            {
                Status = "Success",
                Message = result == 0 ? "No Record Found" : "Record Found",
                StatusCode = HttpStatusCode.OK,
                Result = result == 0 ? false : true
            };
            return Ok(responce);
        }
        catch (Exception ex)
        {
            var responce = new ResponseDTO<object>()
            {
                Status = "Failed",
                Message = ex.Message,
                StatusCode = HttpStatusCode.BadRequest,
                Result = false
            };
            return Ok(responce);
        }
      
    }


}