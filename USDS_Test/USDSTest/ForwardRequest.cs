using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;

namespace USDSTest
{       
    public class ForwardRequest
    {
        private readonly ILogger<ForwardRequest> _logger;
        private readonly HttpClient _httpClient = new HttpClient();

        public ForwardRequest(ILogger<ForwardRequest> logger)
        {
            _logger = logger;
        }

        [Function("ForwardRequest")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req, ILogger log)
        {   
            string responseMessage = string.Empty;
            string responseContent = string.Empty;
            string url = string.Empty;

            try
            {
                _logger.LogInformation("ForwardRequest...Starting");

                url = req.Form["hidUrl"];

                _httpClient.BaseAddress = new Uri(url);
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync(url);

                try
                {
                    response.EnsureSuccessStatusCode();
                }

                catch (Exception ex)
                {
                    responseMessage = $"There was an error processing the request. Exception.Message: {ex.Message} Exception.InnerException {ex.InnerException}\r\n";
                    _logger.LogError(responseMessage);
                    //return new OkObjectResult(responseMessage);
                    throw;
                }

                try
                {
                    responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("responseContent=" + responseContent);
                    responseMessage = "";
                }

                catch (Exception ex)
                {
                    responseMessage += responseMessage + $" There was an error reading the response. Exception.Message: {ex.Message} Exception.InnerException {ex.InnerException}\r\n";
                    _logger.LogError(responseMessage);
                    //return new OkObjectResult(responseMessage);
                    throw;
                }
            }

            catch (Exception ex)
            {
                responseMessage += responseMessage + $" There was an error reading the response. Exception.Message: {ex.Message} Exception.InnerException {ex.InnerException}\r\n";
                _logger.LogError(responseMessage);
            }

            finally
            {
                _logger.LogInformation("ForwardRequest...Ending");
            }

            return new OkObjectResult(responseMessage + "\r\n\r\n" + responseContent);
        }
    }
}
