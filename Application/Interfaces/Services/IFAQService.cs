using Application.DTOs.FAQ;

namespace Application.Interfaces.Services;

public interface IFAQService
{
    Task<List<FAQResponseDTO>> GetAllFAQs();
}