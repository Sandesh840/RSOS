using System.Security.Cryptography;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;

namespace Data.Persistence.Seed;

public class DbInitializerService : IDbInitializerService
{
    private const char SegmentDelimiter = ':';
    private readonly IGenericRepository _genericRepository;

    public DbInitializerService(IGenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task Initialize()
    {
        var users = await _genericRepository.Count<tblUser>();
        
        if(users > 0) return;

        var superAdminUser = new tblUser
        {
            UserName = "superadmin",
            Password = HashSecret("Admin@123")
        };
        
        await _genericRepository.InsertAsync(superAdminUser);
    }

    private static string HashSecret(string input)
    {
        var saltSize = 16;
        var iterations = 100_000;
        var keySize = 32;
        var algorithm = HashAlgorithmName.SHA256;
        var salt = RandomNumberGenerator.GetBytes(saltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(input, salt, iterations, algorithm, keySize);

        var result = string.Join(
            SegmentDelimiter,
            Convert.ToHexString(hash),
            Convert.ToHexString(salt),
            iterations,
            algorithm
        );

        return result;
    }
}