using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nerve.core.synapse.util
{
    public class SynspseException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public SynspseException(string message = "", Exception exception = null) :
            base(message, exception)
        {

        }
    }
}
