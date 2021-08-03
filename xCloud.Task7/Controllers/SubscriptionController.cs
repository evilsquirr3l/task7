using System.Linq;
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
        
        [HttpGet]
        public IActionResult UnsubscribeFromTopic()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> UnsubscribeFromTopic(string email)
        {
            using var snsClient = _awsService.GetSnsAccessClient();

            var subscribers = await snsClient.ListSubscriptionsByTopicAsync(_awsService.GetSnsTopicArn());

            var subscription = subscribers.Subscriptions.FirstOrDefault(x => x.Endpoint == email);

            if (subscription is null)
            {
                return NotFound();
            }
            
            var unsubscribeRequest = new UnsubscribeRequest(subscription.SubscriptionArn);
            var unsubscribeResponse = await snsClient.UnsubscribeAsync(unsubscribeRequest);

            return View(unsubscribeResponse.ResponseMetadata);
        }
    }
}