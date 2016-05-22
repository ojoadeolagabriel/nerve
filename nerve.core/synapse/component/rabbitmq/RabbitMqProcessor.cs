using nerve.core.synapse.integrationpattern.process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nerve.core.synapse.dataobjects;

namespace nerve.core.synapse.component.rabbitmq
{
    public class RabbitMqProcessor : ProcessorBase
    {
        public RabbitMqProcessor(UriDescriptor uriInformation, Route route) : base(uriInformation, route)
        {

        }
    }
}
