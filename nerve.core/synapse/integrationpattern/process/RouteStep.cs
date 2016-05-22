using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using log4net.Util;
using nerve.core.synapse.context;
using nerve.core.synapse.dataobjects;
using nerve.core.synapse.integrationpattern.pattern;
using nerve.core.synapse.resolver;
using nerve.core.synapse.tag;
using nerve.core.synapse.util.synapseconstant;

namespace nerve.core.synapse.integrationpattern.process
{
    public class RouteStep
    {
        public RouteStep(XElement currentStepXml, Route routeObj)
        {
            _currentStepXml = currentStepXml;
            Route = routeObj;
            CheckIfRouteNameOverrideRequired();
        }

        /// <summary>
        /// Check If RouteName Override Required
        /// </summary>
        private void CheckIfRouteNameOverrideRequired()
        {
            try
            {
                var stepName = _currentStepXml.Name.ToString();
                switch (stepName)
                {
                    case TagConstant.FromTag:

                        var val = _currentStepXml.Attribute(TagConstant.Uri).Value;
                        var desc = UriDescriptor.Parse(val);

                        if (desc.ComponentName == TagConstant.Direct)
                        {
                            Route.RouteId = desc.ComponentPath;
                        }

                        if (desc.ComponentName == TagConstant.Seda)
                        {
                            Route.RouteId = desc.ComponentPath;
                        }
                        break;
                }
            }
            catch (Exception exception)
            {
                var msg = exception.Message;
            }
        }

        public XElement XmlRaw { get { return _currentStepXml; } }

        public Route Route { get; set; }

        private readonly XElement _currentStepXml;
        public RouteStep NextRouteStep { get; set; }

        /// <summary>
        /// Process channel
        /// </summary>
        /// <param name="exchange"></param>
        public void ProcessChannel(Exchange exchange = null)
        {
            ExecuteRouteStep(_currentStepXml, Route, exchange);

            if (NextRouteStep == null || exchange == null) return;

            switch (exchange.Route.PipelineMode)
            {
                case Route.MessagePipelineMode.Default:
                    NextRouteStep.ProcessChannel(exchange);
                    break;
            }
        }

        /// <summary>
        /// Process Step
        /// </summary>
        /// <param name="step"></param>
        /// <param name="routeObj"></param>
        /// <param name="exchange"></param>
        public static void ExecuteRouteStep(XElement step, Route routeObj, Exchange exchange)
        {
            try
            {
                var stepname = step.Name.ToString().ToLower();

                switch (stepname)
                {
                    case TagConstant.FromTag:
                        HandleFromProcessor(step, routeObj, exchange);
                        break;
                    case TagConstant.ToTag:
                        HandleToProcessor(step, routeObj, exchange);
                        break;
                    case TagConstant.Enrich:
                        HandleToEnricher(step, routeObj, exchange);
                        break;
                    case TagConstant.Split:
                        HandleSplit(step, exchange);
                        break;
                    case TagConstant.Multicast:
                        HandleMulticast(step, exchange);
                        break;
                    case TagConstant.Process:
                        HandleProcessor(step, routeObj, exchange);
                        break;
                    case TagConstant.Bean:
                        HandleBean(step, routeObj, exchange);
                        break;
                    case TagConstant.Convertbodyto:
                        HandleConvertBodyTo(step, routeObj, exchange);
                        break;
                    case TagConstant.Setheader:
                        HandleSetHeader(step, routeObj, exchange);
                        break;
                    case TagConstant.Removeheader:
                        HandleRemoveHeader(step, routeObj, exchange);
                        break;
                    case TagConstant.Setproperty:
                        HandleProperty(step, routeObj, exchange);
                        break;
                    case TagConstant.Choice:
                        HandleChoice(step, routeObj, exchange);
                        break;
                    case TagConstant.Loop:
                        HandleLoop(step, routeObj, exchange);
                        break;
                    case TagConstant.Delay:
                        HandleDelay(step, routeObj, exchange);
                        break;
                    case TagConstant.Wiretap:
                        HandleWireTap(step, routeObj, exchange);
                        break;
                    case TagConstant.Transform:
                        HandleTransform(step, routeObj, exchange);
                        break;
                    case TagConstant.Filter:
                        HandleFilter(step, routeObj, exchange);
                        break;
                }
            }
            catch (Exception exception)
            {
                var msg = exception.Message;
            }
        }

        private static void HandleToEnricher(XElement step, Route routeObj, Exchange exchange)
        {
            EnricherPattern.Enrich(step, exchange, routeObj);
        }

        private static void HandleLoop(XElement step, Route routeObj, Exchange exchange)
        {
            LoopTag.Execute(step, exchange, routeObj);
        }

        private static void HandleMulticast(XElement step, Exchange exchange)
        {
            PublishSubscribePattern.Process(step, exchange);
        }

        private static void HandleSplit(XElement step, Exchange exchange)
        {
            SplitterPattern.Execute(step, exchange);
        }

        private static void HandleFilter(XElement step, Route routeObj, Exchange exchange)
        {

        }

        private static void HandleRemoveHeader(XElement step, Route routeObj, Exchange exchange)
        {
            var headerName = step.Attribute(TagConstant.Headername).Value;
            object removedValue;
            exchange.InMessage.HeaderCollection.TryRemove(headerName, out removedValue);
        }

        private static void HandleTransform(XElement step, Route routeObj, Exchange exchange)
        {
            TransformTag.HandleTransform(step, routeObj, exchange);
        }

        private static void HandleWireTap(XElement step, Route routeObj, Exchange exchange)
        {
            try
            {
                var fullUri = step.Attribute(TagConstant.Uri).Value;
                WireTapPattern.Execute(exchange, fullUri, routeObj);
            }
            catch (AggregateException agrxc)
            {

            }catch (Exception exc)
            {

            }
        }

        private static void HandleDelay(XElement step, Route routeObj, Exchange exchange)
        {
            var delay = step.Attribute(TagConstant.Value).Value;
            DelayTag.Execute(delay, exchange, null);
        }

        private static void HandleChoice(XElement step, Route routeObj, Exchange exchange)
        {
            MessageRouterPattern.Execute(step, exchange);
        }

        private static void HandleProperty(XElement step, Route routeObj, Exchange exchange)
        {
            var propertyName = step.Attribute(TagConstant.Name).Value;
            var value = step.Attribute(TagConstant.Value).Value;

            value = SimpleExpression.ResolveSpecifiedUriPart(value, exchange);
            exchange.SetProperty(propertyName, value);
        }

        private static void HandleSetHeader(XElement step, Route routeObj, Exchange exchange)
        {
            var headerName = step.Attribute(TagConstant.Name).Value;
            var value = step.Attribute(TagConstant.Value).Value;

            value = SimpleExpression.ResolveSpecifiedUriPart(value, exchange);
            exchange.InMessage.SetHeader(headerName, value);
        }

        private static void HandleConvertBodyTo(XElement step, Route routeObj, Exchange exchange)
        {
            try
            {
                var toType = step.Attribute(TagConstant.Type).Value;
                var finalType = Type.GetType(toType);

                if (finalType != null)
                    exchange.InMessage.Body = Convert.ChangeType(exchange.InMessage.Body, finalType);
            }
            catch
            {

            }
        }

        /// <summary>
        /// ProcessRouteInformation Bean
        /// </summary>
        /// <param name="step"></param>
        /// <param name="routeObj"></param>
        /// <param name="exchange"></param>
        private static void HandleBean(XElement step, Route routeObj, Exchange exchange)
        {
            try
            {
                var reference = step.Attribute(TagConstant.Reference).Value;
                var method = step.Attribute(TagConstant.Method).Value;

                var bean = SynapseContext.Registry[reference];
                var methodInfo = bean.GetType().GetMethod(method);

                methodInfo.Invoke(bean, null);
            }
            catch (Exception)
            {

            }
        }

        private static void HandleProcessor(XElement step, Route routeObj, Exchange exchange)
        {
            try
            {
                var attr = step.Attribute(TagConstant.Reference);
                var reference = attr.Value;
                var bean = SynapseContext.Registry[reference] as ProcessorBase;

                if (bean != null)
                    bean.Process(exchange);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        private static void HandleToProcessor(XElement step, Route routeObj, Exchange exchange)
        {
            var uri = step.Attribute(TagConstant.Uri).Value;
            ToTag.Execute(uri, exchange, routeObj);
        }

        private static void HandleFromProcessor(XElement step, Route routeObj, Exchange exchange)
        {
            var uri = step.Attribute(TagConstant.Uri).Value;
            FromTag.Execute(uri, exchange, routeObj);
        }
    }
}
