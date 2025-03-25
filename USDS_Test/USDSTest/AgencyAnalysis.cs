using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text.Json;

namespace USDSTest
{
    public class AgencyAnalysis
    {
        private static string agencyURL = "https://www.ecfr.gov/api/admin/v1/agencies.json";
        private static string agencyCountURL = "https://www.ecfr.gov/api/search/v1/count";
        private static string agencyCountByDateURL = "https://www.ecfr.gov/api/search/v1/counts/daily";
        private static string agencyCountByTitleURL = "https://www.ecfr.gov/api/search/v1/counts/titles";
        /*
        curl -X GET "https://www.ecfr.gov/api/admin/v1/agencies.json" -H "accept: application/json"

        curl -X GET "https://www.ecfr.gov/api/search/v1/count?agency_slugs%5B%5D=advisory-council-on-historic-preservation" -H "accept: application/json"

        curl -X GET "https://www.ecfr.gov/api/search/v1/counts/daily?agency_slugs%5B%5D=agriculture-department" -H "accept: application/json"

        curl -X GET "https://www.ecfr.gov/api/search/v1/counts/titles?agency_slugs%5B%5D=agriculture-department" -H "accept: application/json"
                 
        */

        private readonly ILogger<AgencyAnalysis> _logger;
        private readonly HttpClient _httpClient = new HttpClient();

        public AgencyAnalysis(ILogger<AgencyAnalysis> logger)
        {
            _logger = logger;
        }

        [Function("AgencyAnalysis")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)        
        {
            string responseMessage = string.Empty;
            string responseContent = string.Empty;            

            try
            {
                _logger.LogInformation("AgencyAnalysis...Starting");
                

                _httpClient.BaseAddress = new Uri(agencyURL);
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));                

                var response = await _httpClient.GetAsync(agencyURL);

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

                    //Load responseContent into a collection of objects
                    if (!string.IsNullOrWhiteSpace(responseContent))
                    {
                       Agencies? agencies = JsonSerializer.Deserialize<Agencies>(responseContent);

                        if((agencies != null))
                        {
                            //Get the change count for each agency

                            foreach (Agency agency in agencies.agencies)
                            {
                                await GetAgencyChangeCount(agency);
                                await GetAgencyChangeCountByDate(agency);
                                await GetAgencyChangeCountByTitle(agency);

                            }

                            responseContent = JsonSerializer.Serialize(agencies);
                        }
                        
                    }

                    else
                    {
                        responseMessage = $"There was an error processing the request. responseContent is null or whitespace";
                        _logger.LogError(responseMessage);
                        //return new OkObjectResult(responseMessage);
                        throw new Exception(responseMessage);
                    }




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
                _logger.LogInformation("AgencyAnalysis...Ending");
            }

            return new OkObjectResult(responseMessage + "\r\n\r\n" + responseContent);
        }

        private async Task<IActionResult> GetAgencyChangeCount(Agency agency)
        {
            string responseMessage = string.Empty;
            string responseContent = string.Empty;            
            HttpClient httpClient = new HttpClient();

            try
            {
                _logger.LogInformation("GetAgencyChangeCount...Starting");

                httpClient.BaseAddress = new Uri(agencyCountURL);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //QueryString queryString = new QueryString();
                //queryString.Add("agency_slugs", agency.slug);

                string queryString = "?agency_slugs%5B%5D=" + agency.slug;

                var response = await httpClient.GetAsync(agencyCountURL + queryString);

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

                    AgencyChangeCounts? agencyChangeCounts = JsonSerializer.Deserialize<AgencyChangeCounts>(responseContent);
                    if ((agencyChangeCounts != null)  && (agencyChangeCounts.meta != null))
                    {
                        agency.total_count = agencyChangeCounts.meta.total_count.ToString();
                        agency.description = agencyChangeCounts.meta.description;
                    }

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
                httpClient.Dispose();
                httpClient = null;

                _logger.LogInformation("GetAgencyChangeCount...Ending");
            }

            return new OkObjectResult(responseMessage + "\r\n\r\n" + responseContent);
        }

        private async Task<IActionResult> GetAgencyChangeCountByDate(Agency agency)
        {
            string responseMessage = string.Empty;
            string responseContent = string.Empty;                        
            HttpClient httpClient = new HttpClient();
            KeyValuePairsByDate? dataValues = new KeyValuePairsByDate();

            try
            {
                _logger.LogInformation("GetAgencyChangeCountByDate...Starting");

                httpClient.BaseAddress = new Uri(agencyCountByDateURL);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //QueryString queryString = new QueryString();
                //queryString.Add("agency_slugs", agency.slug);

                string queryString = "?agency_slugs%5B%5D=" + agency.slug;

                var response = await httpClient.GetAsync(agencyCountByDateURL + queryString);

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

                    AgencyChangeCountByDate temp = new AgencyChangeCountByDate();

                    var tempObj = JsonSerializer.Deserialize<object>(responseContent);

                    string temp2 = responseContent.Replace("{\"dates\":{", "");
                    temp2 = temp2.Replace("}}", "");
                    var temp3 = temp2.Split(",");
                    string[] temp4;
                    agency.agencyChangeCountsByDate = new AgencyChangeCountsByDate();
                    AgencyChangeCountByDate agencyChangeCountByDate = new AgencyChangeCountByDate();

                    foreach (string x in temp3)
                    {
                        temp4 = x.Split(":");
                        if(temp4.Length > 1)
                        {
                            agencyChangeCountByDate = new AgencyChangeCountByDate();
                            agencyChangeCountByDate.dateValue = temp4[0];
                            agencyChangeCountByDate.count = Convert.ToInt32(temp4[1]);

                            agency.agencyChangeCountsByDate.dates.Add(agencyChangeCountByDate);
                        }

                        else
                        {
                            responseMessage = "test";
                        }

                    }
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
                httpClient.Dispose();
                httpClient = null;

                _logger.LogInformation("GetAgencyChangeCountByDate...Ending");
            }

            return new OkObjectResult(responseMessage + "\r\n\r\n" + responseContent);
        }

        private async Task<IActionResult> GetAgencyChangeCountByTitle(Agency agency)
        {
            string responseMessage = string.Empty;
            string responseContent = string.Empty;
            HttpClient httpClient = new HttpClient();
            KeyValuePairsByDate? dataValues = new KeyValuePairsByDate();

            try
            {
                _logger.LogInformation("GetAgencyChangeCountByDate...Starting");

                httpClient.BaseAddress = new Uri(agencyCountByTitleURL);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //QueryString queryString = new QueryString();
                //queryString.Add("agency_slugs", agency.slug);

                string queryString = "?agency_slugs%5B%5D=" + agency.slug;

                var response = await httpClient.GetAsync(agencyCountByTitleURL + queryString);

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

                    AgencyChangeCountByDate temp = new AgencyChangeCountByDate();

                    var tempObj = JsonSerializer.Deserialize<object>(responseContent);

                    string temp2 = responseContent.Replace("{\"titles\":{", "");
                    temp2 = temp2.Replace("}}", "");
                    var temp3 = temp2.Split(",");
                    string[] temp4;
                    agency.agencyChangeCountsByTitle = new AgencyChangeCountsByTitle();
                    AgencyChangeCountByTitle agencyChangeCountByTitle = new AgencyChangeCountByTitle();

                    foreach (string x in temp3)
                    {
                        temp4 = x.Split(":");
                        if (temp4.Length > 1)
                        {
                            agencyChangeCountByTitle = new AgencyChangeCountByTitle();
                            agencyChangeCountByTitle.titleValue = temp4[0];
                            agencyChangeCountByTitle.count = Convert.ToInt32(temp4[1]);

                            agency.agencyChangeCountsByTitle.titles.Add(agencyChangeCountByTitle);
                        }

                        else
                        {
                            responseMessage = "test";
                        }

                    }
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
                httpClient.Dispose();
                httpClient = null;

                _logger.LogInformation("GetAgencyChangeCountByDate...Ending");
            }

            return new OkObjectResult(responseMessage + "\r\n\r\n" + responseContent);
        }

    }
}
