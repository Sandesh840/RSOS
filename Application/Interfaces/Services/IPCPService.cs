using Application.DTOs.PCP;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Services;

public interface IPCPService
{
    Task<PCPQuestionsResponseDTO> GetPCPQuestionsByClass(int classId, int type);

    Task<List<PCPQuestionResponseDTO>> GetUploadedQuestionSheets(int subjectCode, int type);
    
    Task<(string, string)> GetUploadedQuestionSheetById(int questionSheetId);

    Task<(bool, string)> IsUploadedSheetValid(PCPQuestionRequestDTO question);

    Task<bool> UploadQuestions(PCPQuestionRequestDTO question);

    Task UploadQuestionsWorksheet(PCPQuestionSheetRequestDTO questionSheet);
    
    Task ArchiveQuestions(int subjectId, int type);
}