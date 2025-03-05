using Application.DTOs.NewsAndAlert;
using Application.Interfaces.Services;
using Application.Interfaces.Repositories;

namespace Data.Implementation.Services;

public class NewsAndAlertService : INewsAndAlertService
{
    private readonly IGenericRepository _genericRepository;

    public NewsAndAlertService(IGenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<List<NewsAndAlertResponseDTO>> GetAllNewsAndAlert()
    {
        var newsAndAlert = await _genericRepository.GetAsync<tblNewsAndAlert>();

        return newsAndAlert.OrderByDescending(x => x.Id).Select(x => new NewsAndAlertResponseDTO
        {
            Id = x.Id,
            Description = x.Description,
            ValidFrom = x.ValidFrom,
            ValidTill = x.ValidTill,
            IsActive = x.IsActive ? 1 : 0,  
            CreatedOn = x.CreatedOn
        }).ToList();
    }

    public async Task<List<NewsAndAlertResponseDTO>> GetAllValidNewsAndAlert()
    {
        var newsAndAlert = await _genericRepository.GetAsync<tblNewsAndAlert>(x =>
            x.IsActive && x.ValidFrom <= DateTime.Now && x.ValidTill >= DateTime.Now);

        return newsAndAlert.Select(x => new NewsAndAlertResponseDTO
        {
            Id = x.Id,
            Description= x.Description,
            ValidFrom = x.ValidFrom,
            ValidTill= x.ValidTill,
            IsActive = x.IsActive ? 1 : 0,
            CreatedOn = x.CreatedOn,
        }).ToList();
    }

    public async Task<NewsAndAlertResponseDTO> GetNewsAndAlertById(int newsAndAlertId)
    {
        var newsAndAlert = await _genericRepository.GetByIdAsync<tblNewsAndAlert>(newsAndAlertId);

        if (newsAndAlert != null)
        {
            return new NewsAndAlertResponseDTO()
            {
                Id = newsAndAlert.Id,
                Description = newsAndAlert.Description,
                ValidTill = newsAndAlert.ValidTill,
                ValidFrom = newsAndAlert.ValidFrom,
            };
        }

        return new NewsAndAlertResponseDTO();
    }

    public async Task InsertNewsAndAlert(NewsAndAlertRequestDTO newsAndAlert)
    {
        var newsAndAlertModel = new tblNewsAndAlert()
        {
            Header = "",
            Description = newsAndAlert.Description,
            ValidFrom = newsAndAlert.ValidFrom,
            ValidTill = newsAndAlert.ValidTill,
            IsActive = true,
            CreatedBy = newsAndAlert.UserId,
            CreatedOn = DateTime.Now,
        };

        await _genericRepository.InsertAsync(newsAndAlertModel);
    }

    public async Task UpdateNewsAndAlert(NewsAndAlertRequestDTO newsAndAlert)
    {
        var newsAndAlertModel = await _genericRepository.GetByIdAsync<tblNewsAndAlert>(newsAndAlert.Id);

        if (newsAndAlertModel != null)
        {
            newsAndAlertModel.Header = "";
            newsAndAlertModel.Description = newsAndAlert.Description;
            newsAndAlertModel.ValidFrom = newsAndAlert.ValidFrom;
            newsAndAlertModel.ValidTill = newsAndAlert.ValidTill;

            newsAndAlertModel.LastUpdatedBy = newsAndAlert.UserId;
            newsAndAlertModel.LastUpdatedOn = DateTime.Now;

            await _genericRepository.UpdateAsync(newsAndAlertModel);
        }
    }
    
    public async Task UpdateNewsAndAlertStatus(int newsAndAlertId)
    {
        var newsAndAlert = await _genericRepository.GetByIdAsync<tblNewsAndAlert>(newsAndAlertId);

        if (newsAndAlert != null)
        {
            newsAndAlert.IsActive = !newsAndAlert.IsActive;

            await _genericRepository.UpdateAsync(newsAndAlert);
        }
    }
}