using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nerve.core.synapse.component.file;
using nerve.core.synapse.componentbase;
using nerve.core.synapse.componentbase.impl;
using nerve.core.synapse.dataobjects;
using nerve.core.synapse.integrationpattern.process;

namespace nerve.core.synapse.component.direct
{
    public class DirectEndpoint : EndpointBase
    {
        private DirectConsumer _consumer;

        public DirectEndpoint(string uri, Route route) : base(uri, route)
        {

        }

        public override void Send(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            var producer = CreateProducer();
            producer.Process(exchange, endPointDescriptor);
        }

        public DirectConsumer CreateConsumer()
        {
            return new DirectConsumer(new DirectProcessor(UriDescriptor, base.Route));
        }

        public ProducerBase CreateProducer()
        {
            return new DirectProducer(UriDescriptor, Route);
        }

        /// <summary>
        /// Start work
        /// </summary>
        public override void Start()
        {
            _consumer = CreateConsumer();
        }
    }
}
