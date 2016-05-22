using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        public static ConcurrentDictionary<string, IModel> Channel = new ConcurrentDictionary<string, IModel>();

        public IModel GetChannel(string channelId, string queue, string host)
        {
            if (!Channel.Keys.Contains(channelId))
            {
                var factory = new ConnectionFactory() { HostName = host };
                var connection = factory.CreateConnection();
                var channel = connection.CreateModel();
                Channel.TryAdd(channelId, channel);
            }

            return Channel[channelId];
        }

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
            var channel = GetChannel(string.Format("{0}::{1}::{2}", hostname, port, queue), queue, hostname);

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
