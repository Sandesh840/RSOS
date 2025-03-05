using Application.DTOs.Authentication;
using Application.DTOs.Base;
using Application.DTOs.Content;
using Application.DTOs.User;
using Application.Interfaces.Services;
using Azure;
using Common.Utilities;
using Data;
using Data.Implementation.Services;
using Data.Persistence;
using Edi.Captcha;
using Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Data;
using Common.Constants;
using static Google.Apis.Requests.BatchRequest;
using Microsoft.Extensions.Options;

namespace RSOS.Controllers
{
    //[Authentication]
    public class DataCaptureController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMemoryCache _memoryCache;
        private readonly ICachePolicy _cachePolicy;
        private readonly ApplicationDbContext _context;
        private readonly ISessionBasedCaptcha _captcha;
        private readonly RsosSettings _rsosSettings;

        public DataCaptureController(IAuthenticationService authenticationService, ApplicationDbContext context, ISessionBasedCaptcha captcha, IWebHostEnvironment webHostEnvironment,IMemoryCache memoryCache,ICachePolicy cachePolicy,
        IOptions<RsosSettings> rsosSettings)
        {
            _captcha = captcha;
            _authenticationService = authenticationService;
            _memoryCache = memoryCache;
            _cachePolicy = cachePolicy;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
            _rsosSettings = rsosSettings.Value;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("StudentId") != null)
            {
                return RedirectToAction("Index", "DataCapture");
            }
            if (HttpContext.Session.GetInt32("UserName") != null)
            {
                UserRequestDTO userRequestDTO = new UserRequestDTO();
                userRequestDTO.ExamUserName = HttpContext.Session.GetInt32("UserName").ToString();
               
            }

            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {

            var userId = HttpContext.Session.GetInt32("UserId");
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("StudentId");
            HttpContext.Session.Remove("dataClassSubject");
            TempData["Success"] = "Successfully logged out.";
            return RedirectToAction("Login","Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> LoginNew(UserRequestDTO userRequest)
        {

            if (string.IsNullOrEmpty(userRequest.ExamUserName) || string.IsNullOrEmpty(userRequest.ExamPassword))
            {
                TempData["Warning"] = "Please insert your username and password before submitting your request.";

                return RedirectToAction("Login","Home");
            }
            AuthenticationRequestDTO authenticationRequest = new AuthenticationRequestDTO();
            authenticationRequest.SSOID = userRequest.ExamUserName.ToUpper();
            authenticationRequest.DateOfBirth = userRequest.ExamPassword;
            authenticationRequest.VersionId = "Web";
            authenticationRequest.enrollment = userRequest.Enrollment;

            //var isPasswordValid = await _userService.IsUserAuthenticated(userRequest);

            var cacheKey = CacheName.Login.ToString() + authenticationRequest.SSOID.ToUpper().ToString() + authenticationRequest.DateOfBirth.ToString() + authenticationRequest.enrollment?.ToString() + authenticationRequest.VersionId.ToString();
            var lstCacheData = new AuthenticationResponseDTO();
            //bool isExist = _memoryCache.TryGetValue(cacheKey, out lstCacheData);
            bool isExist = false;
            if (!isExist) {
                var response = await _authenticationService.Authenticate(authenticationRequest);

                if (response.Id == 0)
                {
                    TempData["Warning"] = "Invalid username or password.";
                    //return RedirectToAction("Login", "Home");
                    var responseJson = new
                    {
                        isEnroll = false,
                        IsSuccess = true,
                    };
                    return Json(responseJson);
                }
                if (response.Id == -2)
                {
                    var responseJson = new
                    {
                        enrollement = response.EnrollmentList.Data,
                        isEnroll = true,
                        IsSuccess = true,
                    };
                    return Json(responseJson);
                    //return RedirectToAction("Login", "Home");
                }


                HttpContext.Session.SetInt32("StudentId", response.Id);

                var CacheUtility = new CacheUtility(_memoryCache);
                CacheUtility.RemoveCacheByKey(cacheKey);

                //var cachePolicy = _cachePolicy.CacheExpiryOptions();
                //_memoryCache.Set(cacheKey, response, cachePolicy[0]);

                TempData["Success"] = "Successfully authenticated.";
                //var dataClassSubject = await CallSubjectAPI(response.SSOID.ToUpper(), response.DateOfBirth, response.SecureRSOSToken);
                var dataClassSubject = await CallSubjectAPI(response.SSOID.ToUpper(), response.DateOfBirth, response.SecureRSOSToken,response.Enrollment == null ? "" : response.Enrollment);
                if (dataClassSubject != "0")
                {
                    HttpContext.Session.SetString("dataClassSubject", dataClassSubject);
                    //return RedirectToAction("Index");
                    var responseJson = new
                    {
                        isEnroll = false,
                        IsSuccess = true,
                    };
                    return Json(responseJson);
                }
                TempData["Warning"] = "Subject and class getting fail.";
            }
            else
            {
                HttpContext.Session.SetInt32("StudentId", Convert.ToString(lstCacheData.Id) == "" ? 0 : lstCacheData.Id);
                TempData["Success"] = "Successfully authenticated.";
                //var dataClassSubject = await CallSubjectAPI(lstCacheData.SSOID.ToUpper(), lstCacheData.DateOfBirth, lstCacheData.SecureRSOSToken);
                var dataClassSubject = await CallSubjectAPI(lstCacheData.SSOID.ToUpper(), lstCacheData.DateOfBirth, lstCacheData.SecureRSOSToken, lstCacheData.Enrollment == null ? "" : lstCacheData.Enrollment);
                if (dataClassSubject != "0")
                {
                    HttpContext.Session.SetString("dataClassSubject", dataClassSubject);
                    //return RedirectToAction("Index");
                    var responseJson = new
                    {
                        isEnroll = false,
                        IsSuccess = true,
                    };
                    return Json(responseJson);
                }
                TempData["Warning"] = "Subject and class getting fail.";
            }
            

            return RedirectToAction("Login","Home");
        }

        public async Task<string> CallSubjectAPI(string ssoid,string dob, string secureToken,string enrollment)
        {
            var rsosUrl = _rsosSettings.URL;
            var url = $"{rsosUrl}/new_api_student_exam_subjects";
            //var url = "https://rsosadmission.rajasthan.gov.in/rsos/api/new_api_student_exam_subjects";

            // Create a new HttpClient instance
            using (HttpClient client = new HttpClient())
            {
                // Create a collection of key-value pairs for the form data
                var values = new Dictionary<string, string>
                {
                    { "ssoid", ssoid },
                    { "dob", dob },
                    { "token", "IUAjJCVeJiooKWRvaXRj" },
                    { "enrollment", enrollment },
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

        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("StudentId") != null && HttpContext.Session.GetInt32("dataClassSubject") != null)
            {
                //ViewBag.ssoid = HttpContext.Session.GetString("ssoid");
                //ViewBag.dob = HttpContext.Session.GetString("dob");
                //ViewBag.secure_token = HttpContext.Session.GetString("secure_token");
                ViewBag.dataClassSubject = HttpContext.Session.GetString("dataClassSubject");

                ViewBag.FormEvalGuid = GetGuid();
                ViewBag.SurveyStartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                return View();
            }
            else
            {
                return RedirectToAction("Login","Home");
            }

        }

        public static string GetGuid()
        {
            return Guid.NewGuid().ToString().Replace("-", "").ToUpper() + DateTime.Now.ToString("yyyyMMddHHss");
        }

        [HttpGet]
        public async Task<IActionResult> GetSubjectList(int classId,string subjectId)
        {
            var listSubject = JsonConvert.DeserializeObject<List<int>>(subjectId);
            var studentId = HttpContext.Session.GetInt32("StudentId");
            if (studentId != null)
            {
                //var lstSubjects = await _context.tblSubjects.Where(p => (p.Class == classId) && listSubject.Contains(p.Id)).ToListAsync();

                var scoreSubject = subjectId.Replace("]", "").Replace("[", "");
                var query = "select s.Id, s.SubjectCode,s.TitleInHindi as Subject,s.MaximumMarks,convert(varchar,ss.CreatedOn,100) as CreatedOn from tblSubjects s left join tblStudentScore ss on s.Id = ss.SubjectId and ss.StudentId =  " + studentId+"  where s.Id IN ("+scoreSubject+")";
                var dt = DynamicQuerySqlServer(_context.Database.GetConnectionString(), query);

                return PartialView("_RenderSubjects", dt);
            }
            else
            {
                return RedirectToAction("Login","Home");
            }
        }

        public async Task<IActionResult> GetQuestionList(int classId,int subjectId)
        {
            if (HttpContext.Session.GetInt32("StudentId") != null)
            {
                var lstQuestions = await _context.tblQuestions.Where(p => p.Class == classId && p.SubjectId == subjectId && p.TopicId == 0).ToListAsync();
                var lstOptions = await _context.tblCommons.Where(p => lstQuestions.Select(p => p.Flag).Contains(p.Flag)).ToListAsync();
                var collection = new SurveyCollection();
                lstQuestions.ForEach(p => p.PaperType = 1);
                collection.TblQuestions = lstQuestions;
                collection.TblOptions = lstOptions;
                return PartialView("_RenderQuestions", collection);
            }
            else
            {
                return RedirectToAction("Login","Home");
            }

        }

        [HttpPost]
        public async Task<IActionResult> SaveResponse(IFormCollection formData){
            //ExamData examData) { 
            //int ClassId,int SubjectId,string Score,string StartTime,string evaluationValue){
            var studentId = (int)HttpContext.Session.GetInt32("StudentId");

            if (studentId != null)
            {
                try
                {
                    var ClassId = Convert.ToInt32(formData["ClassId"]);
                    var SubjectId = Convert.ToInt32(formData["SubjectId"]);
                    var Score = formData["Score"].ToString();
                    var StartTime = Convert.ToDateTime(formData["StartTime"]);
                    var evaluationValue = formData["evaluationValue"].ToString();

                    //var ClassId = examData.ClassId;
                    //var SubjectId = examData.SubjectId;
                    //var Score = examData.Score;
                    //var StartTime = examData.StartTime;
                    //var evaluationValue = examData.evaluationValue;

                    //var studentId = 0;  // change student id
                    var studentResponses = JsonConvert.DeserializeObject<List<tblStudentResponse>>(evaluationValue);
                    var studentScore = new tblStudentScore();
                    studentScore.StudentId = studentId;
                    studentScore.Class = ClassId;
                    studentScore.SubjectId = SubjectId;
                    studentScore.TopicId = 0;
                    studentScore.Score = Score;
                    studentScore.IsEdited = true;
                    studentScore.IsUploaded = true;
                    studentScore.IsActive = true;
                    studentScore.CreatedBy = studentId;
                    studentScore.CreatedOn = Convert.ToDateTime(StartTime);
                    studentScore.LastUpdatedOn = DateTime.Now;
                    studentScore.GUID = studentResponses.Select(p => p.QuizGUID).FirstOrDefault();

                    if (_context.tblStudentScores.Where(p => p.StudentId == studentScore.StudentId && p.SubjectId == studentScore.SubjectId).Count() == 0)
                    {
                        await _context.tblStudentScores.AddAsync(studentScore);
                        studentResponses.ForEach(p => p.IsActive = true);
                        studentResponses.ForEach(p => p.IsEdited = true);
                        studentResponses.ForEach(p => p.IsUploaded = true);
                        studentResponses.ForEach(p => p.StudentId = studentId);
                        studentResponses.ForEach(p => p.CreatedBy = studentId);
                        studentResponses.ForEach(p => p.CreatedOn = DateTime.Now);
                        await _context.tblStudentResponses.AddRangeAsync(studentResponses);
                        await _context.SaveChangesAsync();
                        HttpContext.Session.SetInt32("ExamStatus", 1);
                        return RedirectToAction("AfterExam");
                    }
                    else
                    {
                        HttpContext.Session.SetInt32("ExamStatus", 2);
                        return RedirectToAction("AfterExam");
                    }
                }
                catch (Exception ex)
                {
                    return Json("Index");
                }
            }
            else
            {
                return RedirectToAction("Login","Home");
            }

        }

        public IActionResult AfterExam(int id)
        {
            ViewBag.ExamStatus = HttpContext.Session.GetInt32("ExamStatus");

            //HttpContext.Session.Clear();
            //HttpContext.Session.Remove("StudentId");
            //HttpContext.Session.Remove("dataClassSubject");
            //HttpContext.Session.Remove("ExamStatus");

            return View();
        }

        public DataTable DynamicQuerySqlServer(string connectionStrings, string query)
        {
            try
            {
                using (var _connection = new SqlConnection(connectionStrings))
                {
                    _connection.Open();
                    using (SqlCommand command = new SqlCommand(query, _connection))
                    {
                        var adapter = new SqlDataAdapter(command);
                        adapter.SelectCommand.CommandTimeout = 0;
                        DataTable table = new DataTable();
                        adapter.Fill(table);
                        return table;
                    }
                }
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }

        public IActionResult AdmitCard()
        {
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "documents", "apk", "rsos.apk");

            if (!System.IO.File.Exists(filePath)) return NotFound();

            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            const string fileName = "RSOS_App.apk";

            return File(fileBytes, "application/vnd.android.package-archive", fileName);
        }
    }

    public class SurveyCollection
    {
        public List<Data.tblQuestion>? TblQuestions { get; set; }
        public List<Data.tblCommon>? TblOptions { get; set; }
    }
    public class ExamData
    {
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public string? Score { get; set; }
        public string? StartTime { get; set; }
        public string? evaluationValue { get; set; }
    }
}
