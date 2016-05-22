using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nerve.core.synapse.dataobjects;

namespace nerve.core.synapse.strategy.contracts
{
    public interface ISplitterStrategy
    {
        List<string> Split(Exchange exchange);
    }
}
