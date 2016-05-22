using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nerve.core.synapse.componentbase;
using nerve.core.synapse.context;
using nerve.core.synapse.dataobjects;
using nerve.core.synapse.integrationpattern.process;

namespace nerve.core.synapse.component.direct
{
    public class DirectProducer : ProducerBase
    {
        public DirectProducer(UriDescriptor uriInformation, Route route) : base(uriInformation, route)
        {
        }

        public override Exchange Process(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            var path = endPointDescriptor.ComponentPath;
            var route = SynapseContext.GetRouteBy(path);

            if (route != null)
                route.CurrentRouteStep.ProcessChannel(exchange);

            return exchange;
        }
    }
}
