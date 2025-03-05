using Application.DTOs.NewsAndAlert;
using Application.DTOs.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services;

public interface INewsAndAlertService
{
    Task<List<NewsAndAlertResponseDTO>> GetAllNewsAndAlert();

    Task<List<NewsAndAlertResponseDTO>> GetAllValidNewsAndAlert();

    Task<NewsAndAlertResponseDTO> GetNewsAndAlertById(int newsAndAlertId);

    Task InsertNewsAndAlert(NewsAndAlertRequestDTO newsAndAlert);

    Task UpdateNewsAndAlert(NewsAndAlertRequestDTO newsAndAlert);
    
    Task UpdateNewsAndAlertStatus(int newsAndAlertId);
}