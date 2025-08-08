using System;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;           // BasicAWSCredentials
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Clinica.Services
{
    public class CloudflareR2Service
    {
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly string _bucketName;
        private readonly IConfiguration _config;


        public CloudflareR2Service(IConfiguration config)
        {
            _config = config!;
            _accessKey = config["Cloudflare:R2AccessKey"] ?? Environment.GetEnvironmentVariable("Cloudflare__R2AccessKey") ?? "";
            _secretKey = config["Cloudflare:R2SecretKey"] ?? Environment.GetEnvironmentVariable("Cloudflare__R2SecretKey") ?? "";
            _bucketName = config["Cloudflare:R2BucketName"] ?? Environment.GetEnvironmentVariable("Cloudflare__R2BucketName") ?? "";
            
            var accountId = config["Cloudflare:AccountId"] ?? Environment.GetEnvironmentVariable("Cloudflare__AccountId");
            
            // DEBUG: Imprime las credenciales (sin mostrar la clave completa)
            Console.WriteLine($"[R2] AccountId: {accountId}");
            Console.WriteLine($"[R2] AccessKey: {(_accessKey.Length > 0 ? $"{_accessKey[..4]}***" : "MISSING")}");
            Console.WriteLine($"[R2] SecretKey: {(_secretKey.Length > 0 ? $"{_secretKey[..4]}***" : "MISSING")}");
            Console.WriteLine($"[R2] BucketName: {_bucketName}");
            
            if (string.IsNullOrEmpty(_accessKey) || string.IsNullOrEmpty(_secretKey) || string.IsNullOrEmpty(accountId))
            {
                throw new InvalidOperationException("Cloudflare R2 credentials are not properly configured");
            }
        }


        public async Task<string> UploadDocumentToCloudflareR2(IFormFile file)
        {
            var accountId = _config["Cloudflare:AccountId"] 
                            ?? Environment.GetEnvironmentVariable("Cloudflare__AccountId");

            if (string.IsNullOrWhiteSpace(accountId))
                throw new InvalidOperationException("Cloudflare AccountId no configurado.");

            var publicBase = _config["Cloudflare:PublicBase"]; // opcional
            var bucket = _bucketName;

            // Nombre único: opcional subcarpeta por fecha
            var ext = Path.GetExtension(file.FileName);
            var key = $"{DateTime.UtcNow:yyyy/MM/dd}/{Guid.NewGuid():N}{ext}";
            var safeKeyForUrl = Uri.EscapeDataString(key);

            try
            {
                var s3Config = new AmazonS3Config
                {
                    ServiceURL = $"https://{accountId}.r2.cloudflarestorage.com",
                    ForcePathStyle = true,
                    RegionEndpoint = RegionEndpoint.USEast1
                };

                var creds = new BasicAWSCredentials(_accessKey, _secretKey);
                using var client = new AmazonS3Client(creds, s3Config);
                var transfer = new TransferUtility(client);

                using var stream = file.OpenReadStream();
                var request = new TransferUtilityUploadRequest
                {
                    BucketName = bucket,
                    InputStream = stream,
                    Key = key,
                    CannedACL = S3CannedACL.PublicRead,
                    ContentType = file.ContentType
                };

                await transfer.UploadAsync(request);

                // URL pública (usa PublicBase si está configurado)
                var url = !string.IsNullOrEmpty(publicBase)
                    ? $"{publicBase}/{safeKeyForUrl}"
                    : $"https://{bucket}.{accountId}.r2.cloudflarestorage.com/{safeKeyForUrl}";

                return url;
            }
            catch (AmazonS3Exception s3Ex)
            {
                // Rethrow with full detail
                var msg = $"R2 S3Error [{s3Ex.ErrorCode}]: {s3Ex.Message}";
                Console.WriteLine($"⛔ {msg}");
                throw new Exception(msg, s3Ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Unexpected Error: {ex.Message}");
                throw;
            }
        }


    }
}
