using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using nerve.core.synapse.componentbase;
using nerve.core.synapse.context;
using nerve.core.synapse.dataobjects;
using nerve.core.synapse.util.ext;

namespace nerve.core.synapse.integrationpattern.process
{
    public class SedaDriver
    {
        public static ConcurrentDictionary<UriDescriptor, Exchange> SedaQueue = new ConcurrentDictionary<UriDescriptor, Exchange>();

        public void ProcessSedaMessageQueue()
        {
            System.Threading.Tasks.Task.Factory.StartNew(HandleQueue);
        }

        /// <summary>
        /// 
        /// </summary>
        private static void HandleQueue()
        {
            while (true)
            {
                var data = SedaQueue.FirstOrDefault();
                if (data.IsNull())
                {
                    Thread.Sleep(1000);
                    continue;
                }

                Exchange removedData;
                SedaQueue.TryRemove(data.Key, out removedData);

                var concurrentConsumers = data.Key.GetUriProperty("concurrentConsumers", false);
                var timeOut = data.Key.GetUriProperty("timeOut", 0);

                if (concurrentConsumers)
                {
                    System.Threading.Tasks.Task.Factory.StartNew(() => ProcessNextStep(data));
                }
                else
                    ProcessNextStep(data);
            }
        }

        /// <summary>
        /// Process Next Step
        /// </summary>
        /// <param name="xchangeInfo"></param>
        private static void ProcessNextStep(KeyValuePair<UriDescriptor, Exchange> xchangeInfo)
        {
            //handle
            EndpointBase endPoint;
            if (!SynapseContext.EnPointCollection.TryGetValue(xchangeInfo.Key.FullUri, out endPoint)) return;

            Route sedaRoute;
            SynapseContext.RouteCollection.TryGetValue(endPoint.UriDescriptor.ComponentPath, out sedaRoute);
            if (sedaRoute != null)
            {
                EndpointBuilder.HandleFrom(endPoint.UriDescriptor, xchangeInfo.Value.Route, xchangeInfo.Value);
            }

            xchangeInfo.Value.Route.CurrentRouteStep.NextRouteStep.ProcessChannel(xchangeInfo.Value);
        }
    }
}
