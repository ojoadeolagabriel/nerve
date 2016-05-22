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
            var pollInterval = _fileProcessor.UriInformation.GetUriProperty("poll", 1000);
            var maxThreadCount = _fileProcessor.UriInformation.GetUriProperty("threadCount", 3);
            var createIfDirNotFound = _fileProcessor.UriInformation.GetUriProperty("create", true);
            var initialDelay = _fileProcessor.UriInformation.GetUriProperty("initialDelay", 1000);

            if (initialDelay > 100)
                Thread.Sleep(initialDelay);

            var totalCount = 0;

            //loop.
            while (true)
            {
                if (totalCount >= maxThreadCount)
                    continue;

                if(!CanRun(_fileProcessor.Route.PackageDescriptor.PackageStatus))
                    continue;

                ++totalCount;
                try
                {
                    var exchange = new Exchange(_fileProcessor.Route);
                    var fileData = "";
                    var fileFolderPath = _fileProcessor.UriInformation.ComponentPath;

                    if (Directory.Exists(fileFolderPath))
                    {
                        var fileInfo = Directory.GetFiles(fileFolderPath).FirstOrDefault();
                        if (fileInfo != null)
                        {
                            fileFolderPath = fileInfo;
                            exchange.InMessage.SetHeader("filePath", fileFolderPath);
                            fileData = File.ReadAllText(fileInfo);
                        }
                    }
                    else if (File.Exists(fileFolderPath))
                    {
                        fileData = File.ReadAllText(fileFolderPath);
                    }

                    ProcessResponse(fileData, exchange, fileFolderPath);
                }
                catch (Exception exception)
                {
                    var msg = exception.Message;
                }
                
                --totalCount;
                Thread.Sleep(pollInterval);
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
