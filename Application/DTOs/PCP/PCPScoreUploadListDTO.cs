using Microsoft.AspNetCore.Http;

namespace Application.DTOs.PCP;

public class PCPScoreUploadListDTO
{
    public List<PCPScoreUploadRequestDTO> pCPScoreUploadRequestDTO { get; set; }
}