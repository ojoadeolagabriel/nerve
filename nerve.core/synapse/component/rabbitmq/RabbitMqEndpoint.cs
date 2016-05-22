using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nerve.core.synapse.component.file;
using nerve.core.synapse.componentbase;
using nerve.core.synapse.componentbase.impl;
using nerve.core.synapse.dataobjects;
using nerve.core.synapse.integrationpattern.process;

namespace nerve.core.synapse.component.rabbitmq
{
    class RabbitMqEndpoint : EndpointBase
    {
         /// <summary>
        /// File Consumer
        /// </summary>
        private RabbitMqConsumer _consumer;


        public override void Send(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            var producer = CreateProducer();
            producer.Process(exchange, endPointDescriptor);
        }

        /// <summary>
        /// Start work
        /// </summary>
        public override void Start()
        {
            _consumer = (RabbitMqConsumer)CreateConsumer();

            if (_consumer.GetType().IsSubclassOf(typeof(PollingConsumer)))
            {
                _consumer.Poll();
            }
            else
            {
                _consumer.Execute();
            }
        }

        /// <summary>
        /// Init endpoint
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="route"></param>
        public RabbitMqEndpoint(string uri, Route route)
            : base(uri, route)
        {

        }

        public ConsumerBase CreateConsumer()
        {
            return new RabbitMqConsumer(new RabbitMqProcessor(UriDescriptor, Route));
        }

        public ProducerBase CreateProducer()
        {
            return new RabbitMqProducer(UriDescriptor, Route);
        }
    }
}
