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
        private readonly IConfiguration _config;


        public CloudflareR2Service(IConfiguration config)
        {
            _config     = config;
            _accessKey  = config["Cloudflare:R2AccessKey"];
            _secretKey  = config["Cloudflare:R2SecretKey"];
            _bucketName = config["Cloudflare:R2BucketName"];
            // si vas a leer AccountId más abajo, también lo hará _config["Cloudflare:AccountId"]
            _region     = "auto";
        }

        public async Task<string> UploadDocumentToCloudflareR2(IFormFile file)
        {
            try
            {
                var accountId = _config["Cloudflare:AccountId"];
                var s3Config = new AmazonS3Config {
                    ServiceURL     = $"https://{accountId}.r2.cloudflarestorage.com",
                    ForcePathStyle = true
                };
                var credentials = new Amazon.Runtime.BasicAWSCredentials(_accessKey, _secretKey);
                var s3Client    = new AmazonS3Client(credentials, s3Config);
                var fileTransferUtility = new TransferUtility(s3Client);

                string fileName = Path.GetFileName(file.FileName);
                using (var stream = file.OpenReadStream())
                {
                    var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                    {
                        BucketName = _bucketName,
                        InputStream = stream,  // Aquí estamos usando el flujo de entrada del archivo
                        Key = fileName,
                        CannedACL = S3CannedACL.PublicRead 
                    };
                    Console.WriteLine($"Uploading file {fileName} to Cloudflare R2...");

                    // Agrega depuración antes de realizar la carga
                    var uploadTask = fileTransferUtility.UploadAsync(fileTransferUtilityRequest);

                    // Espera a que la carga termine
                    await uploadTask;

                    // Si todo ha ido bien, imprime un mensaje de éxito
                    Console.WriteLine($"File {fileName} uploaded successfully to R2.");
                }


                // Generar la URL completa del archivo subido
                string fileUrl = $"https://{_bucketName}.r2.cloudflarestorage.com/{fileName}";

                // Verifica la URL generada
                Console.WriteLine($"File URL: {fileUrl}");

                return fileUrl;
            }
            catch (AmazonS3Exception s3Ex)
            {
                // logging mínimo
                Console.WriteLine($"⛔ S3 Error Code: {s3Ex.ErrorCode}");
                Console.WriteLine($"⛔ S3 HTTP Status: {s3Ex.StatusCode}");
                Console.WriteLine($"⛔ Message       : {s3Ex.Message}");
                Console.WriteLine($"⛔ StackTrace    : {s3Ex.StackTrace}");
                // opcional: si quisieras usar ILogger en lugar de Console:
                // _logger.LogError(s3Ex, "Error subiendo a R2: {Code}", s3Ex.ErrorCode);
                return null;
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error al subir el archivo a Cloudflare R2: {ex.Message}");
                Console.WriteLine($"Error al subir el archivo a Cloudflare R2: {ex.Message}");
                Console.WriteLine($"Detalles adicionales: {ex.StackTrace}");
                return null;
            }
        }

    }
}
