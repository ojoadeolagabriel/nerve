using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace nerve.core.util.reader
{
    public class XmlDataUtil
    {
        public static T Attribute<T>(XElement element, string attribute, T defaultValue = default (T))
        {
            try
            {
                var result = element.Attribute(attribute).Value;
                return (T)Convert.ChangeType(result, typeof (T));
            }
            catch
            {
                
            }
            return defaultValue;
        }

        public static T GetValue<T>(XElement element, string tag, T defaultValue = default (T), string parentTag = null)
        {
            try
            {
                if (parentTag == null)
                {
                    return (T)Convert.ChangeType(element.Element(tag).Value, typeof(T));
                }
                return (T)Convert.ChangeType(element.Element(parentTag).Element(tag).Value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
