using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace RabbitSubscriberPagamentos
{
    class Subscriber
    {
        static async Task Main(string[] args)
        {
            var factory = new ConnectionFactory()
            {
                Uri = new Uri("amqps://lbidfgrk:u51bF7jvAFUkgdo_JiezdaFaFsahUnYh@jaragua.lmq.cloudamqp.com/lbidfgrk")
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            string queueName = "AddPagamento_queue";
            string confirmQueue = "ConfirmPagamento_queue";
            string apiUrl = "https://localhost:7069/api/pagamentos";

            // Declaração das filas
            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: confirmQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);

            // Consumidor da fila de pagamentos
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Pagamento recebido: {message}");

                using HttpClient client = new HttpClient();

                HttpContent content = new StringContent(message, Encoding.UTF8, "application/json");
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                string idPagamento = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(idPagamento))
                {
                    // Processando o JSON retornado pela API e pegando o idPagamento
                    idPagamento = idPagamento.Trim('"');
                    Console.WriteLine(idPagamento);

                    var confirmMessage = $"{idPagamento}";
                    string confirmJson = JsonSerializer.Serialize(confirmMessage);

                    // Publicando confirmação na fila "ConfirmPagamento_queue"
                    channel.BasicPublish(exchange: "topic_exchange", routingKey: "confirm.pagamento", basicProperties: null, body: Encoding.UTF8.GetBytes(confirmJson));

                    Console.WriteLine($"Confirmação publicada com ID: {idPagamento}");
                }
            
                else
                {
                    Console.WriteLine($"Erro: resposta da API não contém um ID de pagamento válido");
                }
            };

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
            Console.WriteLine("Aguardando mensagens de pagamento...");
            Console.ReadLine();
        }
    }
}