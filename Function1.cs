using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BasicHTTPTrigger
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("Function1")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("HTTP trigger function processed a request.");

            // Try to get 'name' from query string
            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            string name = query["name"];

            // Optional: Try get 'name' from POST body (JSON)
            if (string.IsNullOrEmpty(name) && req.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                var body = await req.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(body))
                {
                    try
                    {
                        var data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(body);
                        if (data != null && data.ContainsKey("name"))
                        {
                            name = data["name"];
                        }
                    }
                    catch
                    {
                        // Ignore bad JSON
                    }
                }
            }

            name ??= "Ariana";

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync($"Hello, {name}! Your Azure Function executed successfully.");
            return response;
        }
    }
}