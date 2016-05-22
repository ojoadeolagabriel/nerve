using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nerve.core.synapse.util.synapseconstant
{
    public class TagConstant
    {
        public const string FromTag = "from";
        public const string ToTag = "to";
        public const string Direct = "direct";
        public const string Seda = "seda";
        public const string Uri = "uri";
        public const string Route = "route";
        public const string Description = "description";

        public const string Enrich = "enrich";
        public const string Split = "split";
        public const string Multicast = "multicast";
        public const string Process = "process";
        public const string Bean = "bean";

        public const string Convertbodyto = "convertbodyto";
        public const string Setheader = "setheader";
        public const string Removeheader = "removeheader";
        public const string Setproperty = "setproperty";
        public const string Choice = "choice";
        public const string Reference = "ref";
        public const string Method = "method";

        public const string Loop = "loop";
        public const string Delay = "delay";
        public const string Wiretap = "wiretap";
        public const string Transform = "transform";
        public const string Filter = "filter";
        public const string Headername = "headername";
        public const string Value = "value";
        public const string Name = "name";
        public const string Type = "type";
    }
}
