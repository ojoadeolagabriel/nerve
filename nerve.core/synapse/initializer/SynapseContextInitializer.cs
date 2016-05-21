using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using nerve.core.synapse.util;
using nerve.core.util.reader;

namespace nerve.core.synapse.initializer
{
    public class SynapseContextInitializer
    {
        public class RouteStepProcessor
        {
            public void DigestRouteInformation(XElement route, bool autoTrigger = false)
            {
                if (route == null)
                    throw new SynspseException("[ErrorReadingRouteXml] - route information not valid");

                var description = XmlDataUtil.Attribute<string>(route, "description");
            }
        }
    }
}
