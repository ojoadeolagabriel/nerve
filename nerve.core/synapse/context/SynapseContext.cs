using System.Collections.Concurrent;
using nerve.core.synapse.dataobjects;

namespace nerve.core.synapse.context
{
    public class SynapseContext
    {
        public static readonly ConcurrentDictionary<string, Route> RouteCollection = new ConcurrentDictionary<string, Route>();

        public static void SetRoute(Route routeObj)
        {
            try
            {
                RouteCollection.TryAdd(routeObj.RouteId, routeObj);
            }
            catch
            {

            }
        }
    }
}
