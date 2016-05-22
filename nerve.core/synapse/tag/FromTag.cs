using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nerve.core.synapse.dataobjects;
using nerve.core.synapse.integrationpattern.process;
using nerve.core.synapse.resolver;

namespace nerve.core.synapse.tag
{
    public class FromTag
    {
        public static void Execute(string uri, Exchange exchange, Route route)
        {
            uri = SimpleExpression.ResolveSpecifiedUriPart(uri, exchange);
            var leafNodeParts = UriDescriptor.Parse(uri, exchange);
            EndpointBuilder.HandleFrom(leafNodeParts, route);
        }
    }
}
