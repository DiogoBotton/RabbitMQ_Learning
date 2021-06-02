using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Tools.Contexts;
using RabbitMQ.Tools.Domains;
using RabbitMQ.Tools.Enums;
using RabbitMQ.Tools.Producers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebApi.RabbitMQ.Consumer.Receiver
{
    public class PersonReceiver : BackgroundService
    {
        private readonly string _hostname;
        private readonly string _password;
        private readonly string _username;
        private IConnection _connection;
        private IModel _channel;
        private Messenger _messenger;
        private Progress _currentProgress;

        public PersonReceiver(IOptions<RabbitOptions> rabbitOptions)
        {
            _hostname = rabbitOptions.Value.HostName;
            _password = rabbitOptions.Value.Password;
            _username = rabbitOptions.Value.UserName;
            _messenger = new Messenger();
            _currentProgress = new Progress();

            InitializerRabbitMQListener();
        }

        public void InitializerRabbitMQListener()
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
                _connection.ConnectionShutdown += _RabbitMQ_ConnectionShutdown;
                _channel = _connection.CreateModel();
                _channel.QueueDeclare(queue: QueuesDefaultValues.GetValue(EnumQueues.PessoasQueue), durable: false, exclusive: false, autoDelete: false, arguments: null);
            }
            catch (Exception ex)
            {
                throw new Exception($"Não foi possível estabelecer a conexão do RabbitMQ da parte do Listener/Consumer. {ex.Message}");
            }
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                // Reseta progress a cada leitura
                _currentProgress = new Progress();
                _messenger.SendMessage(_currentProgress, QueuesDefaultValues.GetValue(EnumQueues.ProgressQueue));

                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    // Desserializa a mensagem string em uma lista de pessoas
                    List<Pessoa> pessoas = JsonConvert.DeserializeObject<List<Pessoa>>(message);

                    foreach (var p in pessoas)
                    {
                        Console.WriteLine($"Nome: {p.Nome} | Idade: {p.Idade} | Genero: {p.Genero}.");
                    }

                    Console.WriteLine($"Número de pessoas: {pessoas.Count}");

                    _channel.BasicAck(ea.DeliveryTag, false);

                    _currentProgress.UpdateStatusProcessingComplete();
                    _messenger.SendMessage(_currentProgress, QueuesDefaultValues.GetValue(EnumQueues.ProgressQueue));
                }
                catch (Exception ex)
                {
                    _currentProgress.UpdateStatusError("Houve um erro no processamento do arquivo, o mesmo será recolocado na fila. ErrorMessage: " + ex.Message);
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(QueuesDefaultValues.GetValue(EnumQueues.PessoasQueue), false, consumer);

            return Task.CompletedTask;
        }
        private void _RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
        }
    }
}
