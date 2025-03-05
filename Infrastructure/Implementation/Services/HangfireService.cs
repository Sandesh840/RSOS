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

namespace Data.Implementation.Services;

public class HangfireService : IHostedService
{
    private readonly RsosSettings _rsosSettings;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly HangfireSettings _hangfireSettings;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public HangfireService(IOptions<RsosSettings> rsosSettings, IOptions<HangfireSettings> hangfireSettings, IServiceScopeFactory scopeFactory, IWebHostEnvironment webHostEnvironment)
    {
        _scopeFactory = scopeFactory;
        _rsosSettings = rsosSettings.Value;
        _hangfireSettings = hangfireSettings.Value;
        _webHostEnvironment = webHostEnvironment;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        //var jobOptions = new RecurringJobOptions
        //{
        //    TimeZone = TimeZoneInfo.Local,
        //    MisfireHandling = MisfireHandlingMode.Strict
        //};

        //RecurringJob.AddOrUpdate("ePCP", () => InsertData(), Cron.Daily(_hangfireSettings.StartingHours, _hangfireSettings.StartingMinutes), jobOptions);

        //RecurringJob.AddOrUpdate("ePCP", () => InsertDataNew(), Cron.Hourly(), jobOptions);

        return Task.CompletedTask;
    }

    public async Task InsertData()
    {
        using var scope = _scopeFactory.CreateScope();

        var genericRepository = scope.ServiceProvider.GetRequiredService<IGenericRepository>();

        try
        {
            var pcpStudentScores =
            await genericRepository.GetAsync<tblStudentScore>(x =>
                x.TopicId == 0 && !x.IsUploaded && (x.Id > 40000 && x.Id < 50000));

            var studentScores = pcpStudentScores as tblStudentScore[] ?? pcpStudentScores.ToArray();

            var students =
                await genericRepository.GetAsync<tblStudentLoginHistory>(x =>
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
                                var subject = await genericRepository.GetByIdAsync<tblSubject>(score.SubjectId);

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

                                        await genericRepository.UpdateAsync(score);

                                        var successLog =
                                            $"DOIT Success - Student SSOID: {student.SSOID}, Subject Id: {score.SubjectId.ToString()}, Score: {studentScore}";

                                        var exception = new tblExceptionLog()
                                        {
                                            DateTime = DateTime.Now,
                                            ErrorLog = successLog
                                        };

                                        await genericRepository.InsertAsync(exception);
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

                                        await genericRepository.InsertAsync(exception);
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

            await genericRepository.InsertAsync(exception);
            await InsertData();
        }
    }


    //public async Task InsertDataNew()
    //{
    //    using var scope = _scopeFactory.CreateScope();

    //    var genericRepository = scope.ServiceProvider.GetRequiredService<IGenericRepository>();

    //    try
    //    {
    //        var pcpStudentScores =
    //        await genericRepository.GetAsync<tblStudentScore>(x =>
    //            x.TopicId == 0 && !x.IsUploaded && (x.Id > 40000 && x.Id < 50000));

    //        var studentScores = pcpStudentScores as tblStudentScore[] ?? pcpStudentScores.ToArray();

    //        var students =
    //            await genericRepository.GetAsync<tblStudentLoginHistory>(x =>
    //                studentScores.Select(z => z.StudentId).Contains(x.StudentId ?? 0));


    //        foreach (var student in students)
    //        {
    //            string Enroll = student.Enrollment ?? "";
    //            String Enroll2 = "1";
    //            if (Enroll.Length > 5)
    //            {
    //                Enroll = Enroll.Substring(Enroll.Length - 6);
    //                Enroll2 = Enroll.Substring(2, 1);
    //                Enroll = Enroll.Substring(0, 2);
    //            }
    //            if (Enroll == "23" && Enroll2 != "4" && Enroll2 != "5")
    //            {
    //                //if (DateTime.Now.Hour >= _hangfireSettings.EndingHour && DateTime.Now.Hour <= _hangfireSettings.StartingHours) return;

    //                if (student.SSOID == null || student.DateOfBirth == null || student.StudentId == null) continue;

    //                var httpClient = new HttpClient();

    //                var rsosToken = _rsosSettings.Token;

    //                var rsosUrl = _rsosSettings.URL;

    //                var loginBaseUrl = $"{rsosUrl}/new_api_student_login";

    //                var loginQueryParams = new System.Collections.Specialized.NameValueCollection
    //            {
    //                { "ssoid", student.SSOID },
    //                { "dob", student.DateOfBirth },
    //                { "token", rsosToken }
    //            };

    //                var loginUriBuilder = new UriBuilder(loginBaseUrl)
    //                {
    //                    Query = string.Join("&", Array.ConvertAll(loginQueryParams.AllKeys,
    //                        key => $"{Uri.EscapeDataString(key!)}={Uri.EscapeDataString(loginQueryParams[key]!)}"))
    //                };

    //                var loginPostData = new StringContent("{\"key\": \"value\"}", Encoding.UTF8, "application/json");

    //                var loginResponse = await httpClient.PostAsync(loginUriBuilder.Uri, loginPostData);

    //                if (loginResponse.IsSuccessStatusCode)
    //                {
    //                    var loginResponseData = await loginResponse.Content.ReadAsStringAsync();

    //                    var loginApiResponse = JsonConvert.DeserializeObject<RSOSLoginResponse>(loginResponseData);

    //                    if (loginApiResponse is { Status: true })
    //                    {
    //                        var studentResponseData = JsonConvert.DeserializeObject<LoginResponseDTO>(loginResponseData);

    //                        var studentLoginData = studentResponseData!.Data.Student;

    //                        var secureToken = studentResponseData.secure_token;

    //                        var pcpBaseUrl = $"{rsosUrl}/new_api_set_student_sessional_exam_subject_marks";Test_Publish (18-07-2024)

    //                        var scoreAttribute = studentScores.Where(x => x.StudentId == studentLoginData.Id);

    //                        foreach (var score in scoreAttribute)
    //                        {
    //                            if (double.TryParse(score.Score, out var result))
    //                            {
    //                                var subject = await genericRepository.GetByIdAsync<tblSubject>(score.SubjectId);

    //                                var studentScore = result / 100.0 * (subject?.MaximumMarks ?? 0);

    //                                studentScore = Math.Round(studentScore * 2) / 2;

    //                                var pcpQueryParams = new System.Collections.Specialized.NameValueCollection
    //                            {
    //                                { "token", rsosToken },
    //                                { "secure_token", secureToken},
    //                                { "enrollment", studentLoginData.Enrollment },
    //                                { "ssoid", student.SSOID },
    //                                { "subject_id", score.SubjectId.ToString() },
    //                                { "obtained_marks", Math.Ceiling(studentScore).ToString(CultureInfo.InvariantCulture) }
    //                            };

    //                                var pcpUriBuilder = new UriBuilder(pcpBaseUrl)
    //                                {
    //                                    Query = string.Join("&", Array.ConvertAll(pcpQueryParams.AllKeys,
    //                                        key => $"{Uri.EscapeDataString(key!)}={Uri.EscapeDataString(pcpQueryParams[key]!)}"))
    //                                };

    //                                var pcpPostData = new StringContent("{\"key\": \"value\"}", Encoding.UTF8, "application/json");

    //                                var pcpResponse = await httpClient.PostAsync(pcpUriBuilder.Uri, pcpPostData);

    //                                if (pcpResponse.IsSuccessStatusCode)
    //                                {
    //                                    var pcpResponseData = await pcpResponse.Content.ReadAsStringAsync();

    //                                    var pcpApiResponse = JsonConvert.DeserializeObject<StudentSessionalMarksDTO>(pcpResponseData);

    //                                    if (pcpApiResponse is { Status: true })
    //                                    {
    //                                        score.IsUploaded = true;
    //                                        score.LastUpdatedOn = DateTime.Now;

    //                                        await genericRepository.UpdateAsync(score);

    //                                        var successLog =
    //                                            $"DOIT Success - Student SSOID: {student.SSOID}, Subject Id: {score.SubjectId.ToString()}, Score: {studentScore}";

    //                                        var exception = new tblExceptionLog()
    //                                        {
    //                                            DateTime = DateTime.Now,
    //                                            ErrorLog = successLog
    //                                        };

    //                                        await genericRepository.InsertAsync(exception);
    //                                    }
    //                                    else
    //                                    {
    //                                        var errorLog =
    //                                            $"DOIT Exception - Student SSOID: {student.SSOID}, Subject Id: {score.SubjectId.ToString()}, Error: {pcpApiResponse?.Error.ToString()}";

    //                                        var exception = new tblExceptionLog()
    //                                        {
    //                                            DateTime = DateTime.Now,
    //                                            ErrorLog = errorLog
    //                                        };

    //                                        await genericRepository.InsertAsync(exception);
    //                                    }
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        var exception = new tblExceptionLog()
    //        {
    //            DateTime = DateTime.Now,
    //            ErrorLog = $"Hangfire Service - {e.Message}"
    //        };

    //        await genericRepository.InsertAsync(exception);
    //        await InsertDataNew();
    //    }
    //}


    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}