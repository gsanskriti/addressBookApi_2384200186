using System.Threading.Tasks;

namespace RepositoryLayer.Interface
{
    public interface IRabbitMQService
    {
        void PublishMessage(string queueName, string message);
    }
}
