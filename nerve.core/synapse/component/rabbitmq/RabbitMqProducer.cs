using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nerve.core.synapse.componentbase;
using nerve.core.synapse.context;
using nerve.core.synapse.dataobjects;
using nerve.core.synapse.integrationpattern.process;
using RabbitMQ.Client;

namespace nerve.core.synapse.component.rabbitmq
{
    public class RabbitMqProducer : ProducerBase
    {
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

        public RabbitMqProducer(UriDescriptor uriInformation, Route route)
            : base(uriInformation, route)
        {

        }

        public override Exchange Process(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            try
            {
                var hostname = endPointDescriptor.GetUriProperty("hostname", "localhost");
                var queue = endPointDescriptor.GetUriProperty("queue", "systemQueue");
                var port = endPointDescriptor.GetUriProperty("port", 5672);
                var channel = GetChannel(string.Format("{0}::{1}::{2}",hostname,port,queue),queue,hostname);

                channel.QueueDeclare(queue: queue,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                var msg = exchange.InMessage.Body.ToString();
                var body = Encoding.UTF8.GetBytes(msg);

                //send
                channel.BasicPublish(exchange: "",
                             routingKey: queue,
                             basicProperties: null,
                             body: body);
            }
            catch (Exception exception)
            {
                var msg = exception.Message;
            }

            return exchange;
        }
    }
}
