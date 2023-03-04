using System.Net;
using System.Text;
using LineBff.BusinessLogic;
using LineBff.RequestDTO;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;

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
            try
            {
                var dto = _service.GenerateAuthURL();
                var json = JsonConvert.SerializeObject(dto);
                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "application/json");
                response.Body = new MemoryStream(Encoding.UTF8.GetBytes(json));
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Function("generate-accesstoken")]
        public async Task<HttpResponseData> RunGenerateAccesstoken([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                //TODO: バリデーション
                var request = JsonConvert.DeserializeObject<GenerateAccesstokenRequest>(requestBody) ?? throw new ArgumentNullException();
                var dto = await _service.GenerateAccesstoken(request);
                var json = JsonConvert.SerializeObject(dto);
                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "application/json");
                response.Body = new MemoryStream(Encoding.UTF8.GetBytes(json));
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

