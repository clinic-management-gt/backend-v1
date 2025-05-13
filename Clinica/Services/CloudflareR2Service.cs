using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Clinica.Services
{
    public class CloudflareR2Service
    {
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly string _bucketName;
        private readonly string _region;

        public CloudflareR2Service(IConfiguration config)
        {
            _accessKey = config["Cloudflare:R2AccessKey"];
            _secretKey = config["Cloudflare:R2SecretKey"];
            _bucketName = config["Cloudflare:R2BucketName"];
            _region = "auto"; // Si no tiene región específica, usa "auto"
        }

        public async Task<string> UploadDocumentToCloudflareR2(string filePath)
        {
            try
            {
                var s3Client = new AmazonS3Client(_accessKey, _secretKey, Amazon.RegionEndpoint.USEast1);
                var fileTransferUtility = new TransferUtility(s3Client);

                string fileName = Path.GetFileName(filePath);
                var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                    BucketName = _bucketName,
                    FilePath = filePath,
                    Key = fileName,
                    CannedACL = S3CannedACL.PublicRead 
                };

                await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);

                string fileUrl = $"https://{_bucketName}.r2.cloudflarestorage.com/{fileName}";
                return fileUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al subir el archivo a Cloudflare R2: {ex.Message}");
                return null;
            }
        }
    }
}
