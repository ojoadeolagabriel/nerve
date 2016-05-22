using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using nerve.core.util.reader;

namespace nerve.core.synapse.dataobjects
{
    public class PackageDescriptor
    {
        public string GuidData { get; set; }

        public enum Status
        {
            Starting, Paused, Active, Installed, Stopping, System, UnInstalled
        }

        public PackageDescriptor(string descriptorXmlInfo)
        {
            var xmlData = XElement.Parse(descriptorXmlInfo);

            Name = XmlDataUtil.GetValue<string>(xmlData, "name");
            ModelVersion = XmlDataUtil.GetValue<string>(xmlData, "version");
            Author = XmlDataUtil.GetValue(xmlData, "author", "default.author");
            GroupId = XmlDataUtil.GetValue(xmlData, "groupid", "com.nerve.group");
            Priority = XmlDataUtil.GetValue(xmlData, "priority", "low");
            GuidData = Guid.NewGuid().ToString();
            PackageStatus = Status.Active;
        }

        public string Priority { get; set; }
        public string GroupId { get; set; }
        public string Author { get; set; }
        public string ModelVersion { get; set; }
        public string Name { get; set; }
        public Status PackageStatus { get; set; }
    }
}
