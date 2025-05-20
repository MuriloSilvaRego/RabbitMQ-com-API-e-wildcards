using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;


namespace RabbitSubscriber
{
    class Subscriber
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory()
            {
                Uri = new Uri("amqps://lbidfgrk:u51bF7jvAFUkgdo_JiezdaFaFsahUnYh@jaragua.lmq.cloudamqp.com/lbidfgrk")
            };
            using HttpClient client = new HttpClient();
            string apiUrl = "https://localhost:7069/api/Pedido";


            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            string queueName = "AddPedido_queue";
            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                HttpContent content = new StringContent(message, Encoding.UTF8, "application/json");

                // Set headers
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));

                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                string result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response: {result}");



                Console.WriteLine("Mensagem recebida: " + message);
            };
            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
            Console.WriteLine("Aguardando mensagens. Pressione [enter] para sair.");
            Console.ReadLine();
        }
    }
}