using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using nerve.core.synapse.context;
using nerve.core.synapse.dataobjects;
using nerve.core.synapse.integrationpattern.process;
using nerve.core.synapse.strategy;

namespace nerve.core.synapse.integrationpattern.pattern
{
    public class EnricherPattern
    {
        public static void Enrich(XElement step, Exchange exchange, Route routeObj)
        {
            try
            {
                var uri = step.Attribute("uri");
                var strategyref = step.Attribute("strategyref");

                if (uri == null || strategyref == null)
                    return;

                var uriInfo = UriDescriptor.Parse(uri.Value, exchange);
                var clonedExchange = exchange.CloneExchange();
                EndpointBuilder.HandleTo(uriInfo, clonedExchange, routeObj);

                //fetch strategy
                var stragegyObj = SynapseContext.Registry[strategyref.Value] as AggregationStrategy;
                if (stragegyObj != null)
                {
                    stragegyObj.Aggregate(exchange, clonedExchange);
                }
            }
            catch (Exception exc)
            {

            }
        }
    }
}
