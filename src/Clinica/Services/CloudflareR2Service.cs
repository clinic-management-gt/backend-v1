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
            _accessKey = config["Cloudflare:R2AccessKey"]!;
            _secretKey = config["Cloudflare:R2SecretKey"]!;
            _bucketName = config["Cloudflare:R2BucketName"]!;

            Console.WriteLine($"🔑 R2AccessKey   = {_accessKey ?? "(null)"}");
            Console.WriteLine($"🔒 R2SecretKey  = {(_secretKey?.Substring(0, 8) ?? "(null)")}…");
            Console.WriteLine($"📦 R2BucketName = {_bucketName ?? "(null)"}");
        }


        public async Task<string> UploadDocumentToCloudflareR2(IFormFile file)
        {
            var accountId = _config["Cloudflare:AccountId"];
            var bucket = _bucketName;
            var fileName = Path.GetFileName(file.FileName);
            var safeName = Uri.EscapeDataString(fileName);

            try
            {
                Console.WriteLine($"📂 Preparing to upload {fileName} ({file.Length} bytes)");

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
                    Key = fileName,
                    CannedACL = S3CannedACL.PublicRead
                };

                await transfer.UploadAsync(request);
                Console.WriteLine("✅ Upload to R2 succeeded!");

                var url = $"https://{bucket}.{accountId}.r2.cloudflarestorage.com/{safeName}";
                Console.WriteLine($"🌐 File URL: {url}");
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
