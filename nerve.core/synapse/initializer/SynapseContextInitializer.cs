using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using nerve.core.synapse.context;
using nerve.core.synapse.dataobjects;
using nerve.core.synapse.initializer.processors;
using nerve.core.synapse.integrationpattern.process;
using nerve.core.synapse.util;
using nerve.core.util.reader;

namespace nerve.core.synapse.initializer
{
    /// <summary>
    /// SynapseContextInitializer Class.
    /// </summary>
    public class SynapseContextInitializer
    {
        private const string DefaultResourceFilePath = "property.route.route.xml";

        /// <summary>
        /// Get Package Descriptor File
        /// </summary>
        /// <param name="bundleDllPath"></param>
        /// <returns></returns>
        public static string GetPackageDescriptorFile(string bundleDllPath)
        {
            try
            {
                var assemblyName = Path.GetFileNameWithoutExtension(bundleDllPath);
                var routePath = string.Format("{0}.{1}", assemblyName, DefaultResourceFilePath);

                Console.Write("[LoadingPackageResourceFile_Exec] - [{0}]", assemblyName);
                string result;

                var assembly = Assembly.LoadFile(bundleDllPath);
                using (var stream = assembly.GetManifestResourceStream(routePath))
                {
                    if (stream == null)
                        return null;
                    using (var sr = new StreamReader(stream))
                    {
                        result = sr.ReadToEnd();
                    }
                }
                return result;
            }
            catch (Exception exception)
            {
                throw new SynapseException(string.Format("[GetPackageDescriptorFile_Error] - '{0}'", exception.Message), exception);
            }
        }

        /// <summary>
        /// GetDescriptorResourceTextFile
        /// </summary>
        /// <param name="bundleDllPath"></param>
        /// <returns></returns>
        public static PackageDescriptor LoadPackageDescriptorFromDll(string bundleDllPath)
        {
            try
            {
                var assemblyName = Path.GetFileNameWithoutExtension(bundleDllPath);
                var routePath = string.Format("{0}.property.descriptor.xml", assemblyName);

                var assembly = Assembly.LoadFile(bundleDllPath);
                using (var stream = assembly.GetManifestResourceStream(routePath))
                {
                    if (stream == null)
                        return null;

                    using (var sr = new StreamReader(stream))
                    {
                        var data = sr.ReadToEnd();
                        var obj = new PackageDescriptor(data);
                        Console.WriteLine(" version: [{0}], friendly-name: [{1}]", obj.ModelVersion, obj.Name);
                        return obj;
                    }
                }
            }
            catch (Exception exception)
            {
                throw new SynapseException(string.Format("[LoadPackageDescriptorFromDll_Error] - {0}", bundleDllPath), exception);
            }
        }

        /// <summary>
        /// Initialize Route File.
        /// </summary>
        /// <param name="fileInformation"></param>
        /// <param name="isPackage"></param>
        public static void Initialize(string fileInformation, bool isPackage = true)
        {
            try
            {
                //check if file exists
                if (!File.Exists(fileInformation))
                    throw new SynapseException(string.Format("[RouteFileNotFound_Error] - '{0}'", fileInformation));

                XElement routeConfigFile;
                PackageDescriptor packageDescriptor = null;

                //read fileInformation
                if (isPackage)
                {
                    //as package
                    var routeXml = GetPackageDescriptorFile(fileInformation);
                    packageDescriptor = LoadPackageDescriptorFromDll(fileInformation);
                    routeConfigFile = XElement.Parse(routeXml);

                    Console.WriteLine(Environment.NewLine + "({1}) === [ {0} ] ===========================================" + Environment.NewLine, Path.GetFileName(fileInformation), DateTime.Now.ToString(CultureInfo.InvariantCulture));
                    Console.WriteLine(routeXml.ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    //as raw file data
                    try
                    {
                        routeConfigFile = XElement.Parse(fileInformation);
                    }
                    catch (Exception exception)
                    {
                        throw new SynapseException(string.Format("[RouteFileParsingError] - '{0}'", fileInformation), exception);
                    }
                }

                new BeanStepProcessor(routeConfigFile).Run();
                new RouteStepProcessor(routeConfigFile, packageDescriptor: packageDescriptor).Run();
            }
            catch (Exception exception)
            {
                throw new SynapseException(string.Format("[RouteDataError_Unknown] - '{0}'", fileInformation), exception);
            }
        }
    }
}
