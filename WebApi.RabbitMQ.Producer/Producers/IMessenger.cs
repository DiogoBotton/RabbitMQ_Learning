using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.RabbitMQ.Producer.Producers
{
    public interface IMessenger
    {
        // Data deve ser uma classe
        void SendMessage<T>(T data, string queueName) where T : class;
    }
}
