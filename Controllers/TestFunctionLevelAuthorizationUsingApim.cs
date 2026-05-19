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
    public class TestFunctionLevelAuthorizationUsingApim : ControllerBase
    {
       
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ApimService _apimService;
        public TestFunctionLevelAuthorizationUsingApim(AddCustomerHandler handler, ServiceBusClient serviceBusClient, 
                                                       IConfiguration configuration, ApimService apimService)
        {
          
            _configuration = configuration;
            string apimKey = _configuration["ApimSubscriptionKey"];

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apimKey);
            _apimService = apimService;
        }
        [HttpGet("GetCustomer")]
        public async Task<IActionResult> GetCustomer()
        {
            //Test apim call
            string url = "https://lubeanalyst-apim.azure-api.net/ProcessLabTestFunction-LubeAnalyst/GetUserList";

            var response = await _httpClient.GetAsync(url);
            string result1 = await response.Content.ReadAsStringAsync();

           
            return Ok(result1);
        }
        [HttpGet("GetCustomerUsingADAuthentication")]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _apimService.GetUserListAsync();
            return Ok(result);
        }
    }
}
