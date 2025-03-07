using System.Net;
using System.Text;
using Application.DTOs.Scheme;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Model.Models;
using Newtonsoft.Json;

namespace Data.Implementation.Services;

public class SchemeService : ISchemeService
{
    private readonly IConfiguration _configuration;
    private readonly IGenericRepository _genericRepository;

    public SchemeService(IConfiguration configuration, IGenericRepository genericRepository)
    {
        _configuration = configuration;
        _genericRepository = genericRepository;
    }

    public async Task<List<SchemeResponseDTO>> GetAllSchemes()
    {
        var schemes = await _genericRepository.GetAsync<tblScheme>(includeProperties: "SchemeFile");


        return schemes.OrderByDescending(x => x.Id).Select(x => new SchemeResponseDTO
        {
            Id = x.Id,
            Title = x.Header,
            Description = x.Description,
            //UploadedFileName = x.UploadedFileName,
            //UploadedFileUrl = x.UploadedFileUrl,
            SchemeFile = x.SchemeFile?.Select(f => new tblSchemeFile
            {
                Id = f.Id,
                FileUrl = f.FileUrl
            }).ToList() ?? new List<tblSchemeFile>(),

            URL = x.URL,
            IsTriggered = x.IsTriggered ? 1 : 0,
            IsActive = x.IsActive ? 1 : 0,
            CreatedOn = x.CreatedOn
        }).ToList();
    }

    public async Task<List<SchemeResponseDTO>> GetAllValidSchemes()
    {
        //var schemes = await _genericRepository.GetAsync<tblScheme>(x => 
        //    x.ValidFrom <= DateTime.Now && x.ValidTill >= DateTime.Now);

        var schemes = await _genericRepository.GetAsync<tblScheme>();

        return schemes.Select(x => new SchemeResponseDTO
        {
            Id = x.Id,
            Title = x.Header,
            Description = x.Description,
            //UploadedFileName = x.UploadedFileName,
            //UploadedFileUrl = x.UploadedFileUrl,
            SchemeFile = x.SchemeFile?.Select(f => new tblSchemeFile
            {
                Id = f.Id,
                FileUrl = f.FileUrl
            }).ToList() ?? new List<tblSchemeFile>(),

            URL = x.URL,
            IsActive = x.IsActive ? 1 : 0,
            IsTriggered = 1,
            CreatedOn = x.CreatedOn,
        }).ToList();
    }
    
    public async Task<SchemeResponseDTO> GetSchemeById(int schemeId)
    {
        //var scheme = await _genericRepository.GetByIdAsync<tblScheme>(schemeId);
        var schemes = await _genericRepository.GetAsync<tblScheme>(includeProperties: "SchemeFile");

        var data = schemes.Where(x => x.Id == schemeId).FirstOrDefault();

        if (data != null)
        {
            return new SchemeResponseDTO()
            {
                Id = data.Id,
                Title = data.Header,
                Description = data.Description,
                URL = data.URL,
                //UploadedFileUrl = scheme.UploadedFileUrl,
                //UploadedFileName = scheme.UploadedFileName

                SchemeFile = data.SchemeFile?.Select(f => new tblSchemeFile
                {
                    Id = f.Id,
                    FileUrl = f.FileUrl
                }).ToList() ?? new List<tblSchemeFile>(),
            };
        }

        return new SchemeResponseDTO();
    }


    public async Task<List<tblSchemeFile>> GetSchemeFileById(int fileId)
    {
        var scheme = await _genericRepository.GetAsync<tblSchemeFile>(
       f => f.FileId == fileId); // Correct filter expression

        return scheme.ToList();
    }

    public async Task DeleteSchemeFileById(int fileId)
    {
        await _genericRepository.DeleteAsync<tblSchemeFile>(fileId);
    }


    public async Task<int> InsertScheme(SchemeRequestDTO scheme)
    {
        var schemeModel = new tblScheme()
        {
            Header = scheme.Title,
            Description = scheme.Description,
            URL = scheme.URL,

            SchemeFile = scheme.tblSchemeFile?.Select(f => new tblSchemeFile
            {
                Id = f.Id,
                FileUrl = f.FileUrl
            }).ToList() ?? new List<tblSchemeFile>(),

            IsActive = true,
            CreatedBy = scheme.UserId,
            CreatedOn = DateTime.Now
        };

        var newId= await _genericRepository.InsertAsync(schemeModel);
        return newId;
    }

    public async Task UpdateScheme(SchemeRequestDTO scheme)
    {
        var schemeModel = await _genericRepository.GetByIdAsync<tblScheme>(scheme.Id);

        if (schemeModel != null)
        {
            schemeModel.Header = scheme.Title;
            schemeModel.Description = scheme.Description;
            schemeModel.URL = scheme.URL;

            schemeModel.LastUpdatedBy = scheme.UserId;
            schemeModel.LastUpdatedOn = DateTime.Now;

            if (scheme.tblSchemeFile != null)
            {
                schemeModel.SchemeFile = scheme.tblSchemeFile;
            }


            //if (scheme.UploadedFileUrl != null)
            //{
            //    schemeModel.UploadedFileName = scheme.UploadedFileName;
            //    schemeModel.UploadedFileUrl = scheme.UploadedFileUrl;
            //}

            await _genericRepository.UpdateAsync(schemeModel);
        }
    }

    public async Task UpdateSchemeStatus(int schemeId)
    {
        var schemeModel = await _genericRepository.GetByIdAsync<tblScheme>(schemeId);
        
        if (schemeModel != null)
        {
            schemeModel.IsActive = !schemeModel.IsActive;
            
            await _genericRepository.UpdateAsync(schemeModel);
        }
    }

    public async Task NotifyScheme(int schemeId)
    {
        var scheme = await _genericRepository.GetByIdAsync<tblScheme>(schemeId);

        if(scheme == null) return;
        
        scheme.IsTriggered = true;

        await _genericRepository.UpdateAsync(scheme);
        
        var latestLoginDetails = (await _genericRepository
            .GetAsync<tblStudentLoginDetail>())
            .GroupBy(s => s.SSOID)
            .Select(g => g.OrderByDescending(s => s.LoginTime).First())
            .ToList();

        foreach (var detail in latestLoginDetails)
        {
            var userDeviceToken = detail.DeviceRegistrationToken;
            
            // Server Key from FCM Console
            var serverKey = $"key={_configuration["FCM:SERVER_KEY"]}";

            // Sender ID from FCM Console
            var senderId = $"id={_configuration["FCM:SENDER_ID"]}";

            var tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
        
            tRequest.Method = "post";
        
            tRequest.Headers.Add($"Authorization: {serverKey}");
        
            tRequest.Headers.Add($"Sender: {senderId}");
        
            tRequest.ContentType = "application/json";
        
            var payload = new
            {
                to = userDeviceToken,
                scheme = new
                {
                    title = scheme.Header,
                    body = scheme.Description
                },
                data = new 
                {
                    
                }
            };
        
            var postBody = JsonConvert.SerializeObject(payload);
        
            var byteArray = Encoding.UTF8.GetBytes(postBody);
        
            tRequest.ContentLength = byteArray.Length;

            await using var dataStream = await tRequest.GetRequestStreamAsync();
        
            await dataStream.WriteAsync(byteArray, 0, byteArray.Length);
        
            using var tResponse = await tRequest.GetResponseAsync();
        
            await using var dataStreamResponse = tResponse.GetResponseStream();

            using var tReader = new StreamReader(dataStreamResponse);
        
            await tReader.ReadToEndAsync();
        }
    }
    
    public async Task NotifyScheme(string registrationToken)
    {
        // Server Key from FCM Console
        var serverKey = $"key={_configuration["FCM:SERVER_KEY"]}";

        // Sender ID from FCM Console
        var senderId = $"id={_configuration["FCM:SENDER_ID"]}";

        var tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
    
        tRequest.Method = "post";
    
        tRequest.Headers.Add($"Authorization: {serverKey}");
    
        tRequest.Headers.Add($"Sender: {senderId}");
    
        tRequest.ContentType = "application/json";
    
        var payload = new
        {
            to = registrationToken,
            scheme = new
            {
                title = "Header",
                body = "Description"
            },
            data = new 
            {
                
            }
        };
    
        var postBody = JsonConvert.SerializeObject(payload);
    
        var byteArray = Encoding.UTF8.GetBytes(postBody);
    
        tRequest.ContentLength = byteArray.Length;

        await using var dataStream = await tRequest.GetRequestStreamAsync();
    
        await dataStream.WriteAsync(byteArray, 0, byteArray.Length);
    
        using var tResponse = await tRequest.GetResponseAsync();
    
        await using var dataStreamResponse = tResponse.GetResponseStream();

        using var tReader = new StreamReader(dataStreamResponse);
    
        await tReader.ReadToEndAsync();
    }
}