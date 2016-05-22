using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nerve.core.synapse.dataobjects;
using nerve.core.synapse.integrationpattern.process;

namespace nerve.core.synapse.componentbase
{
    public class EndpointBase
    {
        public Route Route { get; set; }
        public string ComponentTitle { get; set; }

        public virtual void Start()
        {
            
        }

        public EndpointBase(string uri, Route route)
        {
            Route = route;
            Uri = uri;
            UriDescriptor = UriDescriptor.Parse(uri);    
        }

        public UriDescriptor UriDescriptor { get; set; }

        public string Uri { get; set; }

        public virtual void Send(Exchange exchange, UriDescriptor endPointDescriptor)
        {
        }

        public virtual void StartWithExistingExchange(Exchange exchangeData)
        {
        }
    }
}
