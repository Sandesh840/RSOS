using System.Globalization;
using Hangfire;
using System.Text;
using Newtonsoft.Json;
using Common.Utilities;
using Application.DTOs.Authentication;
using Application.DTOs.Student;
using Application.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using DocumentFormat.OpenXml.InkML;
using Application.DTOs.Dropdown;
using Application.DTOs.PCP;
using Irony.Parsing;
using Newtonsoft.Json.Linq;
using Azure;
using Microsoft.AspNetCore.Hosting;
using Data.Implementation.Repositories;
using Application.Interfaces.Services;

namespace Data.Implementation.Services;

public class BulkUploadService : IBulkUploadService
{
    private readonly RsosSettings _rsosSettings;
    private readonly IGenericRepository _genericRepository;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public BulkUploadService(IOptions<RsosSettings> rsosSettings,IGenericRepository genericRepository, IWebHostEnvironment webHostEnvironment)
    {
        _rsosSettings = rsosSettings.Value;
        _genericRepository = genericRepository;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<bool> GetJsonFile(int startId, int endId)
    {
        try
        {
            var pcpStudentScores = await _genericRepository.GetAsync<tblStudentScore>(x => x.TopicId == 0 && !x.IsUploaded && (x.Id > startId && x.Id < endId));

            var studentScores = pcpStudentScores as tblStudentScore[] ?? pcpStudentScores.ToArray();

            var students = await _genericRepository.GetAsync<tblStudentLoginHistory>(x => studentScores.Select(z => z.StudentId).Contains(x.StudentId ?? 0));

            var scroreUpload = new List<PCPScoreUploadRequestDTO>();

            foreach (var student in students)
            {

                string Enroll = student.Enrollment ?? "";
                String Enroll2 = "1";
                if (Enroll.Length > 5)
                {
                    Enroll = Enroll.Substring(Enroll.Length - 6);
                    Enroll2 = Enroll.Substring(2, 1);
                    Enroll = Enroll.Substring(0, 2);
                }
                if (Enroll == "23" && Enroll2 !="4" && Enroll2 != "5")
                {
                    if (student.SSOID == null || student.DateOfBirth == null || student.StudentId == null) continue;

                    //var httpClient = new HttpClient();

                    //var rsosToken = _rsosSettings.Token;

                    //var rsosUrl = _rsosSettings.URL;

                    //var pcpBaseUrl = $"{rsosUrl}/new_api_set_student_sessional_exam_subject_marks";

                    var scoreAttribute = studentScores.Where(x => x.StudentId == student.StudentId);

                    foreach (var score in scoreAttribute)
                    {
                        if (double.TryParse(score.Score, out double result))
                        {
                            if (result > 0)
                            {
                                var subject = await _genericRepository.GetByIdAsync<tblSubject>(score.SubjectId);

                                var studentScore = result / 100.0 * (subject?.MaximumMarks ?? 0);

                                studentScore = Math.Round(studentScore * 2) / 2;

                                var list = new PCPScoreUploadRequestDTO()
                                {
                                    enrollment = student.Enrollment ?? "0",
                                    subject_id = score.SubjectId.ToString(),
                                    obtained_marks = Math.Ceiling(studentScore).ToString(CultureInfo.InvariantCulture)
                                };
                                scroreUpload.Add(list);
                            }
                        }
                    }
                }
            }

            var jsonFile = JsonConvert.SerializeObject(scroreUpload);

            string currentDateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            string fileName = $"data_{currentDateTime}_{startId}_To_{endId}.json";

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "documents", "jsonFile", fileName);

            File.WriteAllText(filePath, jsonFile);
            return true;
        }
        catch (Exception e)
        {
            return false;
            throw;
        }

    }


}