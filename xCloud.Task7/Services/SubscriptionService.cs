using System.Linq;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService.Model;
using xCloud.Task7.Interfaces;

namespace xCloud.Task7.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IAwsService _awsService;

        public SubscriptionService(IAwsService awsService)
        {
            _awsService = awsService;
        }

        public async Task<UnsubscribeResponse> UnsubscribeFromTopic(string email)
        {
            using var snsClient = _awsService.GetSnsAccessClient();

            var subscribers = await snsClient.ListSubscriptionsByTopicAsync(_awsService.GetSnsTopicArn());

            var subscription = subscribers.Subscriptions.FirstOrDefault(x => x.Endpoint == email);

            if (subscription is null)
            {
                return null;
            }
            
            var unsubscribeRequest = new UnsubscribeRequest(subscription.SubscriptionArn);
            var unsubscribeResponse = await snsClient.UnsubscribeAsync(unsubscribeRequest);
            return unsubscribeResponse;
        }
        
        public async Task<SubscribeResponse> SubscribeForNotifications(string email)
        {
            using var snsClient = _awsService.GetSnsAccessClient();

            var subscribeRequest = new SubscribeRequest(_awsService.GetSnsTopicArn(), "email", email);
            var subscribeResponse = await snsClient.SubscribeAsync(subscribeRequest);
            return subscribeResponse;
        }
    }
}