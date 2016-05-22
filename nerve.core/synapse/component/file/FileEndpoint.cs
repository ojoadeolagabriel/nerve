using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nerve.core.synapse.componentbase;
using nerve.core.synapse.componentbase.impl;
using nerve.core.synapse.dataobjects;
using nerve.core.synapse.integrationpattern.process;

namespace nerve.core.synapse.component.file
{
    public class FileEndpoint : EndpointBase
    {
        /// <summary>
        /// File Consumer
        /// </summary>
        private FileConsumer _consumer;


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
            _consumer = (FileConsumer)CreateConsumer();

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
        public FileEndpoint(string uri, Route route)
            : base(uri, route)
        {

        }

        public ConsumerBase CreateConsumer()
        {
            return new FileConsumer(new FileProcessor(UriDescriptor, base.Route));
        }

        public ProducerBase CreateProducer()
        {
            return new FileProducer(UriDescriptor, Route);
        }
    }
}
