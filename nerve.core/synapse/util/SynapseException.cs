using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nerve.core.synapse.util
{
    public class SynapseException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public SynapseException(string message = "", Exception exception = null) :
            base(message, exception)
        {

        }
    }
}
