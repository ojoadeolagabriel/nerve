using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nerve.core.synapse.componentbase.impl;
using nerve.core.synapse.dataobjects;

namespace nerve.core.synapse.component.direct
{
    public class DirectConsumer : PollingConsumer
    {
        private readonly DirectProcessor _directProcessor;

        public DirectConsumer(DirectProcessor directProcessor)
        {
            _directProcessor = directProcessor;
        }

        public override Exchange Execute()
        {
            var exchange = new Exchange(_directProcessor.Route);
            _directProcessor.Process(exchange);
            return exchange;
        }
    }
}
