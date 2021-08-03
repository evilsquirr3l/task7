using System.Threading.Tasks;
using Amazon.SimpleNotificationService.Model;
using Microsoft.AspNetCore.Mvc;
using xCloud.Task7.Interfaces;

namespace xCloud.Task7.Controllers
{
    public class SubscriptionController : Controller
    {
        private readonly IAwsService _awsService;

        public SubscriptionController(IAwsService awsService)
        {
            _awsService = awsService;
        }

        [HttpGet]
        public IActionResult SubscribeForNotifications()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> SubscribeForNotifications(string email)
        {
            using var snsClient = _awsService.GetSnsAccessClient();
            
            var subscribeRequest = new SubscribeRequest(_awsService.GetSnsTopicArn(), "email", email);
            var subscribeResponse = await snsClient.SubscribeAsync(subscribeRequest);

            return View(subscribeResponse.ResponseMetadata);
        }
    }
}