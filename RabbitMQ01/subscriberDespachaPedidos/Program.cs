using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;

namespace DespachaPedidoSubscriber
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
            string apiUrl = "https://localhost:7069/api/Pedido/";

            string queueName = "DespachaPedido_queue";
            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var mensagem = JsonSerializer.Deserialize<Mensagem>(message);

                apiUrl = apiUrl + mensagem.idPedido;




                using HttpClient client = new HttpClient();
                HttpContent content = new StringContent("true", Encoding.UTF8, "application/json");
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));


                HttpResponseMessage response = await client.PutAsync(apiUrl, content);

                // Simulando despacho de pedido
                
                Console.WriteLine("✅ Pedido despachado com sucesso!");
            };

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
            Console.WriteLine($"📢 Aguardando mensagens na fila '{queueName}'. Pressione [ENTER] para sair.");
            Console.ReadLine();
        }
    }
}