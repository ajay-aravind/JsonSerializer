using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JsonSerializer
{
    /// <summary>
    /// features so far:
    /// 1. handling dictionaries
    /// 2. support for jsonignore and jsonNameProperties
    /// 3. support for ShouldSerialize{property} methods to control serialization
    /// 4. limit depth
    /// 5. detect cycle references and throw
    /// 6. [todo]fix printings
    /// 7. Adding jsonIgnoreCondition attribute support
    /// 8. Add support for custom json converters
    /// 9. Separate writing to the string, as there can be multiple cultures that we need to write
    /// 10. Separate logic that depends on the reflection API, as it can change
    /// </summary>
    internal class JsonSerializer
    {
        private JsonSerializerSettings settings;
        private JsonSerializerFormatter formatter;
        private HashSet<Object> seenObjects;
        private JsonWriter jsonWriter;

        public JsonSerializer(JsonSerializerSettings settings, JsonSerializerFormatter formatter)
        {
            this.settings = settings;
            this.formatter = formatter;
        }
        public JsonSerializer() 
        {
            this.settings = new JsonSerializerSettings();
        }

        public string Serialize(Object inputObj)
        {
            this.seenObjects = new HashSet<object>();
            this.jsonWriter = new JsonWriter();
            this.SerializeInternal(inputObj, 0);
            return jsonWriter.ResultJson;
        }

        public void SerializeInternal(Object inputObj, int depthLevel)
            {
            if(inputObj == null)
            {
                jsonWriter.WriteNull();
                return;
            }

            if(inputObj is string)
            {
                jsonWriter.WriteStringValue(inputObj.ToString());
                return;
            }

            var type = inputObj.GetType();

            if (type.IsPrimitive)
            {
                jsonWriter.WriteStringValue(inputObj.ToString().ToLower());
                return;
            }

            if(typeof(IEnumerable).IsAssignableFrom(type))
            {
                this.SerializeEnumerable(inputObj, depthLevel);
                return;
            }

            if (settings.customConverters.ContainsKey(type))
            {
                ICustomJsonConverter converter = settings.customConverters.GetValueOrDefault(type);
                converter.WriteJson(inputObj);
                return;
            }
            else
            {
                SerializeComplexObject(inputObj, depthLevel);
                return;
            }
        }

        private void SerializeEnumerable(Object inputObject, int depthLevel)
        {
            IReflectionAPI reflectionAPI = new ReflectionAPI();

            if(depthLevel > this.settings.depthLimit)
            {
                throw new InvalidOperationException("Max depth limit reached");
            }

            if (this.seenObjects.Contains(inputObject))
            {
                throw new InvalidOperationException("Cycling reference found. Already seen object: "+ inputObject.GetType().ToString()+ ":"+ inputObject.ToString());
            }
            
            depthLevel++;
            IEnumerable inputCollection = (IEnumerable)inputObject;
            this.jsonWriter.WriteStringAsIs("[");

            foreach(var item in inputCollection)
            {
                var type = item.GetType();

                if (reflectionAPI.isDictionaryType(type))
                {
                    if (reflectionAPI.IsDictionaryKeyAString(type.GetGenericArguments()))
                    {
                        var key = reflectionAPI.GetKeyName(item, type);
                        object? value = reflectionAPI.GetKeyValue(item, type);
                        this.jsonWriter.WriteStringAsIs("\"" + key.ToString() + "\" : ");
                        this.SerializeInternal(value, depthLevel);
                    }
                    else
                    {
                        this.SerializeInternal(item, depthLevel);
                    }
                }
                else
                {
                    this.SerializeInternal(item, depthLevel);
                }
                this.jsonWriter.WriteStringAsIs(",");
            }
            this.jsonWriter.WriteStringAsIs("]");
            this.seenObjects.Add(inputObject);
        }

        private void SerializeComplexObject(Object complexObject, int depthLevel)
        {
            IReflectionAPI reflectionAPI = new ReflectionAPI();
            if (depthLevel > this.settings.depthLimit)
            {
                throw new InvalidOperationException("Max depth limit reached");
            }

            if (this.seenObjects.Contains(complexObject))
            {
                throw new InvalidOperationException("Cycling reference found. Already seen object: " + complexObject.GetType().ToString() + ":" + complexObject.ToString());
            }

            Type type = complexObject.GetType();
            PropertyInfo[] listOfProps = reflectionAPI.GetPropertiesOfType(type);
            depthLevel++;
            jsonWriter.WriteStringAsIs("{\n");

            foreach (var prop in listOfProps)
            {
                if (ShouldSerializeProperty(type, prop, complexObject))
                {

                    string keyName = prop.Name;

                    // JsonPropertyName Attribute support
                    if (reflectionAPI.IsAttributeDefinedOnProperty(prop, typeof(JsonPropertyNameAttribute)))
                    {
                        var nameProperty = reflectionAPI.GetCustomAttirbuteValue<JsonPropertyNameAttribute>(prop);
                        keyName = nameProperty.Name;

                    }

                    jsonWriter.WriteStringAsIs("\"");
                    jsonWriter.WriteStringAsIs(keyName);
                    jsonWriter.WriteStringAsIs("\" : ");
                    this.SerializeInternal(prop.GetValue(complexObject), depthLevel);
                    jsonWriter.WriteStringAsIs(",\n");
                }
            }
            this.seenObjects.Add(complexObject);

            //return this.AddDepthToString("{\n", depthLevel-1) + this.AddDepthToString(serializedString.ToString(), depthLevel) + this.AddDepthToString("}", depthLevel -1);
            jsonWriter.WriteStringAsIs("\n}");
        }

        private string AddDepthToString(String serializedString, int depthLevel)
        {
            string[] stringParts = serializedString.Split("\n");
            string resultString = "";
            foreach(string line in stringParts)
            {
                string depthString = "";
                for(int i = 0; i < depthLevel; i++)
                {
                    depthString += "  ";
                }
                resultString = resultString + depthString + line + "\n";
            }
            return resultString;
        }

        private bool ShouldSerializeProperty(Type type, PropertyInfo prop, Object complexObject)
        {
            IReflectionAPI reflectionAPI = new ReflectionAPI();

            // ShouldSerialize{Name} method support
            var ShouldSerializeMethod = reflectionAPI.GetShouldSerializeMethod(type, prop);
            if (ShouldSerializeMethod != null)
            {
                object? result = reflectionAPI.InvokeMethod(complexObject, ShouldSerializeMethod);
                if (result != null)
                {
                    bool shouldSerialize = (bool)result;
                    return shouldSerialize;
                }

            }

            // JsonIgnore Attribute support
            if (reflectionAPI.IsAttributeDefinedOnProperty(prop, typeof(JsonIgnoreAttribute)))
            {
                return false;
            }

            return true;
        }
    }

    internal class JsonSerializerSettings 
    {
        internal readonly int depthLimit;
        internal readonly Dictionary<Type, ICustomJsonConverter> customConverters;

        public JsonSerializerSettings(int depthLimit, Dictionary<Type, ICustomJsonConverter> customConverters)
        {
            this.depthLimit = depthLimit;
            this.customConverters = customConverters;
        }

        public JsonSerializerSettings()
        {
            this.depthLimit = 12;
            this.customConverters = new Dictionary<Type, ICustomJsonConverter>();
        }
    }

    internal class JsonSerializerFormatter
    {

    }
}
