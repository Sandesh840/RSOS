using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IBulkUploadService
    {
        Task<bool> GetJsonFile(int startId, int endId);


    }
}