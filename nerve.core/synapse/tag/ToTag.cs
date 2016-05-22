using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nerve.core.synapse.dataobjects;
using nerve.core.synapse.integrationpattern.process;

namespace nerve.core.synapse.tag
{
    public class ToTag
    {
        public static void Execute(string uri, Exchange exchange, Route route)
        {
            var leafNodeParts = UriDescriptor.Parse(uri, exchange);
            EndpointBuilder.HandleTo(leafNodeParts, exchange, route);
        }
    }
}
