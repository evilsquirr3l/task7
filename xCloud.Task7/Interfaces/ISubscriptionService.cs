using System.Threading.Tasks;
using Amazon.SimpleNotificationService.Model;

namespace xCloud.Task7.Interfaces
{
    public interface ISubscriptionService
    {
        Task<UnsubscribeResponse> UnsubscribeFromTopic(string email);
        Task<SubscribeResponse> SubscribeForNotifications(string email);
    }
}