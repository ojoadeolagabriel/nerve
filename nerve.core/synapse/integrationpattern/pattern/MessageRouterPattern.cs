﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using nerve.core.synapse.context;
using nerve.core.synapse.dataobjects;
using nerve.core.synapse.integrationpattern.process;
using nerve.core.synapse.resolver;

namespace nerve.core.synapse.integrationpattern.pattern
{
    class MessageRouterPattern
    {
        public static void Execute(XElement choicElement, Exchange exchange)
        {
            DoWhen(choicElement, exchange);
        }

        public static void DoWhen(XElement choicElement, Exchange exchange)
        {
            var whenElements = choicElement.Elements("when");
            var passed = false;

            foreach (var whenElement in whenElements)
            {
                passed = CheckRequiremnt(whenElement, exchange);
                if (!passed) continue;

                var functions = whenElement.Elements().Skip(1);
                foreach (var xmlStep in functions)
                {
                    RouteStep.ExecuteRouteStep(xmlStep, exchange.Route, exchange);
                }
                break;
            }

            if (!passed)
            {
                //handle otherwise
                var otherwiseXml = choicElement.Element("otherwise");
                if (otherwiseXml == null) return;

                var otherwiseFunctions = otherwiseXml.Elements();

                foreach (var xmlStep in otherwiseFunctions)
                {
                    RouteStep.ExecuteRouteStep(xmlStep, exchange.Route, exchange);
                }
            }
        }

        private static bool CheckRequiremnt(XElement whenElement, Exchange exchange)
        {
            var conditionXml = whenElement.Elements().FirstOrDefault();
            if (conditionXml == null)
                return false;

            var conditionType = conditionXml.Name.ToString();
            switch (conditionType)
            {
                case "simple":
                    return ProcessSimple(conditionXml, exchange);
                    break;
                case "xpath":
                    return ProcessXPath(conditionXml, exchange);
                    break;
                case "method":
                    return ProcessBean(conditionXml, exchange);
                    break;
                case "processor-method":
                    return ProcessExchangeBean(conditionXml, exchange);
                    break;
                default:
                    return false;
            }

            return false;

        }

        private static bool ProcessBean(XElement conditionXml, Exchange exchange)
        {
            if (conditionXml == null)
                return false;

            var bean = conditionXml.Attribute("bean");
            var method = conditionXml.Attribute("method");
            if (bean == null || method == null)
                return false;

            var beanObj = SynapseContext.Registry[bean.Value];
            var methodInst = beanObj.GetType().GetMethod(method.Value, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (methodInst == null)
                return false;

            var result = methodInst.Invoke(beanObj, null);
            return Convert.ToBoolean(result);
        }

        private static bool ProcessExchangeBean(XElement conditionXml, Exchange exchange)
        {
            if (conditionXml == null)
                return false;

            var bean = conditionXml.Attribute("bean");
            var method = conditionXml.Attribute("method");
            if (bean == null || method == null)
                return false;

            var beanObj = SynapseContext.Registry[bean.Value];
            var methodInst = beanObj.GetType().GetMethod(method.Value, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (methodInst == null)
                return false;

            var result = methodInst.Invoke(beanObj, new object[] { exchange });
            return Convert.ToBoolean(result);
        }

        private static bool ProcessXPath(XElement conditionXml, Exchange exchange)
        {
            var rule = conditionXml.Value;
            var message = exchange.InMessage.Body.ToString();

            var xml = XElement.Parse(message);
            var isMatch = (bool)xml.XPathEvaluate(rule);

            return isMatch;
        }

        private static bool ProcessSimple(XElement conditionXml, Exchange exchange)
        {
            return SimpleExpression.Evaluate(exchange, conditionXml.Value);
        }
    }
}
