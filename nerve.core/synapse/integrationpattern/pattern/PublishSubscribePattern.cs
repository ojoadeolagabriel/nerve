using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using nerve.core.synapse.dataobjects;
using nerve.core.synapse.integrationpattern.process;

namespace nerve.core.synapse.integrationpattern.pattern
{
    public class PublishSubscribePattern
    {
        public static void Process(XElement multicastElement, Exchange exchange)
        {
            try
            {
                var toElements = multicastElement.Elements("to");
                var xElements = toElements as XElement[] ?? toElements.ToArray();
                if (!xElements.Any())
                    return;

                foreach (var toTag in xElements)
                {
                    RouteStep.ExecuteRouteStep(toTag, exchange.Route, exchange);
                }
            }
            catch (Exception exception)
            {

            }
        }
    }
}
