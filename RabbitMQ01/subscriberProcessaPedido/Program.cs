using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Net.Http;
using System.Text.Json;

namespace ProcessaPagamentoSubscriber
{
    class Subscriber
    {
        public class Mensagem
        {
            public string idPedido { get; set; }
            public string idPagamento { get; set; }
        }

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory()
            {
                Uri = new Uri("amqps://lbidfgrk:u51bF7jvAFUkgdo_JiezdaFaFsahUnYh@jaragua.lmq.cloudamqp.com/lbidfgrk")
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            using HttpClient client = new HttpClient();
            string apiUrl = "https://localhost:7069/api/pagamentos/";

            string queueName = "ProcessaPagamento_queue";
            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var mensagem = JsonSerializer.Deserialize<Mensagem>(message);
                apiUrl = apiUrl + mensagem.idPagamento + "/processar";

                using HttpClient client = new HttpClient();
                HttpContent content = new StringContent("true", Encoding.UTF8, "application/json");
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));

                HttpResponseMessage response = await client.PutAsync(apiUrl, content);
                string result = await response.Content.ReadAsStringAsync();


                Console.WriteLine($"Pagamento {mensagem.idPagamento} foi processado");
                
            };

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
            Console.WriteLine($"📢 Aguardando mensagens na fila '{queueName}'. Pressione [ENTER] para sair.");
            Console.ReadLine();
        }
    }
}