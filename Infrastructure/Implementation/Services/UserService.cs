using System.Security.Cryptography;
using System.Text;
using Application.DTOs.User;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;

namespace Data.Implementation.Services;

public class UserService : IUserService
{
    private const char SegmentDelimiter = ':';
    private static readonly int PasswordSalt = 16;
    private readonly IGenericRepository _genericRepository;

    public UserService(IGenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<int> GetUserId(UserRequestDTO userRequest)
    {
        userRequest.UserName = DecryptStringAes(userRequest.HdUserName);

        var userId = await _genericRepository.GetFirstOrDefaultAsync<tblUser>(x => x.UserName == userRequest.UserName);
        
        return userId?.Id ?? 0;
    }

    public async Task<bool> IsUserAuthenticated(UserRequestDTO userRequest)
    {
        userRequest.UserName = DecryptStringAes(userRequest.HdUserName);
    
        userRequest.Password = DecryptStringAes(userRequest.HdPassword).Replace(userRequest.HdCp, "");

        if (userRequest.UserName == "keyError" || userRequest.Password == "keyError") return false;
    
        var user = await _genericRepository.GetFirstOrDefaultAsync<tblUser>(x => x.UserName == userRequest.UserName);
        
        return user != null && VerifyPassword(userRequest.Password, user.Password, PasswordSalt);
    }

    public async Task<bool> ChangePassword(int userId, string oldPassword, string newPassword)
    {
        oldPassword = DecryptStringAes(oldPassword);

        newPassword = DecryptStringAes(newPassword);
        
        var user = await _genericRepository.GetFirstOrDefaultAsync<tblUser>(x => x.Id == userId);
            
        if (user == null) return false;
            
        if (!VerifyPassword(oldPassword, user.Password, PasswordSalt)) return false;
            
        user.Password = CreatePasswordHash(newPassword.Trim(), CreateSalt(PasswordSalt));
            
        await _genericRepository.UpdateAsync(user);
            
        return true;
    }
    
    private static string DecryptStringAes(string cipherText)
    {
        var keyBytes = "8080808080808080"u8.ToArray();
        
        var iv = "8080808080808080"u8.ToArray();

        var encrypted = Convert.FromBase64String(cipherText);
        
        var decryptedFromJavascript = DecryptStringFromBytes(encrypted, keyBytes, iv);
        
        return string.Format(decryptedFromJavascript);
    }
    
    private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
    {
        if (cipherText is not { Length: > 0 })
        {
            throw new ArgumentNullException("cipherText");
        }
        if (key is not { Length: > 0 })
        {
            throw new ArgumentNullException("key");
        }
        if (iv is not { Length: > 0 })
        {
            throw new ArgumentNullException("key");
        }

        string plaintext = null;

        using var rijAlg = new RijndaelManaged();

        rijAlg.IV = iv;
        rijAlg.Key = key;
        rijAlg.FeedbackSize = 128;
        rijAlg.Mode = CipherMode.CBC;
        rijAlg.Padding = PaddingMode.PKCS7;

        var decrypt = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

        try
        {
            using var msDecrypt = new MemoryStream(cipherText);
                
            using var csDecrypt = new CryptoStream(msDecrypt, decrypt, CryptoStreamMode.Read);
                
            using var srDecrypt = new StreamReader(csDecrypt);
                
            plaintext = srDecrypt.ReadToEnd();
        }
        catch
        {
            plaintext = "keyError";
        }

        return plaintext;
    }
    
    private static bool VerifyPassword(string userPassword, string dbUserPassword, int saltSize)
    {
        if (string.IsNullOrEmpty(dbUserPassword)) return false;
        
        var salt = dbUserPassword.Substring(dbUserPassword.Length - CreateSalt(saltSize).Length);
        
        var hashedPasswordAndSalt = CreatePasswordHash(userPassword, salt);
        
        var passwordMatch = hashedPasswordAndSalt.Equals(dbUserPassword);
        
        return passwordMatch;
    }

    private static string CreatePasswordHash(string pwd, string salt)
    {
        var saltAndPwd = string.Concat(pwd, salt);
        
        var hashAlgorithm = SHA512.Create();
        
        var pass = new List<byte>(Encoding.Unicode.GetBytes(saltAndPwd));
        
        var hashedPassword = Convert.ToBase64String(hashAlgorithm.ComputeHash(pass.ToArray()));
        
        return string.Concat(hashedPassword, salt);
    }
    
    private static string CreateSalt(int size)
    {
        var rng = new RNGCryptoServiceProvider();
        
        var buff = new byte[size]; 

        rng.GetBytes(buff);
        
        return Convert.ToBase64String(buff);
    }
    
    private static string HashSecret(string input)
    {
        const int saltSize = 16;
        const int iterations = 100_000;
        const int keySize = 32;
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

    private static bool VerifyHash(string input, string hashString)
    {
        var segments = hashString.Split(SegmentDelimiter);
        var hash = Convert.FromHexString(segments[0]);
        var salt = Convert.FromHexString(segments[1]);
        var iterations = int.Parse(segments[2]);
        var algorithm = new HashAlgorithmName(segments[3]);
            
        var inputHash = Rfc2898DeriveBytes.Pbkdf2(
            input,
            salt,
            iterations,
            algorithm,
            hash.Length
        );

        return CryptographicOperations.FixedTimeEquals(inputHash, hash);
    }
}