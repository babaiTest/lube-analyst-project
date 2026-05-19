using Azure.Messaging.ServiceBus;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using LubeAnalyst.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LubeAnalyst.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessTestReportController : ControllerBase
    {
        private readonly BlobContainerClient _blobContainer;
        private readonly string _connectionString;
        private readonly LabDbContext _context;
        private readonly ServiceBusClient _serviceBusClient;
        public ProcessTestReportController(BlobContainerClient blobContainer, IConfiguration configuration, LabDbContext context, ServiceBusClient serviceBusClient)
        {
            _blobContainer = blobContainer;
            _connectionString = configuration["AzureBlobStorage:ConnectionString"]
                                ?? throw new ArgumentNullException(nameof(configuration), "AzureBlobStorage connection string is missing.");
            _context = context;
            _serviceBusClient = serviceBusClient;
        }

        [HttpPost("UploadReport")]
        public async Task<IActionResult> UploadReport(IFormFile file, string testRequestId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            try
            {
                var blobName = testRequestId + "_" + file.FileName;
                var blobClient = _blobContainer.GetBlobClient(blobName);
                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, true);
                }
                // extract accountName + accountKey properly
                var (accountName, accountKey) = ParseStorageAccount(_connectionString);
               
                // build SAS token
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = "lubricant-test",
                    BlobName = blobName,
                    Resource = "b",
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(24) // expires in 24h
                };
                sasBuilder.SetPermissions(BlobSasPermissions.Read);

                var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(accountName, accountKey)).ToString();
                var fileUrl = $"{blobClient.Uri}?{sasToken}";

                _context.LabReports.Add(new LabReport
                {
                    TestRequestID = Convert.ToInt32(testRequestId),
                    ReportFilePath = fileUrl
                });
               
                // Update TestRequest status to Completed
                var testRequest = _context.TestRequests.Find(Convert.ToInt32(testRequestId));
                if (testRequest == null)
                {
                    return NotFound($"TestRequest with ID {testRequestId} not found.");
                }
                testRequest.Status = "Completed";
                _context.SaveChanges();

                // Push message to Service Bus
                var sender = _serviceBusClient.CreateSender("report-completed-queue");
                var messagePayload = new
                {
                    TestRequestID = testRequestId,
                    ReportFilePath = fileUrl
                };
                var message = new ServiceBusMessage(System.Text.Json.JsonSerializer.Serialize(messagePayload))
                {
                    ContentType = "application/json"
                };
                await sender.SendMessageAsync(message);

                return Ok("Report uploaded successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error uploading report: {ex.Message}");
            }
        }

        private (string accountName, string accountKey) ParseStorageAccount(string connectionString)
        {
            var dict = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(part =>
                {
                    var idx = part.IndexOf('=');
                    var key = part.Substring(0, idx);
                    var value = part.Substring(idx + 1);
                    return new { key, value };
                })
                .ToDictionary(sp => sp.key, sp => sp.value);

            return (dict["AccountName"], dict["AccountKey"]);
        }
    }
}
