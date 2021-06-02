using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Consumer.Domains;
using RabbitMQ.Consumer.Producers;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Progress currentProgress = new Progress();

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "pessoasQueue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        // Reseta progresso quando é enviado novos objetos na fila
                        currentProgress = new Progress();
                        // Atualização de Status
                        Messenger.SendProgressQeue(currentProgress);

                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);

                        // Desserializa a mensagem string em uma lista de pessoas
                        List<Pessoa> pessoas = JsonConvert.DeserializeObject<List<Pessoa>>(message);

                        foreach (var p in pessoas)
                        {
                            Console.WriteLine($"Nome: {p.Nome} | Idade: {p.Idade} | Genero: {p.Genero}.");
                        }

                        Console.WriteLine($"Número de pessoas: {pessoas.Count}");

                        channel.BasicAck(ea.DeliveryTag, false);

                        // Atualização de Status completo
                        currentProgress.UpdateStatusComplete();
                        Messenger.SendProgressQeue(currentProgress);
                    }
                    catch (Exception ex)
                    {
                        // Atualização de Status de erro
                        currentProgress.UpdateStatusError("Houve um erro no processamento do arquivo, o mesmo será recolocado na fila. ErrorMessage: " + ex.Message);
                        Messenger.SendProgressQeue(currentProgress);

                        //Log (Nack para dizer que o consumo da msg não foi possível)
                        // DelivaryTag -> Carimbo de entrega
                        // 3° Parametro recoloca ou não a msg na fila novamente
                        channel.BasicNack(ea.DeliveryTag, false, true);
                    }
                };
                channel.BasicConsume(queue: "pessoasQueue",
                                     //AutoAck false para caso dê alguma excessão na leitura da msg, não confirme que mensagem foi lida automaticamente
                                     autoAck: false,
                                     consumer: consumer);


                Console.WriteLine(" Pressione [enter] para sair.");
                Console.ReadLine();
            }
        }
    }
}