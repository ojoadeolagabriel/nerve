using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nerve.core.synapse.componentbase.impl;
using nerve.core.synapse.dataobjects;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace nerve.core.synapse.component.rabbitmq
{
    public class RabbitMqConsumer : PollingConsumer
    {
        private readonly RabbitMqProcessor _rabbitMqProcessor;

        public override Exchange Poll()
        {
            Task.Factory.StartNew(PollingMessageConsumer);
            return null;
        }

        private void PollingMessageConsumer()
        {
            var hostname = _rabbitMqProcessor.UriInformation.GetUriProperty("hostname", "localhost");
            var queue = _rabbitMqProcessor.UriInformation.GetUriProperty("queue", "systemQueue");
            var port = _rabbitMqProcessor.UriInformation.GetUriProperty("port", 5672);

            var factory = new ConnectionFactory() { HostName = hostname };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queue,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                    var exchange = new Exchange(_rabbitMqProcessor.Route);
                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);

                        exchange.InMessage.SetHeader("queue", queue);
                        exchange.InMessage.Body = message;
                        ProcessResponse(exchange);
                    };

                    channel.BasicConsume(queue: queue,
                                         noAck: true,
                                         consumer: consumer);

                    Console.ReadLine();
                }
            }
        }

        private void ProcessResponse(Exchange exchange)
        {
            _rabbitMqProcessor.Process(exchange);
        }

        public RabbitMqConsumer(RabbitMqProcessor processor)
        {
            _rabbitMqProcessor = processor;
        }
    }
}
