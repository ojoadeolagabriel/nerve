using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using nerve.core.secure.auth;
using nerve.core.synapse.context;
using nerve.core.synapse.initializer;

namespace nerve
{
    public class SignalStarter
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SignalStarter));

        static void Main(string[] args)
        {
            var strength = PasswordAdvisor.CheckStrength("PaSSwordGab43434.@@$$###r.12$");
            for (var i = 1; i < 100; i++)
            {
                _log.Error("console test log " + i);
            }
        }

        void StartServer()
        {
            SynapseContext.LoadRoutePath(new List<string> { @"C:\Users\AdeolaOjo\Documents\visual studio 2013\Projects\nerve\nervefileconsumer\bin\Debug\nervefileconsumer.dll" });
            SynapseContext.StartEngine();
            Console.ReadLine();
        }
    }
}
