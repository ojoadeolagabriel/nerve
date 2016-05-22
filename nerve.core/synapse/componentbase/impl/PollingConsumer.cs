using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nerve.core.synapse.dataobjects;

namespace nerve.core.synapse.componentbase.impl
{
    public class PollingConsumer : ConsumerBase
    {
        public bool CanRun(PackageDescriptor.Status status)
        {
            return status == PackageDescriptor.Status.Active;
        }

        public void StopRun()
        {

        }

        public virtual Exchange Poll()
        {
            return null;
        }
    }
}
