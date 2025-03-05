using Application.DTOs.Authentication.Session;
using Application.DTOs.Enrollment;
using Application.Interfaces.Repositories;
using ClosedXML.Excel;
using Data;

namespace Application.Interfaces.Services;

public interface IEnrollmentService
{
    Task<EnrollmentResponseDTO> GetEnrollmentStatus(int enrollmentId);
    
    Task InsertEnrollments(List<EnrollmentRequestDTO> enrollmentDetails);
    
    XLWorkbook DownloadEnrollmentSheet();

    List<EnrollmentRequestDTO> ProcessWorksheet(IXLWorksheet worksheet);
    Task<List<SessionResponseDTO>> GetAllSession();
   
}