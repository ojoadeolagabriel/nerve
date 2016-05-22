using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nerve.core.synapse.integrationpattern.process;

namespace nerve.core.synapse.dataobjects
{
    public class Route
    {
        public string Description { get; set; }
        public PackageDescriptor PackageDescriptor { get; set; }

        public RouteStep CurrentRouteStep { get; set; }
        public string RouteId;
        public MessagePipelineMode PipelineMode = MessagePipelineMode.Default;

        public Route()
        {
            RouteId = Guid.NewGuid().ToString();
        }

        public enum MessagePipelineMode
        {
            Default,
            Seda
        }
    }
}
