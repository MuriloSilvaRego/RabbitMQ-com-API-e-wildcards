using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RabbitPublisherPagamentos
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqps://lbidfgrk:u51bF7jvAFUkgdo_JiezdaFaFsahUnYh@jaragua.lmq.cloudamqp.com/lbidfgrk")
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            string exchangeName = "topic_exchange";
            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);

            string addPagamento = "AddPagamento_queue";
            string confirmaPagamento = "ConfirmPagamento_queue";
            string processaPagamento = "ProcessaPagamento_queue";
            string despachaPedido = "DespachaPedido_queue";

            channel.QueueDeclare(queue: addPagamento, durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: confirmaPagamento, durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: processaPagamento, durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: despachaPedido, durable: false, exclusive: false, autoDelete: false, arguments: null);

            // 🔹 Aplicando wildcard corretamente no bind das filas
            channel.QueueBind(queue: addPagamento, exchange: exchangeName, routingKey: "add.pagamento");
            channel.QueueBind(queue: confirmaPagamento, exchange: exchangeName, routingKey: "confirm.pagamento");
            channel.QueueBind(queue: processaPagamento, exchange: exchangeName, routingKey: "confirma.*");
            channel.QueueBind(queue: despachaPedido, exchange: exchangeName, routingKey: "confirma.*");

            string PedidoId = "06c7dcb4-2870-4a0f-be11-21edee5e78a1";

            

            var AddPagamento = new { pedidoId = PedidoId, valor = 159.90, statusProcessado = false };



            string addPagamentoJson = JsonSerializer.Serialize(AddPagamento);
            //'{"pedidoId": "fc3f1523-91be-4343-86db-5c44d40a3224","valor": 20,"statusProcessado": false}'

            channel.BasicPublish(exchange: exchangeName, routingKey: "add.pagamento", basicProperties: null, body: Encoding.UTF8.GetBytes(addPagamentoJson));
            Console.WriteLine("Pagamento solicitado. Aguardando confirmação do subscriber...");

            // 🔹 Consumindo resposta do subscriber e obtendo idPagamento
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                message = message.Replace("\"", "");

                string[] IDs = message.Split(';');
                Console.WriteLine($"Pagamento confirmado com ID {IDs[0]} e ID de pedido {IDs[1]}. Publicando confirmações...");


                var ConfirmaPagamento = new { idPedido = $"{IDs[1]}", idPagamento = $"{IDs[0]}" };
                

                // 🔹 Publicando confirmações com routing keys individuais
                channel.BasicPublish(exchange: exchangeName, routingKey: "confirma.pagamento", basicProperties: null, body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(ConfirmaPagamento)));
                
                Console.WriteLine("Confirmações enviadas para todas as filas vinculadas a 'confirma.*'!");

            };

            channel.BasicConsume(queue: "ConfirmPagamento_queue", autoAck: true, consumer: consumer);
            Console.WriteLine("Esperando confirmação do pagamento...");
            Console.ReadLine();
            
        }
    }
}