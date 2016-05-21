using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using nerve.core.synapse.dataobjects;

namespace nerve.core.synapse.integrationpattern.process
{
    public class RouteStep
    {
        public RouteStep(XElement xmlStep, Route routeObj)
        {
            
        }

        public RouteStep NextRouteStep { get; set; }
    }
}
