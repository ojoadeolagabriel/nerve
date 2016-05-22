using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using nerve.core.synapse.componentbase;
using nerve.core.synapse.context;
using nerve.core.synapse.dataobjects;
using nerve.core.synapse.util;

namespace nerve.core.synapse.integrationpattern.process
{
    public class EndpointBuilder
    {
        public static ConcurrentQueue<string> PermissibleNamespaces = new ConcurrentQueue<string>();

        public static void BuildNamespaces(List<string> namespaces)
        {
            namespaces.ForEach(c =>
            {
                if (!PermissibleNamespaces.Contains(c))
                    PermissibleNamespaces.Enqueue(c);
            });
        }


        /// <summary>
        /// ProcessRouteInformation From Method
        /// </summary>
        /// <param name="leafDescriptor"></param>
        /// <param name="route"></param>
        /// <param name="exchangeData"></param>
        public static void HandleFrom(UriDescriptor leafDescriptor, Route route, Exchange exchangeData = null)
        {
            try
            {
                //init endpoint
                EndpointBase endPoint;
                if (SynapseContext.EnPointCollection.TryGetValue(leafDescriptor.FullUri, out endPoint))
                {

                }
                else
                {
                    var execAssembly = Assembly.GetExecutingAssembly();
                    var types = execAssembly.GetTypes();

                    foreach (var namespaceToCheck in PermissibleNamespaces)
                    {
                        try
                        {
                            var typeData = types.FirstOrDefault(
                                                    c => c.FullName.Equals(
                                                            string.Format("{0}.{1}.{2}", namespaceToCheck, leafDescriptor.ComponentName,
                                                                leafDescriptor.ComponentName),
                                                            StringComparison.InvariantCultureIgnoreCase) ||
                                                        c.FullName.Equals(string.Format("{0}.{1}.{2}EndPoint", namespaceToCheck,
                                                                leafDescriptor.ComponentName, leafDescriptor.ComponentName),
                                                            StringComparison.InvariantCultureIgnoreCase));

                            if (typeData == null)
                                continue;

                            endPoint = (EndpointBase)Activator.CreateInstance(typeData, leafDescriptor.FullUri, route);
                            SynapseContext.EnPointCollection.TryAdd(leafDescriptor.FullUri, endPoint);
                            break;
                        }
                        catch (Exception exception)
                        {
                        }
                    }
                }

                if (endPoint != null)
                {
                    if (exchangeData == null)
                        endPoint.Start();
                    else
                    {
                        endPoint.StartWithExistingExchange(exchangeData);
                    }
                }
                else
                    throw new SynapseException("[ErrorInitializingEndPoint] - " + leafDescriptor.ComponentName);
            }
            catch (AggregateException exception)
            {

            }
            catch (Exception exception)
            {
                throw new SynapseException("[ErrorInitializingEndPoint - FromTagIssues] - " + leafDescriptor.ComponentName);
            }
        }

        /// <summary>
        /// Handle To Method
        /// </summary>
        /// <param name="leafDescriptor"></param>
        /// <param name="exchange"></param>
        /// <param name="route"></param>
        public static void HandleTo(UriDescriptor leafDescriptor, Exchange exchange, Route route)
        {
            try
            {
                EndpointBase endPoint;
                if (SynapseContext.EnPointCollection.TryGetValue(leafDescriptor.FullUri, out endPoint))
                {
                    
                }
                else
                {
                    var execAssembly = Assembly.GetExecutingAssembly();
                    var types = execAssembly.GetTypes();

                    foreach (var namespaceToCheck in PermissibleNamespaces)
                    {
                        var typeData = types.FirstOrDefault(c => c.FullName.Equals(string.Format("{0}.{1}.{2}", namespaceToCheck, leafDescriptor.ComponentName, leafDescriptor.ComponentName),
                           StringComparison.InvariantCultureIgnoreCase) ||
                           c.FullName.Equals(string.Format("{0}.{1}.{2}EndPoint", namespaceToCheck, leafDescriptor.ComponentName, leafDescriptor.ComponentName),
                           StringComparison.InvariantCultureIgnoreCase));

                        if (typeData == null)
                            continue;

                        endPoint = (EndpointBase)Activator.CreateInstance(typeData, leafDescriptor.FullUri, route);
                        SynapseContext.EnPointCollection.TryAdd(leafDescriptor.FullUri, endPoint);
                        break;
                    }
                }

                if (endPoint != null)
                    endPoint.Send(exchange, leafDescriptor);
                else
                    throw new SynapseException("[MissingEndPoint] - " + leafDescriptor.ComponentName);
            }
            catch (AggregateException exception)
            {
                throw new SynapseException("[EndPointIssues] - " + leafDescriptor.ComponentName, exception);
            }
            catch (Exception exception)
            {
                throw new SynapseException("[EndPointIssues] - " + leafDescriptor.ComponentName, exception);
            }
        }
    }
}
