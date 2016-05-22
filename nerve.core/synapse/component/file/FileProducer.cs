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

        /// <summary>
        /// Process
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="endPointDescriptor"></param>
        /// <returns></returns>
        public override Exchange Process(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            try
            {
                //can create dir
                var createOnMissingDir = endPointDescriptor.GetUriProperty<bool>("create");

                //create dir if not found
                if (!Directory.Exists(endPointDescriptor.ComponentPath))
                {
                    if (createOnMissingDir)
                        Directory.CreateDirectory(endPointDescriptor.ComponentPath);
                    else
                    {
                        return exchange;
                    }
                }

                //get details.
                var filePath = exchange.InMessage.GetHeader<string>("filePath");
                var fileExtension = endPointDescriptor.GetUriProperty<string>("fileExtension");

                //change_file_path
                var originalExt = Path.GetExtension(filePath);
                filePath = Path.ChangeExtension(filePath, fileExtension ?? originalExt);
                var newExt = Path.GetExtension(filePath);

                //new path
                var fileNameWithExt = Path.GetFileName(filePath);
                if (fileNameWithExt != null) 
                    filePath = Path.Combine(endPointDescriptor.ComponentPath, fileNameWithExt);

                filePath = Path.ChangeExtension(filePath, string.Format("{0}{1}", DateTime.Now.Ticks, newExt));

                //write
                File.AppendAllText(filePath, exchange.InMessage.Body.ToString());
            }
            catch (Exception exception)
            {
                var msg = exception.Message;
            }

            //done
            return exchange;
        }
    }
}
