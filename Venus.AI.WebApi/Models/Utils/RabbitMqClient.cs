using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venus.AI.WebApi.Models.Utils
{
    internal class RabbitMqClient : IDisposable
    {
        ConnectionFactory _factory;
        IConnection _connection;

        public RabbitMqClient(string hostName)
        {
            _factory = new ConnectionFactory()
            {
                HostName = hostName
            };
            _connection = _factory.CreateConnection();
        }
        public RabbitMqClient(string hostName, int port)
        {
            _factory = new ConnectionFactory()
            {
                HostName = hostName,
                Port = port
            };
            _connection = _factory.CreateConnection();
        }
        public RabbitMqClient(string hostName, string userName, string password)
        {
            _factory = new ConnectionFactory()
            {
                HostName = hostName,
                UserName = userName,
                Password = password
            };
            _connection = _factory.CreateConnection();
        }
        public RabbitMqClient(string hostName, int port, string userName, string password)
        {
            _factory = new ConnectionFactory()
            {
                HostName = hostName,
                Port = port,
                UserName = userName,
                Password = password
            };
            _connection = _factory.CreateConnection();
        }

        public void Put(string jsonMessage, string queue)
        {
            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(queue: queue,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                var body = Encoding.UTF8.GetBytes(jsonMessage);

                channel.BasicPublish(exchange: "",
                                     routingKey: queue,
                                     basicProperties: null,
                                     body: body);
            }
        }

        //TODO Think about timeout!
        public async Task<string> PostAsync(string jsonMessage, string inputQueue, string outputQueue)
        {
            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(queue: inputQueue,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                var body = Encoding.UTF8.GetBytes(jsonMessage);

                channel.BasicPublish(exchange: "",
                                     routingKey: inputQueue,
                                     basicProperties: null,
                                     body: body);
            }
            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(queue: outputQueue,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var tcs = new TaskCompletionSource<string>();
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var result = Encoding.UTF8.GetString(body);
                    tcs.SetResult(result);
                };
                channel.BasicConsume(queue: outputQueue,
                                     autoAck: true,
                                     consumer: consumer);
                
                return await tcs.Task;
            }
        }

        public async Task<string> GetAsync(string queue)
        {
            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(queue: queue,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var tcs = new TaskCompletionSource<string>();
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var result = Encoding.UTF8.GetString(body);
                    tcs.SetResult(result);
                };
                channel.BasicConsume(queue: queue,
                                     autoAck: true,
                                     consumer: consumer);
                return await tcs.Task;
            }
        }

        public void Dispose()
        {
            _factory = null;
            _connection.Close();
            _connection.Dispose();
        }
    }
}