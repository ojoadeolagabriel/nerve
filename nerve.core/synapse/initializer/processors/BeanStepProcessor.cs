using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using nerve.core.synapse.context;
using nerve.core.synapse.initializer.processors;
using nerve.core.synapse.resolver;

namespace nerve.core.synapse.initializer.processors
{
    public class BeanStepProcessor
    {
        private readonly XElement _routeXml;

        public BeanStepProcessor(XElement routeXml)
        {
            _routeXml = routeXml;
        }

        public void Run()
        {
            var beansXml = _routeXml.Elements("bean");
            var xElements = beansXml as XElement[] ?? beansXml.ToArray();
            if (!xElements.Any()) return;

            foreach (var beanXml in xElements)
            {
                try
                {
                    var id = beanXml.Attributes("id").First().Value;
                    var @class = beanXml.Attributes("class").First().Value;

                    var type = SimpleExpression.GetBean(@class);
                    if (type == null) continue;
                    object bean = null;

                    var constrArgs = beanXml.Elements("const-arg").Elements("index");
                    var enumerable = constrArgs as XElement[] ?? constrArgs.ToArray();

                    if (!enumerable.Any())
                        bean = Activator.CreateInstance(type);
                    else
                    {
                        var @params = type.GetConstructors()[0].GetParameters();
                        var args = new List<object>();
                        var xmlConstrArgs = enumerable.ToList();

                        @params.ToList().ForEach(c =>
                        {
                            try
                            {
                                var paramType = c.ParameterType;
                                var xmlIndex = xmlConstrArgs[c.Position];
                                var paramValObj = xmlIndex.Attribute("value").Value;
                                var argObj = Convert.ChangeType(paramValObj, paramType);
                                args.Add(argObj);
                            }
                            catch (Exception exc)
                            {
                                var msg = exc.Message;
                                Console.WriteLine("{0} => {1}", msg, exc.StackTrace);
                            }
                        });

                        //init bean
                        try
                        {
                            bean = Activator.CreateInstance(
                                               type,
                                               BindingFlags.Public | BindingFlags.Instance,
                                               default(Binder),
                                               args.ToArray(),
                                               default(CultureInfo));
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    //process product.
                    IList<XElement> xmlColl = null;

                    try
                    {
                        var propertyXmlColl = beanXml.Elements("propertys").Elements("property");
                        xmlColl = propertyXmlColl as IList<XElement> ?? propertyXmlColl.ToList();
                    }
                    catch (Exception)
                    { }

                    if (xmlColl.Any())
                    {
                        foreach (var item in xmlColl)
                        {
                            string key = "", @value = "";

                            try
                            {
                                key = item.Attribute("key").Value;
                                @value = item.Attribute("value").Value;

                                var prop = bean.GetType().GetProperty(key);
                                if (prop == null) continue;

                                //does rubbish.. change
                                var res = SimpleExpression.ResolveObjectFromRegistry(@value);
                                prop.SetValue(bean, Convert.ChangeType(res, prop.PropertyType), null);
                            }
                            catch (Exception exc)
                            {
                                var msg = exc.Message + string.Format(" [{0}] - [{1}] ", key, @value);
                                Console.WriteLine("{0} => {1}", msg, exc.StackTrace);
                            }
                        }
                    }

                    SynapseContext.Registry.TryAdd(id, bean);
                }
                catch (Exception exc)
                {
                    var msg = exc.Message;
                    Console.WriteLine("{0} => {1}", msg, exc.StackTrace);
                }
            }
        }
    }
}
