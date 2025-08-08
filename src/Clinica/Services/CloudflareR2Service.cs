using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Clinica.Services
{
    public class CloudflareR2Service
    {
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly string _bucketName;
        private readonly string _accountId;
        private readonly string _publicBase;
        private readonly HttpClient _httpClient;

        public CloudflareR2Service(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _accessKey = Environment.GetEnvironmentVariable("Cloudflare__R2AccessKey") ?? 
                         config["Cloudflare:R2AccessKey"] ?? "";
                         
            _secretKey = Environment.GetEnvironmentVariable("Cloudflare__R2SecretKey") ?? 
                         config["Cloudflare:R2SecretKey"] ?? "";
                         
            _bucketName = Environment.GetEnvironmentVariable("Cloudflare__R2BucketName") ?? 
                          config["Cloudflare:R2BucketName"] ?? "";
                          
            _accountId = Environment.GetEnvironmentVariable("Cloudflare__AccountId") ?? 
                         config["Cloudflare:AccountId"] ?? "";
                         
            _publicBase = Environment.GetEnvironmentVariable("Cloudflare__PublicBase") ?? 
                          config["Cloudflare:PublicBase"] ?? "";

            Console.WriteLine($"[R2] AccountId: {_accountId}");
            Console.WriteLine($"[R2] AccessKey: {(_accessKey.Length > 3 ? _accessKey[..4] + "***" : "MISSING")}");
            Console.WriteLine($"[R2] SecretKey: {(_secretKey.Length > 3 ? _secretKey[..4] + "***" : "MISSING")}");
            Console.WriteLine($"[R2] BucketName: {_bucketName}");
            
            _httpClient = httpClientFactory.CreateClient("R2Client");
        }

        public async Task<string> UploadDocumentToCloudflareR2(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Archivo vacío", nameof(file));

            var ext = Path.GetExtension(file.FileName);
            var key = $"{DateTime.UtcNow:yyyy/MM/dd}/{Guid.NewGuid():N}{ext}";
            var safeKeyForUrl = Uri.EscapeDataString(key);

            try
            {
                Console.WriteLine($"[R2] Iniciando carga directa HTTP: {file.FileName} ({file.Length} bytes)");
                
                // Cargar archivo a memoria
                byte[] fileBytes;
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    fileBytes = ms.ToArray();
                }

                // Crear solicitud HTTP directa en lugar de usar AWS SDK
                var endpoint = $"https://{_accountId}.r2.cloudflarestorage.com/{_bucketName}/{key}";
                var request = new HttpRequestMessage(HttpMethod.Put, endpoint);
                
                // Configurar headers básicos
                request.Content = new ByteArrayContent(fileBytes);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                request.Content.Headers.ContentLength = fileBytes.Length;
                
                // Agregar fecha para la firma
                var timestamp = DateTime.UtcNow;
                var dateString = timestamp.ToString("yyyyMMddTHHmmssZ");
                var shortDate = timestamp.ToString("yyyyMMdd");
                
                // Agregar encabezados de autenticación AWS4
                request.Headers.Add("x-amz-date", dateString);
                request.Headers.Add("x-amz-content-sha256", "UNSIGNED-PAYLOAD");
                
                // Crear firma AWS v4 simplificada
                var region = "auto";
                var service = "s3";
                
                // Scope para la firma
                var scope = $"{shortDate}/{region}/{service}/aws4_request";
                
                // Crear firma para la solicitud
                var canonicalRequest = $"PUT\n/{_bucketName}/{key}\n\nhost:{_accountId}.r2.cloudflarestorage.com\nx-amz-content-sha256:UNSIGNED-PAYLOAD\nx-amz-date:{dateString}\n\nhost;x-amz-content-sha256;x-amz-date\nUNSIGNED-PAYLOAD";
                var stringToSign = $"AWS4-HMAC-SHA256\n{dateString}\n{scope}\n{HexHash(canonicalRequest)}";
                
                // Derivar claves
                var kSecret = Encoding.UTF8.GetBytes($"AWS4{_secretKey}");
                var kDate = HmacSha256(kSecret, shortDate);
                var kRegion = HmacSha256(kDate, region);
                var kService = HmacSha256(kRegion, service);
                var kSigning = HmacSha256(kService, "aws4_request");
                
                // Calcular firma
                var signature = HexEncode(HmacSha256(kSigning, stringToSign));
                
                // Añadir header de autorización
                var authHeader = $"AWS4-HMAC-SHA256 Credential={_accessKey}/{scope}, SignedHeaders=host;x-amz-content-sha256;x-amz-date, Signature={signature}";
                request.Headers.TryAddWithoutValidation("Authorization", authHeader);
                
                // Hacer la solicitud PUT
                Console.WriteLine("[R2] Enviando solicitud PUT directa...");
                var response = await _httpClient.SendAsync(request);
                
                // Verificar respuesta
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[R2] Archivo subido con éxito. Status: {response.StatusCode}");
                    
                    // Construir URL pública corregida
                    var url = !string.IsNullOrEmpty(_publicBase)
                        ? $"{_publicBase.TrimEnd('/')}/{safeKeyForUrl}"
                        : $"https://{_accountId}.r2.cloudflarestorage.com/{_bucketName}/{safeKeyForUrl}";
                    
                    return url;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[R2] Error al subir: {response.StatusCode}. {errorContent}");
                    throw new Exception($"Error al subir archivo a R2: {response.StatusCode}. {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[R2] Error fatal: {ex.Message}");
                throw new Exception($"Error al subir archivo a R2: {ex.Message}", ex);
            }
        }
        
        #region Métodos auxiliares para firma AWS v4
        private static string HexHash(string data)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
            return HexEncode(bytes);
        }
        
        private static byte[] HmacSha256(byte[] key, string data)
        {
            using var hmac = new HMACSHA256(key);
            return hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        }
        
        private static string HexEncode(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
        #endregion
    }
}
