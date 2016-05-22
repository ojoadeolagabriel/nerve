using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nerve.core.synapse.dataobjects;
using nerve.core.synapse.integrationpattern.process;

namespace nerve.core.synapse.componentbase
{
    public class ProducerBase
    {
        private UriDescriptor _uriInformation;
        private Route _route;

        public ProducerBase(UriDescriptor uriInformation, Route route)
        {
            _uriInformation = uriInformation;
            _route = route;
        }

        public virtual Exchange Process(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            return null;
        }
    }
}
