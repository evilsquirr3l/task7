using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using xCloud.Task7.Interfaces;

namespace xCloud.Task7.Controllers
{
    public class LambdaController : Controller
    {
        private IAwsService _awsService;

        public LambdaController(IAwsService awsService)
        {
            _awsService = awsService;
        }

        [HttpGet]
        public IActionResult TriggerLambda()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> TriggerLambda(string payload)
        {
            await _awsService.InvokeLambda(payload);

            return View();
        }
    }
}