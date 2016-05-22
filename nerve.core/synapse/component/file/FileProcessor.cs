using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nerve.core.synapse.dataobjects;
using nerve.core.synapse.integrationpattern.process;

namespace nerve.core.synapse.component.file
{
    public class FileProcessor : ProcessorBase
    {
        public FileProcessor(UriDescriptor uriInformation, Route route)
            : base(uriInformation, route)
        {

        }
    }
}
