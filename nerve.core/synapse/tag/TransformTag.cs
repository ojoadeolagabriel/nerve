using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using nerve.core.synapse.dataobjects;
using nerve.core.synapse.resolver;

namespace nerve.core.synapse.tag
{
    public class TransformTag
    {
        public static void HandleTransform(XElement step, Route routeObj, Exchange exchange)
        {
            try
            {
                var transformXml = step.Elements().First();
                var opName = transformXml.Name.ToString();

                switch (opName)
                {
                    case "simple":
                        ProcessSimple(transformXml, routeObj, exchange);
                        break;
                    case "xpath":
                        break;
                }
            }
            catch (AggregateException aggregateException)
            {

            }
            catch (Exception exception)
            {

            }
        }

        private static void ProcessSimple(XElement transformXml, Route routeObj, Exchange exchange)
        {
            var newBody = SimpleExpression.ResolveSpecifiedUriPart(transformXml.Value, exchange);
            exchange.InMessage.Body = newBody;
        }
    }
}
