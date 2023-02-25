using System.Net;
using System.Text;
using System.Text.Json;
using LineBff.BusinessLogic;
using LineBff.RequestDTO;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace LineBff
{
    public class LineController
    {
        private readonly ILineService _service;

        public LineController(ILineService service)
        {
            _service = service;
        }

        [Function("generate-authurl")]
        public HttpResponseData RunGenerateAuthURL([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            var dto = _service.GenerateAuthURL();
            var json = JsonSerializer.Serialize(dto);
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            response.Body = new MemoryStream(Encoding.UTF8.GetBytes(json));
            return response;
        }

        [Function("generate-accesstoken")]
        public async Task<HttpResponseData> RunGenerateAccesstoken([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //TODO: バリデーション
            var request = JsonSerializer.Deserialize<GenerateAccesstokenRequest>(requestBody) ?? throw new ArgumentNullException();
            var dto = await _service.GenerateAccesstoken(request);
            var json = JsonSerializer.Serialize(dto);
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            response.Body = new MemoryStream(Encoding.UTF8.GetBytes(json));
            return response;
        }
    }
}

