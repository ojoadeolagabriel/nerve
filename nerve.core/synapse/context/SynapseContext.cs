using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using nerve.core.synapse.componentbase;
using nerve.core.synapse.dataobjects;
using nerve.core.synapse.initializer;
using nerve.core.synapse.integrationpattern.process;

namespace nerve.core.synapse.context
{
    public class SynapseContext
    {
        public static ConcurrentQueue<string> AssemblyCollection = new ConcurrentQueue<string>();
        public static BeanRegistry Registry = new BeanRegistry();
        public static readonly ConcurrentDictionary<string, Route> RouteCollection = new ConcurrentDictionary<string, Route>();
        public static readonly ConcurrentDictionary<string, EndpointBase> EnPointCollection = new ConcurrentDictionary<string, EndpointBase>();

        /// <summary>
        /// Register new route
        /// </summary>
        /// <param name="routeObj"></param>
        public static void SetRoute(Route routeObj)
        {
            try
            {
                RouteCollection.TryAdd(routeObj.RouteId, routeObj);
            }
            catch (AggregateException exception)
            {

            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Load bean
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object LoadBean(string type)
        {
            foreach (var assemblyCollection in AssemblyCollection)
            {
                try
                {
                    var instance = Activator.CreateInstance(assemblyCollection, type).Unwrap();
                    if (instance != null)
                        return instance;
                }
                catch
                {

                }
            }
            return null;
        }

        public static void StartEngine()
        {
            StartAllRoutes();
            StartSedaProcessor();
        }
        public static void LoadRoutePath(List<string> path, List<string> nameSpaces = null, List<string> assemblies = null)
        {
            InitDependencyLibs(nameSpaces ?? new List<string> { AppCoreDefaultNamespace });
            if (assemblies != null) assemblies.ForEach(c => AssemblyCollection.Enqueue(c));

            foreach (var filePath in path)
            {
                SynapseContextInitializer.Initialize(filePath);
            }
        }

        public static string AppCoreDefaultNamespace = "nerve.core.synapse.component";

        public static void InitDependencyLibs(List<string> namespaces)
        {
            EndpointBuilder.BuildNamespaces(namespaces);
        }

        public static void StartAllRoutes()
        {
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Starting all components...");
            RouteCollection.ToList().ForEach(c => c.Value.CurrentRouteStep.ProcessChannel());
        }

        public static void StartSedaProcessor()
        {
            new SedaDriver().ProcessSedaMessageQueue();
        }

    }
}
