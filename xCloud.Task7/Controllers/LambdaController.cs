using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace xCloud.Task7.Controllers
{
    public class LambdaController : Controller
    {
        [HttpGet]
        public IActionResult TriggerLambda()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> TriggerLambda(string uri)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            await httpClient.GetAsync(uri);

            return Ok("Request was sent.");
        }
    }
}