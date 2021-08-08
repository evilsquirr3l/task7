using System.Threading.Tasks;
using Amazon.SQS.Model;

namespace xCloud.Task7.Interfaces
{
    public interface ISqsService
    {
        Task<SendMessageResponse> PublishEventToSqsQueue(string message);

        Task SendSqsMessagesToSnsTopic();
    }
}