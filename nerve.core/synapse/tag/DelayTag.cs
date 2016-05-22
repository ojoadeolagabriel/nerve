using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using nerve.core.synapse.dataobjects;
using nerve.core.synapse.integrationpattern.process;

namespace nerve.core.synapse.tag
{
    public class DelayTag
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="exchange"></param>
        /// <param name="uriDescriptor"></param>
        public static void Execute(string delay, Exchange exchange, UriDescriptor uriDescriptor)
        {
            var ndxDelay = 1000;
            try
            {
                ndxDelay = Convert.ToInt32(delay);
            }
            catch (AggregateException exception)
            {

            }
            catch (Exception exception)
            {

            }

            Thread.Sleep(ndxDelay);
        }
    }
}
