using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using nerve.core.synapse.componentbase;
using nerve.core.synapse.context;
using nerve.core.synapse.dataobjects;
using nerve.core.synapse.integrationpattern.process;

namespace nerve.core.synapse.component.file
{
    public class FileProducer : ProducerBase
    {
        public FileProducer(UriDescriptor uriInformation, Route route)
            : base(uriInformation, route)
        {

        }

        public override Exchange Process(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            var fileData = exchange.OutMessage.Body != null ? exchange.OutMessage.Body.ToString() : "";
            var fileName = endPointDescriptor.ComponentPath;

            if (!string.IsNullOrEmpty(fileData) && !string.IsNullOrEmpty(fileName))
                File.AppendAllText(fileName, fileData);

            return exchange;
        }
    }
}
