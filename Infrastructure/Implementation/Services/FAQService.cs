using Application.DTOs.FAQ;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;

namespace Data.Implementation.Services;

public class FAQService : IFAQService
{
    private readonly IGenericRepository _genericRepository;

    public FAQService(IGenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<List<FAQResponseDTO>> GetAllFAQs()
    {
        var faqs = await _genericRepository.GetAsync<tblFAQ>(x => x.IsActive);

        var result = faqs.OrderBy(x => x.Sequence).Select(x => new FAQResponseDTO()
        {
            Id = x.Id,
            Sequence = x.Sequence,
            Title = $"{x.Sequence}. {x.Title}",
            Description = x.Description
        }).ToList();

        return result;
    }
}