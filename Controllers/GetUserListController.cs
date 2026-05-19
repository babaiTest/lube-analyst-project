using Microsoft.AspNetCore.Mvc;
using Azure.Identity;
using Azure.Core;
using System.Net.Http.Headers;

namespace LubeAnalyst.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetUserListController : ControllerBase
    {
           
        public GetUserListController()
        {
           
        }
        [HttpGet("UserList")]
        public async Task<IActionResult> GetUsersList()
        {            
            
            try
            {
                var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {
                    ExcludeVisualStudioCredential = true,
                    ExcludeVisualStudioCodeCredential = true,
                    ExcludeManagedIdentityCredential = true
                });

                var token = await credential.GetTokenAsync(
                    new TokenRequestContext(
                        new[] { "api://d71ea290-9334-499e-a661-23d9e04854bb/.default" }
                    )
                );
                //return Ok($"Token acquired: {token.Token.Substring(0, 20)}...");
                // 🌐 Call APIM
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token.Token);

                var apimUrl =
                    "https://lubeanalyst-apim.azure-api.net/ProcessLabTestFunction-LubeAnalyst/GetUserList";

                var response = await client.GetAsync(apimUrl);
                var content = await response.Content.ReadAsStringAsync();

                return Ok(new
                {
                    StatusCode = response.StatusCode,
                    Response = content
                });


            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error saving order: {ex.Message}");
            }
        }
    }
}
