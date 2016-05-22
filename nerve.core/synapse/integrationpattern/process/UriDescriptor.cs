using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nerve.core.synapse.dataobjects;
using nerve.core.synapse.resolver;
using nerve.core.synapse.util;

namespace nerve.core.synapse.integrationpattern.process
{
    public class UriDescriptor
    {
        public UriDescriptor Parse(string uri, Exchange exchange = null)
        {
            var parts = new UriDescriptor();

            if (exchange != null)
                uri = SimpleExpression.ResolveSpecifiedUriPart(uri, exchange);

            if (string.IsNullOrEmpty(uri))
                throw new SynapseException("uri data error: cannot be empty");

            var mainParts = uri.Split(new[] { '?' }, 2);

            var uriPrimaryParts = mainParts[0].Split(new[] { ':' }, 2);

            parts.ComponentName = uriPrimaryParts.Length >= 1 ? uriPrimaryParts[0] : "";
            parts.ComponentPath = uriPrimaryParts.Length >= 2 ? uriPrimaryParts[1] : "";
            parts.ComponentQueryPath = mainParts.Length > 1 ? mainParts[1] : "";
            parts.FullUri = uri;

            return parts;
           
        }

        public string FullUri { get; set; }

        public string ComponentQueryPath { get; set; }

        public string ComponentPath { get; set; }

        public string ComponentName { get; set; }
    }
}
