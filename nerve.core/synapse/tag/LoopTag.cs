using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using nerve.core.synapse.context;
using nerve.core.synapse.dataobjects;
using nerve.core.synapse.integrationpattern.process;
using nerve.core.synapse.resolver;

namespace nerve.core.synapse.tag
{
    public class LoopTag
    {
        public static void Execute(XElement loopTag, Exchange exchange, Route route)
        {
            var expressionTag = loopTag.Elements().FirstOrDefault();
            if (expressionTag == null || (expressionTag.Name != "count"))
                return;

            var xpression = expressionTag.Value;
            var count = SimpleExpression.ResolveSpecifiedUriPart(xpression, exchange);

            var mCount = Convert.ToInt32(count);
            for (var i = 0; i < mCount; i++)
            {
                var data = loopTag.Elements().Skip(1);
                foreach (var dataItem in data)
                {
                    try
                    {
                        RouteStep.ExecuteRouteStep(dataItem, route, exchange);
                    }
                    catch (Exception exception)
                    {

                    }
                }
            }
        }
    }
}
