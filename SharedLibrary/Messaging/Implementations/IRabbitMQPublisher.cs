using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Messaging.Implementations
{
    public interface IRabbitMQPublisher
    {
        Task PublishAsync(string queueName, object message);

    }
}
