using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using nerve.core.synapse.context;
using nerve.core.synapse.dataobjects;
using nerve.core.synapse.integrationpattern.process;
using nerve.core.synapse.util;
using nerve.core.util.reader;

namespace nerve.core.synapse.initializer
{
    public class SynapseContextInitializer
    {
        public class RouteStepProcessor
        {
            /// <summary>
            /// Digest Route Information
            /// </summary>
            /// <param name="route"></param>
            /// <param name="autoTrigger"></param>
            /// <param name="packageDescriptor"></param>
            public void DigestRouteInformation(XElement route, bool autoTrigger = false, PackageDescriptor packageDescriptor = null)
            {
                if (route == null)
                    throw new SynspseException("[ErrorReadingRouteXml] - route information not valid");

                try
                {
                    var description = XmlDataUtil.Attribute<string>(route, "description");
                    var routeObj = new Route { Description = description, PackageDescriptor = packageDescriptor };
                    RouteStep nextRouteStepProcessorToLink = null;

                    //read all steps in route
                    foreach (var xmlStep in route.Elements())
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
}
