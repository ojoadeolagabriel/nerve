using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nerve.core.synapse.dataobjects;

namespace nerve.core.synapse.integrationpattern.process
{
    public class ProcessorBase
    {
        public UriDescriptor UriInformation;
        public Route Route;

        protected ProcessorBase(UriDescriptor uriInformation, Route route)
        {
            UriInformation = uriInformation;
            Route = route;
        }

        public virtual Exchange Process(Exchange exchange)
        {
            if (Route.CurrentRouteStep.NextRouteStep != null)
                Route.CurrentRouteStep.NextRouteStep.ProcessChannel(exchange);
            return exchange;
        }
    }
}
