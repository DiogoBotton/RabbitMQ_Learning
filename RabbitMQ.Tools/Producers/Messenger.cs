using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Tools.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Tools.Producers
{
    public class Messenger : IMessenger
    {
        private readonly string _hostname;
        private readonly string _password;
        private readonly string _username;
        private IConnection _connection;

        public Messenger(IOptions<RabbitOptions> rabbitOptions)
        {
            _hostname = rabbitOptions.Value.HostName;
            _password = rabbitOptions.Value.Password;
            _username = rabbitOptions.Value.UserName;

            CreateConnection();
        }

        public Messenger()
        {
            _hostname = "localhost";
            _password = "guest";
            _username = "guest";

            CreateConnection();
        }

        // Este método recebe qualquer objeto, desde que seja uma classe
        public void SendMessage<T>(T data, string queueName) where T : class
        {
            try
            {
                if (ConnectionExists())
                {
                    using(var channel = _connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: queueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                        string message = JsonConvert.SerializeObject(data);
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: null,
                                     body: body);

                        Console.WriteLine(" [x] Sent {0}", message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Houve um erro ao enviar mensagem à fila {queueName}. {ex.Message}");
            }
        }

        public void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = _hostname,
                    Password = _password,
                    UserName = _username
                };

                _connection = factory.CreateConnection();
            }
            catch (Exception ex)
            {
                throw new Exception($"Não foi possível estabelecer a conexão do RabbitMQ. {ex.Message}");
            }
        }

        private bool ConnectionExists()
        {
            if (_connection != null)
            {
                return true;
            }

            CreateConnection();

            return _connection != null;
        }
    }
}
