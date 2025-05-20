using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;

namespace RabbitPublisherPedidos
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqps://lbidfgrk:u51bF7jvAFUkgdo_JiezdaFaFsahUnYh@jaragua.lmq.cloudamqp.com/lbidfgrk")
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            string exchangeName = "topic_exchange";
            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);

            string addPedido = "AddPedido_queue";
            channel.QueueDeclare(queue: addPedido, durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind(queue: addPedido, exchange: exchangeName, routingKey: "add.pedido");

            // Criando Pedido
            var AddPedido = new { nomeCliente = "testeFinal", despachadoStatus = false };

            //{ nomeCliente = "NomeDoCliente", despachadoStatus = false }   exemplo de json

            string addPedidoJson = JsonSerializer.Serialize(AddPedido);

            channel.BasicPublish(exchange: exchangeName, routingKey: "add.pedido", basicProperties: null, body: Encoding.UTF8.GetBytes(addPedidoJson));

            Console.WriteLine($"Pedido criado: {addPedidoJson}");
        }
    }
}