using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JsonSerializer
{
    public interface IReflectionAPI
    {
        object? GetKeyValue(object? item, Type type);

        object? GetKeyName(object item, Type type);

        bool IsDictionaryKeyAString(Type[] genericTypes);

        bool isDictionaryType(Type type);

        T? GetCustomAttirbuteValue<T>(PropertyInfo prop) where T : Attribute;

        bool IsAttributeDefinedOnProperty(PropertyInfo prop, Type type);

        PropertyInfo[] GetPropertiesOfType(Type type);

        object? InvokeMethod(object complexObject, MethodInfo? ShouldSerializeMethod);

        MethodInfo? GetShouldSerializeMethod(Type type, PropertyInfo prop);

        MethodInfo[] GetAllMethods(Type type);

        Type GetHandlerRequestMessageType(MethodInfo method);
    }
}
