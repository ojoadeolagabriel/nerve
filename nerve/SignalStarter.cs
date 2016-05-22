using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nerve.core.synapse.context;
using nerve.core.synapse.initializer;

namespace nerve
{
    public class SignalStarter
    {
        static void Main(string[] args)
        {
            SynapseContext.LoadRoutePath(new List<string> { @"C:\Users\AdeolaOjo\Documents\visual studio 2013\Projects\nerve\nervefileconsumer\bin\Debug\nervefileconsumer.dll" });
            SynapseContext.StartEngine();
            Console.ReadLine();
        }
    }
}
