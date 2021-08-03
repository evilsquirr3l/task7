using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using xCloud.Task7.Interfaces;

namespace xCloud.Task7.Controllers
{
    public class SubscriptionController : Controller
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpGet]
        public IActionResult SubscribeForNotifications()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> SubscribeForNotifications(string email)
        {
            var subscribeResponse = await _subscriptionService.SubscribeForNotifications(email);

            if (subscribeResponse is null)
            {
                return NotFound();
            }
            
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
            var unsubscribeResponse = await _subscriptionService.UnsubscribeFromTopic(email);

            if (unsubscribeResponse is null)
            {
                return NotFound();
            }
            
            return View(unsubscribeResponse.ResponseMetadata);
        }
    }
}