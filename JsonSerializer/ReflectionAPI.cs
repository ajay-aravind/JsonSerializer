using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JsonSerializer
{
    internal class ReflectionAPI: IReflectionAPI
    {
        public object? GetKeyValue(object? item, Type type)
        {
            return type.GetProperty("Value").GetValue(item);
        }

        public object? GetKeyName(object item, Type type)
        {
            return type.GetProperty("Key").GetValue(item);
        }

        public bool IsDictionaryKeyAString(Type[] genericTypes)
        {
            return genericTypes[0].IsPrimitive || genericTypes[0].ToString().Equals("System.String");
        }

        public bool isDictionaryType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);
        }

        public T? GetCustomAttirbuteValue<T>(PropertyInfo prop) where T:Attribute
        {
            return (T)prop.GetCustomAttribute(typeof(T), false);
        }

        public  bool IsAttributeDefinedOnProperty(PropertyInfo prop, Type attributeType)
        {
            return prop.IsDefined(attributeType, false);
        }

        public  PropertyInfo[] GetPropertiesOfType(Type type)
        {
            return type.GetProperties(System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public);
        }

        public object? InvokeMethod(object complexObject, MethodInfo? ShouldSerializeMethod)
        {
            return ShouldSerializeMethod.Invoke(complexObject, null);
        }

        public MethodInfo? GetShouldSerializeMethod(Type type, PropertyInfo prop)
        {
            return type.GetMethod("ShouldSerialize" + prop.Name);
        }

        public MethodInfo[] GetAllMethods(Type type, )
        {
            return type.GetMethods();
        }

        public Type GetHandlerRequestMessageType(MethodInfo method)
        {
            var parameters = method.GetParameters();
            
            if( parameters.Length == 1)
            {
                return parameters[0].ParameterType;
            }
            else
            {
                throw new Exception("Handler should have atleast a single parameter");
            }
        }
    }
}
