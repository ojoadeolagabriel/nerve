﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using nerve.core.synapse.context;
using nerve.core.synapse.dataobjects;

namespace nerve.core.synapse.resolver
{
    public static class SimpleExpression
    {
        public static bool Evaluate(Exchange exchange, string expression)
        {
            try
            {
                var expressionParts = expression.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                var operatorType = expressionParts[1];
                var lhs = expressionParts[0];
                var rhs = expressionParts[2];

                var lhsResult = ResolveSpecifiedUriPart(lhs, exchange);
                var rhsResult = ResolveSpecifiedUriPart(rhs, exchange);
                var result = false;

                switch (operatorType)
                {
                    case "=":
                        result = lhsResult == rhsResult;
                        break;
                    case "!=":
                        result = lhsResult != rhsResult;
                        break;
                    case "<=":
                        result = lhsResult != rhsResult;
                        break;
                    case ">=":
                        result = lhsResult != rhsResult;
                        break;
                }

                if (SynapseContext.LogDebugInformation)
                    Console.WriteLine("Checking: {0} {1} {2}, result => {3}", lhs, operatorType, rhs, result);

                return result;
            }
            catch (Exception exception)
            {
            }

            return false;
        }

        public static Object ReadObjectRecursion(Object mObject, string mProperty, string nextProperty = null)
        {
            while (true)
            {
                var prop = mObject.GetType().GetProperty(mProperty);
                var res = prop.GetValue(mObject, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase, null, null, CultureInfo.CurrentCulture);

                if (string.IsNullOrEmpty(nextProperty))
                    return res;

                var nextPropertyParts = nextProperty.Split(new[] { '.' }, 2);
                var newMProperty = nextPropertyParts[0];
                var newNextProperty = nextPropertyParts.Length > 1 ? nextPropertyParts[1] : null;

                mObject = res;
                mProperty = newMProperty;
                nextProperty = newNextProperty;
            }
        }

        /// <summary>
        /// Resolve Object From Registry
        /// </summary>
        /// <param name="objectExpression"></param>
        /// <returns></returns>
        public static Object ResolveObjectFromRegistry(string objectExpression)
        {
            if (!objectExpression.StartsWith("${") || !objectExpression.EndsWith("}"))
                return objectExpression;

            var mData = objectExpression.Replace("${", "");
            mData = mData.Replace("}", "");

            var mDataParts = mData.Split(new[] { '.' }, 3);
            var objectKey = mDataParts[0];

            var objectData = SynapseContext.Registry[objectKey];
            if (mDataParts.Length == 1)
                return objectData;

            return ReadObjectRecursion(objectData, mDataParts[1], mDataParts.Length == 3 ? mDataParts[2] : null);
        }

        /// <summary>
        /// Resolve path via exchange.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="exchange"></param>
        /// <returns></returns>
        public static string ResolveSpecifiedUriPart(string path, Exchange exchange)
        {
            var matchColl = Regex.Matches(path, @"\${(.*?)\}", RegexOptions.IgnoreCase);

            return matchColl
                .Cast<Match>()
                .Where(match => match.Success).Aggregate(path, (current1, match) => match.Groups.Cast<Group>().Aggregate(current1, (current, @group) => HandleMatch(@group, current, exchange)));
        }

        /// <summary>
        /// Handle match
        /// </summary>
        /// <param name="group"></param>
        /// <param name="componentQueryPath"></param>
        /// <param name="exchange"></param>
        /// <returns></returns>
        private static string HandleMatch(Capture @group, string componentQueryPath, Exchange exchange)
        {
            try
            {
                var originalData = group.Value;

                var mData = group.Value.Replace("${", "");
                mData = mData.Replace("}", "");

                var parts = mData.Split(new[] { ':' });
                if (parts.Length != 2)
                {
                    parts = mData.Split(new[] { '.' });
                }

                var originalObjectKey = parts.Length >= 1 ? parts[0] : "";
                var originalObjectProperty = parts.Length >= 2 ? parts[1] : "";
                var originalObjectPropertyOfProperty = parts.Length >= 3 ? parts[2] : "";

                Object replacementData;

                switch (originalObjectKey)
                {
                    case "in":
                        replacementData = ReadMessageData(exchange.InMessage, originalObjectProperty, originalObjectPropertyOfProperty);
                        break;
                    case "out":
                        replacementData = ReadMessageData(exchange.OutMessage, originalObjectProperty, originalObjectPropertyOfProperty);
                        break;
                    case "property":
                        replacementData = exchange.PropertyCollection[originalObjectPropertyOfProperty];
                        break;
                    case "enum":
                        replacementData = ReadEnumData(originalObjectProperty);
                        break;
                    default:
                        var dataObj = SynapseContext.Registry[originalObjectKey];
                        replacementData = ReadObjectRecursion(dataObj, originalObjectProperty, originalObjectPropertyOfProperty);
                        break;
                }

                //return
                return componentQueryPath.Replace(originalData, replacementData.ToString());
            }
            catch (Exception)
            {
                return componentQueryPath;
            }
        }

        private static object ReadEnumData(string objectDataProperty)
        {
            try
            {
                var parts = objectDataProperty.Split(new[] { '.' });
                var partsLessOne = parts.Take(parts.Length - 1).ToList();
                var last = parts.Last();
                var fullName = "";

                partsLessOne.ForEach(c =>
                {
                    fullName += string.Format("{0}.", c);
                });

                fullName = fullName.Remove(fullName.Length - 1, 1);

                var @enum = GetEnumType(fullName);
                if (@enum != null)
                {
                    var val = (int)Enum.Parse(@enum, last);
                    return val;
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static Type GetEnumType(string name)
        {
            return
             (from assembly in AppDomain.CurrentDomain.GetAssemblies()
              let type = assembly.GetType(name)
              where type != null
                 && type.IsEnum
              select type).FirstOrDefault();
        }

        public static Type GetBean(string name)
        {
            return
             (from assembly in AppDomain.CurrentDomain.GetAssemblies()
              let type = assembly.GetType(name)
              where type != null
                 && type.IsClass
              select type).FirstOrDefault();
        }

        private static object ReadMessageData(Message message, string objectDataProperty, string objectDataKey)
        {
            try
            {
                switch (objectDataProperty)
                {
                    case "header":
                        return message.HeaderCollection[objectDataKey];
                    case "body":
                        return message.Body;
                }
            }
            catch
            {

            }
            return "";
        }
    }
}
