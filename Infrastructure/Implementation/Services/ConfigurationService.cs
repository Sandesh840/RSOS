using Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace Data.Implementation.Services;

public class ConfigurationService : IConfigurationService
{
    private readonly IConfiguration _configuration;

    public ConfigurationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetFirebaseConfiguration()
    {
        var firebaseConfig = _configuration.GetSection("FIREBASE_CONFIG").Value;
        
        return firebaseConfig.Length >= 100 ? firebaseConfig.Substring(0, 100) : firebaseConfig;
    }
}