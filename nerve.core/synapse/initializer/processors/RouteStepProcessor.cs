using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using nerve.core.synapse.context;
using nerve.core.synapse.dataobjects;
using nerve.core.synapse.initializer.processors;
using nerve.core.synapse.integrationpattern.process;
using nerve.core.synapse.util;
using nerve.core.util.reader;

namespace nerve.core.synapse.initializer.processors
{
    public class RouteStepProcessor
    {
        private readonly XElement _route;
        private bool _autoTrigger;
        private readonly PackageDescriptor _packageDescriptor;

        public RouteStepProcessor(XElement route, bool autoTrigger = false, PackageDescriptor packageDescriptor = null)
        {
            _packageDescriptor = packageDescriptor;
            _autoTrigger = autoTrigger;
            _route = route;
        }

        public void LoadAllSteps()
        {
            var leafContextXml = _route.Element(Constant.LeafContext);

            //get first route
            if (leafContextXml != null)
            {
                var routeNode = leafContextXml.Elements("route");
                foreach (var route in routeNode)
                {
                    var xmlRoute = route;
                    Run(xmlRoute);
                }
            }
            else
            {
                throw new SynapseException("[ErrorReadingRouteXml_MissingNerveContext] - context tag possibly omitted in file");
            }
        }

        /// <summary>
        /// Digest Route Information
        /// </summary>
        private void Run(XElement routeElement)
        {
            if (routeElement == null)
                throw new SynapseException("[ErrorReadingRouteXml] - route information not valid");
            try
            {
                var description = XmlDataUtil.Attribute<string>(routeElement, "description");
                var routeObj = new Route { Description = description, PackageDescriptor = _packageDescriptor };
                RouteStep nextRouteStepProcessorToLink = null;

                //read all steps in route
                foreach (var xmlStep in routeElement.Elements())
                {
                    if (routeObj.CurrentRouteStep == null)
                    {
                        routeObj.CurrentRouteStep = new RouteStep(xmlStep, routeObj);
                        nextRouteStepProcessorToLink = routeObj.CurrentRouteStep;
                    }
                    else
                    {
                        var nextStep = new RouteStep(xmlStep, routeObj);
                        if (nextRouteStepProcessorToLink == null)
                            continue;

                        nextRouteStepProcessorToLink.NextRouteStep = nextStep;
                        nextRouteStepProcessorToLink = nextRouteStepProcessorToLink.NextRouteStep;
                    }
                }

                //add route for execution
                SynapseContext.SetRoute(routeObj);
            }
            catch (Exception exception)
            {
                var msg = exception.Message;
            }
        }
    }
}
