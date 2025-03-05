using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Linq;
using System.Security.Cryptography;

namespace Common.Utilities
{
    public class CompressionUtility
    {

        public byte[] CompressJson(object data)
        {
            var jsonString = JsonSerializer.Serialize(data);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            byte[] compressedBytes;

            using (var outputStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    gzipStream.Write(jsonBytes, 0, jsonBytes.Length);
                }
                compressedBytes = outputStream.ToArray();
            }
            return compressedBytes;
        }
        public byte[] CompressJsonOld(object data)
        {
            var jsonString = JsonSerializer.Serialize(data);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);

            using (var outputStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    gzipStream.Write(jsonBytes, 0, jsonBytes.Length);
                }
                return outputStream.ToArray();
            }
        }
        public string DecompressString(byte[] inputBytes)
        {
            using (var inputStream = new MemoryStream(inputBytes))
            using (var outputStream = new MemoryStream())
            using (var compressionStream = new GZipStream(inputStream, CompressionMode.Decompress))
            {
                compressionStream.CopyTo(outputStream);
                byte[] outputBytes = outputStream.ToArray();
                return Encoding.UTF8.GetString(outputBytes);
            }
        }

        public byte[] CompressAndEncryptString(string str, byte[] key, byte[] iv)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(str);
            using (var outputStream = new MemoryStream())
            {
                using (var aesAlg = Aes.Create())
                {
                    aesAlg.Key = key;
                    aesAlg.IV = iv;

                    using (var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                    using (var compressionStream = new GZipStream(outputStream, CompressionMode.Compress))
                    using (var cryptoStream = new CryptoStream(compressionStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(inputBytes, 0, inputBytes.Length);
                    }
                }
                return outputStream.ToArray();
            }
        }

        public string DecryptAndDecompressString(byte[] inputBytes, byte[] key, byte[] iv)
        {
            using (var inputStream = new MemoryStream(inputBytes))
            using (var outputStream = new MemoryStream())
            {
                using (var aesAlg = Aes.Create())
                {
                    aesAlg.Key = key;
                    aesAlg.IV = iv;

                    using (var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                    using (var decompressionStream = new GZipStream(inputStream, CompressionMode.Decompress))
                    using (var cryptoStream = new CryptoStream(decompressionStream, decryptor, CryptoStreamMode.Read))
                    {
                        cryptoStream.CopyTo(outputStream);
                    }
                }
                byte[] outputBytes = outputStream.ToArray();
                return Encoding.UTF8.GetString(outputBytes);
            }
        }
    }
}
