using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nerve.core.synapse.util;
using nerve.core.util.reflectivity;

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
        /// Clone Exchange
        /// </summary>
        /// <param name="inMessage"></param>
        /// <param name="outMessage"></param>
        /// <param name="route"></param>
        /// <returns></returns>
        public Exchange CloneExchange(Message inMessage = null, Message outMessage = null, Route route = null)
        {
            var exc = new Exchange(Route)
            {
                InMessage = inMessage ?? ReflectionHelper.DeepCopy(InMessage),
                OutMessage = outMessage ?? ReflectionHelper.DeepCopy(OutMessage),
                CreatedTimestamp = DateTime.Now,
                ExchangeId = Guid.NewGuid(),
                PropertyCollection = new ConcurrentDictionary<string, string>(),
            };

            return exc;
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
