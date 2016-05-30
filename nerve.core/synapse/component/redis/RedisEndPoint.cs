using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nerve.core.synapse.componentbase;
using nerve.core.synapse.componentbase.impl;
using nerve.core.synapse.dataobjects;

namespace nerve.core.synapse.component.redis
{
    public class RedisEndpoint : EndpointBase
    {
        private RedisConsumer _consumer;

        public RedisEndpoint(string uri, Route route) : base(uri, route)
        {
        }
        public override void Start()
        {
            _consumer = (RedisConsumer)CreateConsumer();

            if (_consumer.GetType().IsSubclassOf(typeof(PollingConsumer)))
            {
                _consumer.Poll();
            }
            else
            {
                _consumer.Execute();
            }
        }

        private RedisConsumer CreateConsumer()
        {
            return new RedisConsumer(new RedisProcessor(UriDescriptor, Route));
        }
    }
}
