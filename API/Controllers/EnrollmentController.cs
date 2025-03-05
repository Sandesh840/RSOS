using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Application.DTOs.Enrollment;
using Common.Utilities;
using DocumentFormat.OpenXml.Vml.Spreadsheet;

namespace RSOS.Controllers;

public class EnrollmentController : BaseController<EnrollmentController>
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    [Authentication]
    public IActionResult Index()
    {
        return View();
    }
    
    public IActionResult DownloadTemplate()
    {
        var workbook = _enrollmentService.DownloadEnrollmentSheet();

        using var stream = new MemoryStream();

        workbook.SaveAs(stream);

        var content = stream.ToArray();
        var dateTime = DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss");

        return File(
            content,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Enrollment_{dateTime}.xlsx"
        );
    }

    [HttpPost]
    public async Task<IActionResult> UploadEnrollmentExcel(IFormFile? excelFile)
    {
        if (excelFile == null || excelFile.Length == 0)
        {
            return Json(new
            {
                errorType = 1,
                errorMessage = "Please upload an excel file before submitting your request."
            });
        }

        using var workbook = new XLWorkbook(excelFile.OpenReadStream());
        
        var worksheet = workbook.Worksheet(1);

        if (worksheet.Name != "Enrollment")
        {
            return Json(new
            {
                errorType = 1,
                errorMessage = "Please upload the excel file provided by the system itself."
            });
        }
        
        var enrollmentDetails = _enrollmentService.ProcessWorksheet(worksheet);
        
        if (enrollmentDetails.Count == 0)
        {
            return Json(new
            {
                errorType = 1,
                errorMessage = "No data found in the excel file, please insert at least a single row of data before submitting your request."
            });
        }
        
        var duplicateEnrollmentIds = enrollmentDetails
            .GroupBy(x => x.EnrollmentId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateEnrollmentIds.Any())
        {
            return Json(new
            {
                errorType = 1,
                errorMessage = "Duplicate enrollment identifier(s) found in the excel file. The identifiers are " + string.Join(", ", duplicateEnrollmentIds)
            });
        }

        var rowsWithZeroEnrollmentId = new List<int>();

        for (var i = 0; i < enrollmentDetails.Count; i++)
        {
            if (enrollmentDetails[i].EnrollmentId == 0)
            {
                rowsWithZeroEnrollmentId.Add(i + 2);
            }
        }

        if (rowsWithZeroEnrollmentId.Any())
        {
            return Json(new
            {
                errorType = 1,
                errorMessage = "Please correct the following rows having empty enrollment identifier(s): " + string.Join(", ", rowsWithZeroEnrollmentId),
            });
        }

        var userId = HttpContext.Session.GetInt32("UserId");
        var enrollmentList = new List<EnrollmentRequestDTO>();

        foreach (var item in enrollmentDetails)
        {
            var enrollment = new EnrollmentRequestDTO
            {
                EnrollmentId = item.EnrollmentId,
                Reason = item.Reason,
                Status = item.Status,
                UserId = userId ?? 1
            };
            enrollmentList.Add(enrollment); 
        }
        
        await _enrollmentService.InsertEnrollments(enrollmentList);

        return Json(new
        {
            errorType = 0,
            successMessage = "Enrollment status successfully uploaded."
        });
    }
}