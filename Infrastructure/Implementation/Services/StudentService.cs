using System.Globalization;
using System.Security.Claims;
using System.Text;
using Application.DTOs.Authentication;
using Application.DTOs.Authentication.Session;
using Application.DTOs.NewsAndAlert;
using Application.DTOs.Student;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Common.Utilities;
using Data.Implementation.Repositories;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Data.Implementation.Services;

public class StudentService : IStudentService
{
    private readonly RsosSettings _rsosSettings;
    private readonly IGenericRepository _genericRepository;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly HangfireSettings _hangfireSettings;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public StudentService(IGenericRepository genericRepository, IHttpContextAccessor contextAccessor, IOptions<RsosSettings> rsosSettings, IOptions<HangfireSettings> hangfireSettings, IWebHostEnvironment webHostEnvironment )
    {
        _genericRepository = genericRepository;
        _contextAccessor = contextAccessor;
        _rsosSettings = rsosSettings.Value;
        _hangfireSettings = hangfireSettings.Value;
        _webHostEnvironment = webHostEnvironment;
    }

    public int StudentId
    {
        get
        {
            var userIdClaimValue = _contextAccessor.HttpContext?.User.FindFirstValue("studentid");

            return int.TryParse(userIdClaimValue, out var userId) ? userId : 0;
        }
    }

    public string StudentDateOfBirth
    {
        get
        {
            var userDobClaimValue = _contextAccessor.HttpContext?.User.FindFirstValue("dob");

            return userDobClaimValue ?? "";
        }
    }

    public string StudentSSOID
    {
        get
        {
            var userSSOIDClaimValue = _contextAccessor.HttpContext?.User.FindFirstValue("ssoid");

            return userSSOIDClaimValue ?? "";
        }
    }

    public async Task<StudentResponseDTO> GetStudentRecords(int studentId)
    {
        var scores = await _genericRepository.GetAsync<tblStudentScore>(x =>
            x.StudentId == studentId && x.IsActive);

        var studentScores = scores.Select(x => new StudentScoreResponseDTO()
        {
            Id = x.Id,
            GUID = x.GUID,
            StudentId = x.StudentId,
            Class = x.Class,
            Score = x.Score,
            SubjectId = x.SubjectId,
            TopicId = x.TopicId,
            IsEdited = x.IsEdited ? 1 : 0,
            IsUploaded = x.IsUploaded ? 1 : 0,
        }).ToList();

        var responses = await _genericRepository.GetAsync<tblStudentResponse>(x => x.StudentId == studentId && x.IsActive);

        var studentResponses = responses.Select(x => new StudentResponsesResponseDTO()
        {
            Id = x.Id,
            GUID = x.GUID,
            StudentId = x.StudentId,
            QuestionId = x.QuestionId,
            IsUploaded = x.IsUploaded ? 1 : 0,
            IsEdited = x.IsEdited ? 1 : 0,
            QuestionValue = x.QuestionValue,
            QuizGUID = x.QuizGUID,
        }).ToList();

        return new StudentResponseDTO()
        {
            StudentResponses = studentResponses,
            StudentScores = studentScores
        };
    }

    public async Task<bool> InsertStudentResponse(List<StudentResponseRequestDTO> studentResponses)
    {
        bool IsSuccess = false;
        foreach (var studentResponse in studentResponses)
        {
            //var response = await _genericRepository.GetFirstOrDefaultAsync<tblStudentResponse>(x =>
            //x.GUID == studentResponse.GUID);
            var response = await _genericRepository.GetFirstOrDefaultAsync<tblStudentResponse>(x =>
                x.StudentId == studentResponse.StudentId && x.QuestionId == studentResponse.QuestionId);

            if (response == null)
            {
                var studentResponseModel = new tblStudentResponse
                {
                    GUID = studentResponse.GUID,
                    QuizGUID = studentResponse.QuizGUID,
                    StudentId = studentResponse.StudentId,
                    QuestionId = studentResponse.QuestionId,
                    QuestionValue = studentResponse.QuestionValue,
                    IsEdited = studentResponse.IsEdited == 1,
                    IsUploaded = studentResponse.IsUploaded == 1,
                    IsActive = true,
                    CreatedBy = studentResponse.StudentId,
                    CreatedOn = DateTime.Now,
                };

                await _genericRepository.InsertAsync(studentResponseModel);
                IsSuccess = true;
            }
            else
            {
                IsSuccess = false;
                //response.QuizGUID = studentResponse.QuizGUID;
                //response.StudentId = studentResponse.StudentId;
                //response.QuestionId = studentResponse.QuestionId;
                //response.QuestionValue = studentResponse.QuestionValue;
                //response.IsEdited = true;
                //response.IsUploaded = studentResponse.IsUploaded == 1;
                //response.LastUpdatedBy = studentResponse.StudentId;
                //response.LastUpdatedOn = DateTime.Now;

                //await _genericRepository.UpdateAsync(response);
            }
        }
        return IsSuccess;
    }

    public async Task<bool> InsertStudentScore(List<StudentScoreRequestDTO> studentScores)
    {
        bool IsSuccess = false;
        foreach (var studentScore in studentScores)
        {
            //var score = await _genericRepository.GetFirstOrDefaultAsync<tblStudentScore>(x =>
            //    x.GUID == studentScore.GUID);
            var score = await _genericRepository.GetFirstOrDefaultAsync<tblStudentScore>(x =>
                x.Class == studentScore.Class && x.SubjectId == studentScore.SubjectId && x.StudentId == studentScore.StudentId);

            if (score == null)
            {
                var studentScoreModel = new tblStudentScore()
                {
                    GUID = studentScore.GUID,
                    StudentId = studentScore.StudentId,
                    Class = studentScore.Class,
                    SubjectId = studentScore.SubjectId,
                    TopicId = studentScore.TopicId,
                    Score = studentScore.Score ?? "0",
                    IsEdited = studentScore.IsEdited == 1,
                    IsUploaded = false,
                    IsActive = true,
                    CreatedBy = studentScore.CreatedBy,
                    CreatedOn = DateTime.Now
                };

                await _genericRepository.InsertAsync(studentScoreModel);
                IsSuccess = true;
            }
            else
            {
                //score.StudentId = studentScore.StudentId;
                //score.Class = studentScore.Class;
                //score.SubjectId = studentScore.SubjectId;
                //score.TopicId = studentScore.TopicId;
                //score.Score = studentScore.Score ?? "0";
                //score.IsEdited = studentScore.IsEdited == 1;
                //score.IsUploaded = studentScore.IsUploaded == 1;
                //score.LastUpdatedBy = studentScore.StudentId;
                //score.LastUpdatedOn = DateTime.Now;

                //await _genericRepository.UpdateAsync(score);
                IsSuccess = false;
            }
        }
        return IsSuccess;
    }

    public async Task<StudentExamResponseDTO> GetStudentExamSubjects(string secureToken, string enrollment)
    {
        var pcpDate = (await _genericRepository.GetAsync<tblPCPDate>(x => x.IsActive)).MaxBy(x => x.Id);

        if (pcpDate == null)
        {
            return new StudentExamResponseDTO()
            {
                IsEligible = false,
                Message = "इस समय ePCP के लिए कोई भी तिथियां आवंटित नहीं की गई हैं।",
                PCPEndDate = "",
                PCPStartDate = "",
                SubjectsList = [],
            };
        }

        if (pcpDate.StartDate <= DateTime.Now && DateTime.Now <= pcpDate.EndDate)
        {
            var studentId = StudentId;

            var httpClient = new HttpClient();

            var rsosToken = _rsosSettings.Token;

            var rsosUrl = _rsosSettings.URL;

            var baseUrl = $"{rsosUrl}/new_api_student_exam_subjects";

            var queryParams = new System.Collections.Specialized.NameValueCollection
            {
                { "ssoid", StudentSSOID },
                { "dob", StudentDateOfBirth },
                { "token", rsosToken },
                { "enrollment", enrollment },
                { "secure_token", secureToken }
            };

            var uriBuilder = new UriBuilder(baseUrl)
            {
                Query = string.Join("&", Array.ConvertAll(queryParams.AllKeys,
                    key => $"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(queryParams[key])}"))
            };

            var postData = new StringContent("{\"key\": \"value\"}", Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(uriBuilder.Uri, postData);

            if (!response.IsSuccessStatusCode)
            {
                return new StudentExamResponseDTO()
                {
                    IsEligible = false,
                    Message = "Invalid User Credentials & Token.",
                    PCPEndDate = pcpDate.StartDate.ToString("dd-MM-yyyy hh:mm:ss tt"),
                    PCPStartDate = pcpDate.EndDate.ToString("dd-MM-yyyy hh:mm:ss tt"),
                    SubjectsList = [],
                };
            }

            var responseData = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonConvert.DeserializeObject<StudentExamDetailsResponseDTO>(responseData);

            if (apiResponse is not { Status: true })
            {
                return new StudentExamResponseDTO()
                {
                    IsEligible = false,
                    Message = "Invalid User Credentials & Token.",
                    PCPEndDate = pcpDate.StartDate.ToString("dd-MM-yyyy hh:mm:ss tt"),
                    PCPStartDate = pcpDate.EndDate.ToString("dd-MM-yyyy hh:mm:ss tt"),
                    SubjectsList = [],
                };
            }

            var studentSubjects = apiResponse.Data.Exam_Subjects;

            var subjects = await _genericRepository.GetAsync<tblSubject>(x =>
                studentSubjects.Select(y => y.Subject_Id).Contains(x.Id));

            var studentScores =
                await _genericRepository.GetAsync<tblStudentScore>(x =>
                    x.StudentId == studentId && x.TopicId == 0);

            var subjectIdsWithZeroScore = studentScores.Select(s => s.SubjectId).ToList();

            var result = subjects.Where(subject =>
                    !subjectIdsWithZeroScore.Contains(subject.Id))
                .Select(subject => new SubjectDetails
                {
                    Id = subject.Id,
                    Code = subject.SubjectCode ?? 0,
                    Class = subject.Class ?? 10,
                    Name = subject.Title
                }).ToList();

            return new StudentExamResponseDTO()
            {
                IsEligible = true,
                Message =
                    $"The ePCP dates align with the allocated period of {pcpDate.StartDate:dd-MM-yyyy hh:mm:ss tt} to {pcpDate.EndDate:dd-MM-yyyy hh:mm:ss tt}. Hence you are eligible for the examination at the moment.",
                PCPEndDate = pcpDate.StartDate.ToString("dd-MM-yyyy hh:mm:ss tt"),
                PCPStartDate = pcpDate.EndDate.ToString("dd-MM-yyyy hh:mm:ss tt"),
                SubjectsList = result,
            };

        }

        return new StudentExamResponseDTO()
        {
            IsEligible = false,
            Message = "अति महत्पूर्ण:E-Pcp पूर्व विषयवस्तु (E-content) व प्रश्नोत्तर का अध्यन करे। E-pcp की दिनाक (समय सारणी) शीघ्र ही आपको सूचित कर दी जाएगी।",
            PCPEndDate = pcpDate.StartDate.ToString("dd-MM-yyyy hh:mm:ss tt"),
            PCPStartDate = pcpDate.EndDate.ToString("dd-MM-yyyy hh:mm:ss tt"),
            SubjectsList = [],
        };
    }

    public async Task UploadStudentScoresOld()
    {
        try
        {
            var pcpStudentScores =
                 await _genericRepository.GetAsync<tblStudentScore>(x =>
                    x.TopicId == 0 && !x.IsUploaded && (x.Id > 15567 && x.Id < 25495));

            var studentScores = pcpStudentScores as tblStudentScore[] ?? pcpStudentScores.ToArray();

            var students =
                await _genericRepository.GetAsync<tblStudentLoginHistory>(x =>
                    studentScores.Select(z => z.StudentId).Contains(x.StudentId ?? 0));

            foreach (var student in students)
            {
                if (DateTime.Now.Hour >= _hangfireSettings.EndingHour && DateTime.Now.Hour <= _hangfireSettings.StartingHours) return;

                if (student.SSOID == null || student.DateOfBirth == null || student.StudentId == null) continue;

                var httpClient = new HttpClient();

                var rsosToken = _rsosSettings.Token;

                var rsosUrl = _rsosSettings.URL;

                var loginBaseUrl = $"{rsosUrl}/new_api_student_login";

                var loginQueryParams = new System.Collections.Specialized.NameValueCollection
                {
                    { "ssoid", student.SSOID },
                    { "dob", student.DateOfBirth },
                    { "token", rsosToken }
                };

                var loginUriBuilder = new UriBuilder(loginBaseUrl)
                {
                    Query = string.Join("&", Array.ConvertAll(loginQueryParams.AllKeys,
                        key => $"{Uri.EscapeDataString(key!)}={Uri.EscapeDataString(loginQueryParams[key]!)}"))
                };

                var loginPostData = new StringContent("{\"key\": \"value\"}", Encoding.UTF8, "application/json");

                var loginResponse = await httpClient.PostAsync(loginUriBuilder.Uri, loginPostData);

                if (loginResponse.IsSuccessStatusCode)
                {
                    var loginResponseData = await loginResponse.Content.ReadAsStringAsync();

                    var loginApiResponse = JsonConvert.DeserializeObject<RSOSLoginResponse>(loginResponseData);

                    if (loginApiResponse is { Status: true })
                    {
                        var studentResponseData = JsonConvert.DeserializeObject<LoginResponseDTO>(loginResponseData);

                        var studentLoginData = studentResponseData!.Data.Student;

                        var secureToken = studentResponseData.secure_token;

                        var pcpBaseUrl = $"{rsosUrl}/new_api_set_student_sessional_exam_subject_marks";

                        var scoreAttribute = studentScores.Where(x => x.StudentId == studentLoginData.Id);

                        foreach (var score in scoreAttribute)
                        {
                            if (double.TryParse(score.Score, out var result))
                            {
                                var subject = await _genericRepository.GetByIdAsync<tblSubject>(score.SubjectId);

                                var studentScore = result / 100.0 * (subject?.MaximumMarks ?? 0);

                                studentScore = Math.Round(studentScore * 2) / 2;

                                var pcpQueryParams = new System.Collections.Specialized.NameValueCollection
                                {
                                    { "token", rsosToken },
                                    { "secure_token", secureToken},
                                    { "enrollment", studentLoginData.Enrollment },
                                    { "ssoid", student.SSOID },
                                    { "subject_id", score.SubjectId.ToString() },
                                    { "obtained_marks", studentScore.ToString(CultureInfo.InvariantCulture) }
                                };

                                var pcpUriBuilder = new UriBuilder(pcpBaseUrl)
                                {
                                    Query = string.Join("&", Array.ConvertAll(pcpQueryParams.AllKeys,
                                        key => $"{Uri.EscapeDataString(key!)}={Uri.EscapeDataString(pcpQueryParams[key]!)}"))
                                };

                                var pcpPostData = new StringContent("{\"key\": \"value\"}", Encoding.UTF8, "application/json");

                                var pcpResponse = await httpClient.PostAsync(pcpUriBuilder.Uri, pcpPostData);

                                if (pcpResponse.IsSuccessStatusCode)
                                {
                                    var pcpResponseData = await pcpResponse.Content.ReadAsStringAsync();

                                    var pcpApiResponse = JsonConvert.DeserializeObject<StudentSessionalMarksDTO>(pcpResponseData);

                                    if (pcpApiResponse is { Status: true })
                                    {
                                        score.IsUploaded = true;
                                        score.LastUpdatedOn = DateTime.Now;

                                        var successLog =
                                            $"DOIT Success - Student SSOID: {student.SSOID}, Subject Id: {score.SubjectId.ToString()}, Score: {studentScore}";

                                        var exception = new tblExceptionLog()
                                        {
                                            DateTime = DateTime.Now,
                                            ErrorLog = successLog
                                        };

                                        await _genericRepository.InsertAsync(exception);

                                        await _genericRepository.UpdateAsync(score);
                                    }
                                    else
                                    {
                                        var errorLog =
                                            $"DOIT Exception - Student SSOID: {student.SSOID}, Subject Id: {score.SubjectId.ToString()}, Error: {pcpApiResponse?.Error.ToString()}";

                                        var exception = new tblExceptionLog()
                                        {
                                            DateTime = DateTime.Now,
                                            ErrorLog = errorLog
                                        };

                                        await _genericRepository.InsertAsync(exception);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            var exception = new tblExceptionLog()
            {
                DateTime = DateTime.Now,
                ErrorLog = $"Hangfire Service - {e.Message}"
            };

            await _genericRepository.InsertAsync(exception);
            await UploadStudentScores();
        }
    }

    public async Task UploadStudentScores()
    {
        try
        {
            var pcpStudentScores =
                await _genericRepository.GetAsync<tblStudentScore>(x =>
                    x.TopicId == 0 && !x.IsUploaded && (x.Id > 15567 && x.Id < 51968));

            var studentScores = pcpStudentScores as tblStudentScore[] ?? pcpStudentScores.ToArray();

            var students =
                await _genericRepository.GetAsync<tblStudentLoginHistory>(x =>
                    studentScores.Select(z => z.StudentId).Contains(x.StudentId ?? 0));

            foreach (var student in students)
            {
                //if (DateTime.Now.Hour >= _hangfireSettings.EndingHour && DateTime.Now.Hour <= _hangfireSettings.StartingHours) return;

                if (student.SSOID == null || student.DateOfBirth == null || student.StudentId == null) continue;

                var httpClient = new HttpClient();

                var rsosToken = _rsosSettings.Token;

                var rsosUrl = _rsosSettings.URL;

                var loginBaseUrl = $"{rsosUrl}/new_api_student_login";

                var loginQueryParams = new System.Collections.Specialized.NameValueCollection
                {
                    { "ssoid", student.SSOID },
                    { "dob", student.DateOfBirth },
                    { "token", rsosToken }
                };

                var loginUriBuilder = new UriBuilder(loginBaseUrl)
                {
                    Query = string.Join("&", Array.ConvertAll(loginQueryParams.AllKeys,
                        key => $"{Uri.EscapeDataString(key!)}={Uri.EscapeDataString(loginQueryParams[key]!)}"))
                };

                var loginPostData = new StringContent("{\"key\": \"value\"}", Encoding.UTF8, "application/json");

                var loginResponse = await httpClient.PostAsync(loginUriBuilder.Uri, loginPostData);

                if (loginResponse.IsSuccessStatusCode)
                {
                    var loginResponseData = await loginResponse.Content.ReadAsStringAsync();

                    var loginApiResponse = JsonConvert.DeserializeObject<RSOSLoginResponse>(loginResponseData);

                    if (loginApiResponse is { Status: true })
                    {
                        var studentResponseData = JsonConvert.DeserializeObject<LoginResponseDTO>(loginResponseData);

                        var studentLoginData = studentResponseData!.Data.Student;

                        var secureToken = studentResponseData.secure_token;

                        var pcpBaseUrl = $"{rsosUrl}/new_api_set_student_sessional_exam_subject_marks";

                        var scoreAttribute = studentScores.Where(x => x.StudentId == studentLoginData.Id);

                        foreach (var score in scoreAttribute)
                        {
                            if (double.TryParse(score.Score, out var result))
                            {
                                var subject = await _genericRepository.GetByIdAsync<tblSubject>(score.SubjectId);

                                var studentScore = result / 100.0 * (subject?.MaximumMarks ?? 0);

                                studentScore = Math.Round(studentScore * 2) / 2;

                                var pcpQueryParams = new System.Collections.Specialized.NameValueCollection
                                {
                                    { "token", rsosToken },
                                    { "secure_token", secureToken},
                                    { "enrollment", studentLoginData.Enrollment },
                                    { "ssoid", student.SSOID },
                                    { "subject_id", score.SubjectId.ToString() },
                                    { "obtained_marks", studentScore.ToString(CultureInfo.InvariantCulture) }
                                };

                                var pcpUriBuilder = new UriBuilder(pcpBaseUrl)
                                {
                                    Query = string.Join("&", Array.ConvertAll(pcpQueryParams.AllKeys,
                                        key => $"{Uri.EscapeDataString(key!)}={Uri.EscapeDataString(pcpQueryParams[key]!)}"))
                                };

                                var pcpPostData = new StringContent("{\"key\": \"value\"}", Encoding.UTF8, "application/json");

                                var pcpResponse = await httpClient.PostAsync(pcpUriBuilder.Uri, pcpPostData);

                                if (pcpResponse.IsSuccessStatusCode)
                                {
                                    var pcpResponseData = await pcpResponse.Content.ReadAsStringAsync();

                                    var pcpApiResponse = JsonConvert.DeserializeObject<StudentSessionalMarksDTO>(pcpResponseData);

                                    if (pcpApiResponse is { Status: true })
                                    {
                                        score.IsUploaded = true;
                                        score.LastUpdatedOn = DateTime.Now;

                                        await _genericRepository.UpdateAsync(score);

                                        var successLog =
                                            $"DOIT Success - Student SSOID: {student.SSOID}, Subject Id: {score.SubjectId.ToString()}, Score: {studentScore}";

                                        var exception = new tblExceptionLog()
                                        {
                                            DateTime = DateTime.Now,
                                            ErrorLog = successLog
                                        };

                                        await _genericRepository.InsertAsync(exception);
                                    }
                                    else
                                    {
                                        var errorLog =
                                            $"DOIT Exception - Student SSOID: {student.SSOID}, Subject Id: {score.SubjectId.ToString()}, Error: {pcpApiResponse?.Error.ToString()}";

                                        var exception = new tblExceptionLog()
                                        {
                                            DateTime = DateTime.Now,
                                            ErrorLog = errorLog
                                        };

                                        await _genericRepository.InsertAsync(exception);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            var exception = new tblExceptionLog()
            {
                DateTime = DateTime.Now,
                ErrorLog = $"Hangfire Service - {e.Message}"
            };

            await _genericRepository.InsertAsync(exception);
            await UploadStudentScores();
        }
    }

    public async Task<int> CheckIsScoreUploaded(int classId, int subjectId, int studentId)
    {
       

        IEnumerable<tblStudentScore> scores = await _genericRepository.GetAsync<tblStudentScore>(x =>x.Class == classId && x.SubjectId == subjectId && x.StudentId == studentId && x.IsActive);

        return scores.Count();
    }

    public async Task<IEnumerable<StudentDetailsResponseDTO>> GetStudentDetails()
    {
        var studentLoginDetails = await _genericRepository.GetAsync<tblStudentLoginDetail>(x => x.DeviceRegistrationToken != null && x.SSOID != null);
        var studentLoginHistories = await _genericRepository.GetAsync<tblStudentLoginHistory>(x => x.Enrollment != null && x.DateOfBirth != null && x.SSOID != null);

        var studentDetails = await _genericRepository.GetAsync<tblStudentDetails>(x => x.StudentId != null);
        var studentIdsInDetails = studentDetails.Select(x => x.StudentId).ToHashSet();

        var groupedStudentLoginDetails = studentLoginDetails
        .GroupBy(x => x.SSOID)
        .Select(g => new
        {
            SSOID = g.Key,
            Count = g.Count(),
        })
        .ToList();


        var result = from detail in groupedStudentLoginDetails
                     join history in studentLoginHistories
                     on detail.SSOID equals history.SSOID
                     where !studentIdsInDetails.Contains(history.StudentId)
                     select new StudentDetailsResponseDTO
                     {
                         StudentId = history.StudentId,
                         Enrollment = history.Enrollment,
                         SSOID = detail.SSOID,
                         DateOfBirth = history.DateOfBirth
                     };
        return result.Distinct().ToList();
    }

    public async Task<int> InsertStudentData(StudentDetailsRequestDTO studentDetailsRequest)
    {
        int result = 0;
        try
        {
            var studentDetailsModel = new tblStudentDetails()
            {
                SSOID = studentDetailsRequest.SSOID,
                DateOfBirth = studentDetailsRequest.DateOfBirth,
                Enrollment = studentDetailsRequest.Enrollment,
                StudentId = studentDetailsRequest.StudentId,
                ClassId = studentDetailsRequest.ClassId,
                SubjectId = studentDetailsRequest.SubjectId,
                MaximumMarks = studentDetailsRequest.MaximumMarks,
                CreatedBy = studentDetailsRequest.CreatedBy,
                CreatedOn = DateTime.Now,
            };

            result = await _genericRepository.InsertAsync(studentDetailsModel);
        }
        catch (Exception ex) {
            result = 0; 
        }
        return result;

    }

    public async Task<bool> GetStudenEnrollment()
    {
        var studentLoginDetails = await _genericRepository.GetAsync<tblStudentLoginDetail>(x => x.DeviceRegistrationToken != null && x.SSOID != null);
        var studentLoginHistories = await _genericRepository.GetAsync<tblStudentLoginHistory>(x => x.Enrollment != null && x.DateOfBirth != null && x.SSOID != null);

        
        var groupedStudentLoginDetails = studentLoginDetails
        .GroupBy(x => x.SSOID)
        .Select(g => new
        {
            SSOID = g.Key,
            Count = g.Count(),
        })
        .ToList();


        var result = from detail in groupedStudentLoginDetails
                     join history in studentLoginHistories
                     on detail.SSOID equals history.SSOID
                     select new StudentEnrollmentResponseNewDTO
                     {
                         Enrollment = history.Enrollment
                     };

        var resultList = result.Distinct().ToList();

        var jsonResult = JsonConvert.SerializeObject(resultList);

        string currentDateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
        string fileName = $"studentenrollment_{currentDateTime}.json";

        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "documents", "jsonFile", fileName);

        File.WriteAllText(filePath, jsonResult);
        return true;

        //return result.Distinct().ToList();
    }

}