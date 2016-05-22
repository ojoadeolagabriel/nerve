using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nerve.core.synapse.dataobjects;
using nerve.core.synapse.integrationpattern.process;
using System.Threading.Tasks;

namespace nerve.core.synapse.integrationpattern.pattern
{
    public class WireTapPattern
    {
        public static void Execute(Exchange exchange, string path, Route route)
        {
            Task.Factory.StartNew(() => Tap(exchange, path, route));
        }

        private static void Tap(Exchange exchange, string path, Route route)
        {
            try
            {
                var leafNodeParts = UriDescriptor.Parse(path, exchange);
                EndpointBuilder.HandleTo(leafNodeParts, exchange, route);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}
