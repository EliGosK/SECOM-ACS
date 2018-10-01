﻿// This is a slightly modified ValueProviderFactory based almost entirely on Microsoft's JsonValueProviderFactory.
// That file is licensed under the Apache 2.0 license, but I am leaving the copyright statement below nonetheless.
// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Web.Mvc;

namespace CSI.Web.Mvc
{
    public sealed class JsonNetValueProviderFactory : ValueProviderFactory
    {
        private static void AddToBackingStore(EntryLimitedDictionary backingStore, string prefix, object value)
        {
            IDictionary<string, object> d = value as IDictionary<string, object>;
            if (d != null)
            {
                foreach (KeyValuePair<string, object> entry in d)
                {
                    AddToBackingStore(backingStore, MakePropertyKey(prefix, entry.Key), entry.Value);
                }
                return;
            }

            IList l = value as IList;
            if (l != null)
            {
                for (int i = 0; i < l.Count; i++)
                {
                    AddToBackingStore(backingStore, MakeArrayKey(prefix, i), l[i]);
                }
                return;
            }

            // primitive
            backingStore.Add(prefix, value);
        }

        private static object GetDeserializedObject(ControllerContext controllerContext)
        {
            if (!controllerContext.HttpContext.Request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
            {
                // not JSON request
                return null;
            }

            TextReader reader = new StreamReader(controllerContext.HttpContext.Request.InputStream);
            string bodyText = reader.ReadToEnd();
            if (String.IsNullOrEmpty(bodyText))
            {
                // no JSON data
                return null;
            }
            //Not sure if I could just cut out a step and use the initial reader, but that would lose
            //the null or empty check.
            reader = new StringReader(bodyText);
            JsonReader jReader = new JsonTextReader(reader);
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new Newtonsoft.Json.Converters.ExpandoObjectConverter());

            object jsonData;
            if (jReader.TokenType == JsonToken.StartArray)
            { jsonData = serializer.Deserialize<List<ExpandoObject>>(jReader); }
            else
            { jsonData = serializer.Deserialize<ExpandoObject>(jReader); }
            //object jsonData = serializer.Deserialize(jReader);
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //object jsonData = serializer.DeserializeObject(bodyText);
            return jsonData;
        }

        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }

            object jsonData = GetDeserializedObject(controllerContext);
            if (jsonData == null)
            {
                return null;
            }

            Dictionary<string, object> backingStore = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            EntryLimitedDictionary backingStoreWrapper = new EntryLimitedDictionary(backingStore);
            AddToBackingStore(backingStoreWrapper, String.Empty, jsonData);
            return new DictionaryValueProvider<object>(backingStore, CultureInfo.CurrentCulture);
        }

        private static string MakeArrayKey(string prefix, int index)
        {
            return prefix + "[" + index.ToString(CultureInfo.InvariantCulture) + "]";
        }

        private static string MakePropertyKey(string prefix, string propertyName)
        {
            return (String.IsNullOrEmpty(prefix)) ? propertyName : prefix + "." + propertyName;
        }

        private class EntryLimitedDictionary
        {
            private static int _maximumDepth = GetMaximumDepth();
            private readonly IDictionary<string, object> _innerDictionary;
            private int _itemCount = 0;

            public EntryLimitedDictionary(IDictionary<string, object> innerDictionary)
            {
                _innerDictionary = innerDictionary;
            }

            public void Add(string key, object value)
            {
                if (++_itemCount > _maximumDepth)
                {
                    throw new InvalidOperationException("Request too large");
                }

                _innerDictionary.Add(key, value);
            }

            private static int GetMaximumDepth()
            {
                NameValueCollection appSettings = ConfigurationManager.AppSettings;
                if (appSettings != null)
                {
                    string[] valueArray = appSettings.GetValues("aspnet:MaxJsonDeserializerMembers");
                    if (valueArray != null && valueArray.Length > 0)
                    {
                        int result;
                        if (Int32.TryParse(valueArray[0], out result))
                        {
                            return result;
                        }
                    }
                }

                return 1000; // Fallback default
            }
        }
    }
}