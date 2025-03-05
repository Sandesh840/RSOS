using Application.DTOs.PcpDate;


namespace Application.Interfaces.Services;

public interface IPcpDatesService
{
    Task<List<PcpDatesResponseDTO>> GetAllPcpDates();
    
    Task InsertPcpDates(PcpDatesRequestDTO pcpDates);
    
    Task UpdatePcpDatesStatus(int pcpDatesId);
}