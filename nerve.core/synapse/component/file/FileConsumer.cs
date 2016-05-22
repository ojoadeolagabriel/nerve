using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using nerve.core.synapse.componentbase.impl;
using nerve.core.synapse.dataobjects;

namespace nerve.core.synapse.component.file
{
    public class FileConsumer : PollingConsumer
    {
        private readonly FileProcessor _fileProcessor;


        public FileConsumer(FileProcessor fileProcessor)
        {
            _fileProcessor = fileProcessor;
        }

        public override Exchange Poll()
        {
            //PollHandler();
            Task.Factory.StartNew(PollHandler);
            return null;
        }

        private void PollHandler()
        {
            var deleteOnRead = _fileProcessor.UriInformation.GetUriProperty("deleteOnRead", false);
            var searchPattern = _fileProcessor.UriInformation.GetUriProperty("searchPattern", "txt");

            //loop.
            while (true)
            {
                if (!CanRun(_fileProcessor.Route.PackageDescriptor.PackageStatus))
                    continue;

                try
                {
                    var exchange = new Exchange(_fileProcessor.Route);
                    var fileFolderPath = _fileProcessor.UriInformation.ComponentPath;

                    if (Directory.Exists(fileFolderPath))
                    {
                        var firstFile = new DirectoryInfo(fileFolderPath).GetFiles().FirstOrDefault(name=> Path.GetExtension(name.FullName) == searchPattern);

                        if (firstFile != null)
                        {
                            exchange.InMessage.SetHeader("filePath", fileFolderPath);
                            fileFolderPath = firstFile.FullName;
                            var fileData = File.ReadAllText(fileFolderPath);

                            if (deleteOnRead)
                                File.Delete(fileFolderPath);

                            ProcessResponse(fileData, exchange, fileFolderPath);
                        }
                        else
                        {
                            Thread.Sleep(1000);
                        }
                    }
                }
                catch (Exception exception)
                {
                    var msg = exception.Message;
                }
            }
        }

        private void ProcessResponse(string fileData, Exchange exchange, string fileFolderPath)
        {
            exchange.InMessage.SetHeader("fileFolderPath", fileFolderPath);
            exchange.InMessage.Body = fileData;

            _fileProcessor.Process(exchange);
            exchange.OutMessage.Body = exchange.InMessage.Body;
        }
    }
}
