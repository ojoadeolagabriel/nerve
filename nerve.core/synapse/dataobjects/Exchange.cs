using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nerve.core.synapse.util;

namespace nerve.core.synapse.dataobjects
{
    /// <summary>
    /// Exchange Class.
    /// </summary>
    public class Exchange
    {

        public Guid ExchangeId;
        public Guid ParentExchangeId;
        public ConcurrentStack<SynapseException> Exception = new ConcurrentStack<SynapseException>();
        public ConcurrentStack<Object> AlternativeMessage = new ConcurrentStack<Object>();
        public ConcurrentDictionary<string, string> PropertyCollection { get; set; }
        public Message InMessage = new Message();
        public Message OutMessage = new Message();
        public Route Route;
        public DateTime CreatedTimestamp { get; set; }
        public MessageExchangeOption MepPattern { get; set; }

        public Exchange(Route route)
        {
            Route = route;
            ExchangeId = Guid.NewGuid();
            CreatedTimestamp = DateTime.Now;
            PropertyCollection = new ConcurrentDictionary<string, string>();
        }

        /// <summary>
        /// Safe get property
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public string GetProperty(string propertyName)
        {
            try
            {
                return PropertyCollection[propertyName];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Safe set property
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public void SetProperty(string propertyName, string value)
        {
            try
            {
                PropertyCollection.TryAdd(propertyName, value);
            }
            catch
            {

            }
        }
    }
}
