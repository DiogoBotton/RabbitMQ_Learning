using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Consumer.Notification.Domains;
using System;
using System.Text;

namespace RabbitMQ.Consumer.Notification
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "progressQueue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);

                        // Desserializa a mensagem string em uma lista de pessoas
                        Progress p = JsonConvert.DeserializeObject<Progress>(message);

                        Console.WriteLine($"NomeArquivo: {p.NomeArquivo} | Status: {p.Status} | Mensagem: {p.Message}.");

                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        //Log (Nack para dizer que o consumo da msg não foi possível)
                        // DelivaryTag -> Carimbo de entrega
                        // 3° Parametro recoloca ou não a msg na fila novamente
                        channel.BasicNack(ea.DeliveryTag, false, true);
                    }
                };
                channel.BasicConsume(queue: "progressQueue",
                                     //AutoAck false para caso dê alguma excessão na leitura da msg, não confirme que mensagem foi lida automaticamente
                                     autoAck: false,
                                     consumer: consumer);

                Console.WriteLine(" Pressione [enter] para sair.");
                Console.ReadLine();
            }
        }
    }
}