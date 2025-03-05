using Application.DTOs.Authentication.Session;
using Application.DTOs.Enrollment;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using ClosedXML.Excel;
using Data.Implementation.Repositories;

namespace Data.Implementation.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IGenericRepository _repository;

    public EnrollmentService(IGenericRepository repository) 
    {
        _repository = repository;
    }

    public async Task<EnrollmentResponseDTO> GetEnrollmentStatus(int enrollmentId)
    {
        var enrollment = await _repository.GetFirstOrDefaultAsync<tblEnrollmentStatus>(x => x.EnrollmentId == enrollmentId);
        
        if (enrollment != null)
        {
            return new EnrollmentResponseDTO()
            {
                EnrollmentId = enrollment.EnrollmentId,
                Status = enrollment.Status,
                Reason = enrollment.Reason,
            };
        }

        return new EnrollmentResponseDTO();
    }

    public async Task InsertEnrollments(List<EnrollmentRequestDTO> enrollmentDetails)
    {
        foreach (var enrollment in enrollmentDetails)
        {
            var existentEnrollment = await _repository.GetFirstOrDefaultAsync<tblEnrollmentStatus>(x => x.EnrollmentId == enrollment.EnrollmentId);

            if (existentEnrollment != null)
            {
                existentEnrollment.Status = enrollment.Status;
                existentEnrollment.Reason = enrollment.Reason;
                existentEnrollment.LastUpdatedBy = enrollment.UserId;
                existentEnrollment.LastUpdatedOn = DateTime.Now;

                await _repository.UpdateAsync(existentEnrollment);
            }
            else
            {
                var result = new tblEnrollmentStatus
                {
                    EnrollmentId = enrollment.EnrollmentId,
                    Status = enrollment.Status,
                    Reason = enrollment.Reason,
                    CreatedBy = enrollment.UserId,
                    CreatedOn = DateTime.Now,
                    IsActive = true,
                };

                await _repository.InsertAsync(result);
            }
        }
    }
    
    public XLWorkbook DownloadEnrollmentSheet()
    {
        var workbook = new XLWorkbook();
        
        workbook.Style.Font.FontName = "Arial";
        
        var workSheet = workbook.Worksheets.Add("Enrollment");

        const int currentRow = 1;

        workSheet.Column(1).Width = 20;
        workSheet.Column(2).Width = 50;

        workSheet.Cell(currentRow, 1).Value = "Enrollment ID";
        workSheet.Cell(currentRow, 2).Value = "Reason";

        for (var col = 1; col <= 2; col++)
        {
            ApplyColors(workSheet.Cell(currentRow, col));
            ApplyHeaderStyles(workSheet.Cell(currentRow, col));
        }

        foreach (var cell in workSheet.CellsUsed())
        {
            if (cell.IsMerged())
            {
                var mergedRange = cell.MergedRange();

                foreach (var mergedCell in mergedRange.Cells())
                {
                    mergedCell.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    mergedCell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    mergedCell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    mergedCell.Style.Border.RightBorder = XLBorderStyleValues.Thin;

                    mergedCell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    mergedCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }
            }
            else
            {
                cell.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.RightBorder = XLBorderStyleValues.Thin;

                cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            cell.Style.Font.FontName = "Arial";
        }
        
        return workbook;
    }

    public List<EnrollmentRequestDTO> ProcessWorksheet(IXLWorksheet worksheet)
    {
        var result = worksheet.Rows().Skip(1)
            .Select(row => new EnrollmentRequestDTO
            {
                EnrollmentId = row.Cell(1).GetValue<int?>() ?? 0, 
                Reason = row.Cell(2).GetValue<string>(), 
                Status = string.IsNullOrEmpty(row.Cell(2).GetValue<string>())
            }).ToList();

        return result;
    }
    
    private static void ApplyColors(IXLCell cell)
    {
        var darkColor = XLColor.Black;
        cell.Style.Font.FontColor = darkColor;
    }
    
    private static void ApplyHeaderStyles(IXLCell cell)
    {
        cell.Style.Font.Bold = true;
        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
    }
    public async Task<List<SessionResponseDTO>> GetAllSession()
    {
        var sessions = await _repository.GetAsync<tblSession>(x => x.IsActive);

        return sessions.OrderBy(x => x.SessionId).Select(x => new SessionResponseDTO()
        {
            Id = x.Id,
            SessionId = x.SessionId,
            Session = x.Session,
            Stream = x.Stream,
            IsPcp = x.IsPcp,
            IsAdmit = x.IsAdmit,
            IsReeval = x.IsReeval,
            IsResult = x.IsResult,
        }).ToList();
    }
}