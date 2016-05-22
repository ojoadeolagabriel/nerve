using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nerve.core.synapse.dataobjects;

namespace nerve.core.synapse.strategy
{
    public abstract class AggregationStrategy
    {
        public abstract Exchange Aggregate(Exchange oldExchange, Exchange newExchange);
    }
}
