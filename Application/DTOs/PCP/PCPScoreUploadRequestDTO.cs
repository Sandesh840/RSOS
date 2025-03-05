using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.PCP;

public class PCPScoreUploadRequestDTO
{
    
    public string enrollment { get; set; }
    
    public string subject_id { get; set; }
    
    public string obtained_marks { get; set; }
}