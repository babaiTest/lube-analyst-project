using Azure.Messaging.ServiceBus;
using LubeAnalyst.Data;
using LubeAnalyst.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace LubeAnalyst.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRegistrationController : ControllerBase
    {
        private readonly AddCustomerHandler _handler;
        private readonly ServiceBusClient _serviceBusClient;
       
        //private readonly IConfiguration _configuration;
       
        public UserRegistrationController(AddCustomerHandler handler, ServiceBusClient serviceBusClient, IConfiguration configuration)
        {
            _handler = handler;
            _serviceBusClient = serviceBusClient;
            //_configuration = configuration;
            
        }
        [HttpPost("RegisterCustomer")]
        public async Task<IActionResult> RegisterCustomer(Customer customerObject)
        {
            var command =new AddCustomerCommand();
            command.Name = customerObject.Name;
            command.Email = customerObject.Email;
            command.Phone = customerObject.Phone;
            command.CompanyName = customerObject.CompanyName;
            command.CreatedAt = DateTime.Now;
            var result = await _handler.Handle(command);
            // Push message to Service Bus
            var sender = _serviceBusClient.CreateSender("userregistration-queue");
            await sender.SendMessageAsync(new ServiceBusMessage(JsonConvert.SerializeObject(customerObject)));
            return Ok("User registered successfully.");
        }
    }
}
