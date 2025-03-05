using System.Net;
using Application.DTOs.Base;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Content;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Common.Utilities;
using Application.DTOs.PCP;
using Data.Implementation.Repositories;
using Data;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;

using Hangfire;
using Application.DTOs.Authentication;
using Application.DTOs.Student;
using Application.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using DocumentFormat.OpenXml.InkML;
using Application.DTOs.Dropdown;
using Irony.Parsing;
using Newtonsoft.Json.Linq;
using Azure;
using Microsoft.AspNetCore.Hosting;

namespace Data.Implementation.Services;

[ApiController]
[Route("api/BulkUploadScore")]
[IgnoreAntiforgeryToken]
public class BulkUploadScoreController : ControllerBase
{
    private readonly IBulkUploadService _bulkUploadService;

    public BulkUploadScoreController(IBulkUploadService bulkUploadService)
    {
       _bulkUploadService = bulkUploadService;
    }

    //[Authorize]
    [HttpPost("get-bulk-data-json")]
    public async Task<IActionResult> GetDataJsonFile(int startId, int endId)
    {
        try
        {
            bool IsFile = await _bulkUploadService.GetJsonFile(startId,endId);
            if(IsFile)
            {
                var response = new ResponseDTO<List<ContentResponseDTO>>()
                {
                    Status = "Success",
                    Message = "Json File Create Successfully",
                    StatusCode = HttpStatusCode.OK,
                };
                return Ok(response);
            }
            else {
                var response = new ResponseDTO<List<ContentResponseDTO>>()
                {
                    Status = "Failed",
                    Message = "Json File Create Failed",
                    StatusCode = HttpStatusCode.OK,
                };
                return Ok(response);
            }
            
        }
       
        catch (Exception ex) {
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