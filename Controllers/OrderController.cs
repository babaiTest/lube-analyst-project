using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using LubeAnalyst.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LubeAnalyst.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly LabDbContext _context;
        private readonly ServiceBusClient _serviceBusClient;        
        public OrderController(LabDbContext context, ServiceBusClient serviceBusClient, BlobContainerClient blobContainer)
        {
            _context = context;
            _serviceBusClient = serviceBusClient;
        }
        [HttpPost("AddOrder")]
        public async Task<IActionResult> AddOrder([FromBody] TestRequest order)
        {            
            if (order == null)
            {
                return BadRequest("Order cannot be null.");
            }
            _context.TestRequests.Add(order);
            try
            {
                _context.SaveChanges();
                //Push message to Service Bus
                var sender = _serviceBusClient.CreateSender("lab-test-queue");
                var messagePayload = new
                {
                    TestRequestID = order.TestRequestID,
                    CustomerID = order.CustomerID,
                    TestType = order.TestType,
                    Status = order.Status
                };
                var message = new ServiceBusMessage(System.Text.Json.JsonSerializer.Serialize(messagePayload))
                {
                    ContentType = "application/json"
                };
                await sender.SendMessageAsync(message);
                //return CreatedAtAction(nameof(GetTestRequest), new { id = testRequest.TestRequestID }, testRequest);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error saving order: {ex.Message}");
            }
            return Ok("Order added successfully.");
        }
    }
}
