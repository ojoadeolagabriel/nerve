using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nerve.core.synapse.dataobjects;

namespace nerve.core.synapse.componentbase
{
    public class ConsumerBase
    {
        public virtual Exchange Execute()
        {
            return null;
        }

        public virtual bool PauseConsumer()
        {
            return false;
        }

        public virtual bool ResumeConsumer()
        {
            return false;
        }

        public virtual Exchange Execute(Exchange exchange)
        {
            return null;
        }

        public virtual bool UnLoad()
        {
            return true;
        }
    }
}
