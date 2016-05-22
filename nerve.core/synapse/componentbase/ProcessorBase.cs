using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nerve.core.synapse.dataobjects;
using nerve.core.synapse.integrationpattern.process;

namespace nerve.core.synapse.componentbase
{
    /// <summary>
    /// ProcessorBase Class.
    /// </summary>
    public class ProcessorBase
    {
        public UriDescriptor UriInformation;

        public Route Route;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="uriInformation"></param>
        /// <param name="route"></param>
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
