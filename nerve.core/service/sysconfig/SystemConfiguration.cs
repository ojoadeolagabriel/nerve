using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nerve.core.util.config;

namespace nerve.core.service.sysconfig
{
    public class SystemConfiguration : ConfigBase<SystemConfiguration>
    {
        public int RabbitMqPort { get; set; }
        public string FileDataDumpPath { get; set; }
        public string RmqFileDataDumpPath { get; set; }

        public SystemConfiguration()
        {
            Load();
        }
    }
}
