using System.Net;
using Application.DTOs.Authentication;
using Application.DTOs.Base;
using Application.DTOs.Content;
using Application.DTOs.Student;
using Application.Interfaces.Services;
using Azure;
using Common.Utilities;
using Data.Implementation.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using static Google.Apis.Requests.BatchRequest;

namespace RSOS.Controllers.APIs;

[ApiController]
[Route("api/studentsDetails")]
[IgnoreAntiforgeryToken]
public class StudentDetailsController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly RsosSettings _rsosSettings;
    private readonly IAuthenticationService _authenticationService;

    public StudentDetailsController(IStudentService studentService, IOptions<RsosSettings> rsosSettings, IAuthenticationService authenticationService)
    {
        _studentService = studentService;
        _rsosSettings = rsosSettings.Value;
        _authenticationService = authenticationService;
    }
  

    //[Authorize]
    [HttpPost("Get-Student-Details")]
    public async Task<IActionResult> GetStudentDetails()
    {
        try
        {
            int result = 0;

            var studentDetails = await _studentService.GetStudentDetails();

            foreach (var student in studentDetails)
            {
                if (student.SSOID == null && student.DateOfBirth == null && student.Enrollment == null) continue;

                AuthenticationRequestDTO authenticationRequest = new AuthenticationRequestDTO();

                authenticationRequest.SSOID = student.SSOID == null ? "" : student.SSOID;
                authenticationRequest.DateOfBirth = student.DateOfBirth == null ? "" : student.DateOfBirth;
                authenticationRequest.enrollment = student.Enrollment == null ? "" : student.Enrollment;
                authenticationRequest.st_keys = student.StudentId.ToString() == null ? "" : student.StudentId.ToString(); ;

                var response = await _authenticationService.AuthenticateForStudent(authenticationRequest);

                if (response.Id > 0)
                {
                    //var dataClassSubject = await CallSubjectAPI(authenticationRequest.SSOID.ToUpper(), authenticationRequest.DateOfBirth, response.SecureRSOSToken, authenticationRequest.enrollment);
                    var dataClassSubject = await CallSubjectAPI(authenticationRequest.SSOID.ToUpper(), authenticationRequest.DateOfBirth, response.SecureRSOSToken, authenticationRequest.st_keys == null ? "" : authenticationRequest.st_keys);

                    if (dataClassSubject != null ) 
                    {
                        try
                        {
                            var subjectResponse = JsonConvert.DeserializeObject<StudentExamDetailsResponseDTO>(dataClassSubject);

                            if (subjectResponse != null && subjectResponse.Status == true)
                            {
                                foreach (var subject in subjectResponse.Data.Exam_Subjects)
                                {
                                    StudentDetailsRequestDTO studentDetailsRequest = new StudentDetailsRequestDTO();
                                    studentDetailsRequest.SSOID = authenticationRequest.SSOID;
                                    studentDetailsRequest.DateOfBirth = subjectResponse?.Data.Student.Dob == null ? "" : authenticationRequest.DateOfBirth;
                                    studentDetailsRequest.Enrollment = subjectResponse?.Data.Student.Enrollment;
                                    studentDetailsRequest.StudentId = subjectResponse?.Data.Student.Id;
                                    studentDetailsRequest.ClassId = subjectResponse?.Data.Student.Course;
                                    studentDetailsRequest.SubjectId = subject.Subject_Id;
                                    studentDetailsRequest.MaximumMarks = 0;
                                    studentDetailsRequest.CreatedBy = 1;
                                    result = await _studentService.InsertStudentData(studentDetailsRequest);
                                }
                            }
                        }
                        catch(Exception ex)
                        {

                        }
                    }
                    
                } 
            }
            var responce = new ResponseDTO<object>()
            {
                Status = "Success",
                Message = "Data Saved",
                StatusCode = HttpStatusCode.OK,
                Result = true
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


    public async Task<string> CallSubjectAPI(string ssoid, string dob, string secureToken, string st_key)
    {
        //var url = "https://rsosadmission.rajasthan.gov.in/rsos/api/new_api_student_exam_subjects";
        //var url = "https://rsosadmission.rajasthan.gov.in/rsos/api/update_new_api_student_exam_subjects";

        var rsosUrl = _rsosSettings.URL;
        var url = $"{rsosUrl}/update_new_api_student_exam_subjects";

        // Create a new HttpClient instance
        using (HttpClient client = new HttpClient())
        {
            // Create a collection of key-value pairs for the form data
            var values = new Dictionary<string, string>
                {
                    { "ssoid", ssoid },
                    { "dob", dob },
                    { "token", "IUAjJCVeJiooKWRvaXRj" },
                    //{ "enrollment", enrollment },
                    { "st_key", st_key },
                    { "secure_token", secureToken },
                };

            // Convert the form data to an instance of FormUrlEncodedContent
            var content = new FormUrlEncodedContent(values);

            // Send the POST request
            HttpResponseMessage response = await client.PostAsync(url, content);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Read and parse the response content
                string responseData = await response.Content.ReadAsStringAsync();
                return responseData;
            }
            else
            {
                return "0";
            }
        }
    }

    //[Authorize]
    [HttpPost("Get-Student-Enrollment")]
    public async Task<IActionResult> GetDataJsonFile(int startId, int endId)
    {
        try
        {
            bool IsFile = await _studentService.GetStudenEnrollment();
            if (IsFile)
            {
                var response = new ResponseDTO<List<ContentResponseDTO>>()
                {
                    Status = "Success",
                    Message = "Json File Create Successfully",
                    StatusCode = HttpStatusCode.OK,
                };
                return Ok(response);
            }
            else
            {
                var response = new ResponseDTO<List<ContentResponseDTO>>()
                {
                    Status = "Failed",
                    Message = "Json File Create Failed",
                    StatusCode = HttpStatusCode.OK,
                };
                return Ok(response);
            }

        }

        catch (Exception ex)
        {
            var response = new ResponseDTO<List<ContentResponseDTO>>()
            {
                Status = "Failed",
                Message = ex.Message,
                StatusCode = HttpStatusCode.OK,
            };

            return Ok(response);
        }

    }


}