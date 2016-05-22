using System;
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
                var port = endPointDescriptor.GetUriProperty("port", 15672);

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

                        var msg = exchange.InMessage.Body.ToString();
                        var body = Encoding.UTF8.GetBytes(msg);

                        channel.BasicPublish(exchange: "",
                                     routingKey: queue,
                                     basicProperties: null,
                                     body: body);

                        if (SynapseContext.LogDebugInformation)
                            Console.WriteLine("[x] Sent {0}", msg);
                    }
                }
            }
            catch (Exception exception)
            {
                var msg = exception.Message;
            }

            return exchange;
        }
    }
}
