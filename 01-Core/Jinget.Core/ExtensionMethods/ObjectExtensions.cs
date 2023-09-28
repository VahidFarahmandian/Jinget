using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Jinget.Core.Exceptions;
using Jinget.Core.ExpressionToSql.Internal;
using Jinget.Core.ExtensionMethods.Reflection;
using Newtonsoft.Json;

namespace Jinget.Core.ExtensionMethods
{
    public static class ObjectExtensions
    {
        public class Options
        {
            public Options(bool ignoreNull = true, bool ignoreExpressions = true)
            {
                IgnoreNull = ignoreNull;
                IgnoreExpressions = ignoreExpressions;
            }
            public bool IgnoreNull { get; set; }
            public bool IgnoreExpressions { get; set; }
            public bool IgnoreExpr2SQLPagings { get; set; }
            public bool IgnoreExpr2SQLOrderBys { get; set; }
        }


        /// <summary>
        /// Check if given type is numeric or not
        /// </summary>
        public static bool IsNumericType(this object o) => Type.GetTypeCode(o.GetType()) switch
        {
            TypeCode.Byte or
            TypeCode.SByte or
            TypeCode.UInt16 or
            TypeCode.UInt32 or
            TypeCode.UInt64 or
            TypeCode.Int16 or
            TypeCode.Int32 or
            TypeCode.Int64 or
            TypeCode.Decimal or
            TypeCode.Double or
            TypeCode.Single => true,
            _ => false,
        };

        /// <summary>
        /// Convert source object to dictionary(key=property name, value=property value)
        /// </summary>
        public static Dictionary<string, object> ToDictionary(this object source, Options? options = null)
        {
            options ??= new Options();

            Dictionary<string, object> result = new();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
            {
                object value = property.GetValue(source);
                if (options.IgnoreNull && value == null)
                    continue;
                else if (options.IgnoreExpressions && value is Expression)
                    continue;
                else if (options.IgnoreExpr2SQLOrderBys && value is List<OrderBy>)
                    continue;
                else if (options.IgnoreExpr2SQLPagings && value is Paging)
                    continue;
                string key = property.Name;
                if (!result.ContainsKey(key))
                {
                    result.Add(key, value);
                }
            }

            return result;
        }

        /// <summary>
        /// Get the value of the given property
        /// </summary>
        public static object GetValue(this object obj, string propertyName)
            => obj.GetType().GetProperty(propertyName).GetValue(obj);

        /// <summary>
        /// Convert two unrelated objects to each other.
        /// </summary>
        public static T ToType<T>(this object obj, bool suppressError = false) where T : class
        {
            Type type = typeof(T);

            T newObj = type.InvokeDefaultConstructor<T>();

            if (obj == null)
                return newObj;

            foreach (PropertyInfo pi in obj.GetType().GetProperties().Where(x => x.Name != "Empty" && x.CanWrite))
            {
                var property = type.GetProperty(pi.Name);

                if (property == null)
                {
                    if (suppressError)
                        continue;
                    throw new JingetException($"Jinget Says: Model {typeof(T).Name} does not contain property with the name {pi.Name}", 1000);
                }

                if (pi.PropertyType.IsValueType || pi.PropertyType == typeof(string))
                {
                    property.SetValue(newObj, pi.GetValue(obj, null));
                }
                else
                {
                    var value = pi.GetValue(obj);

                    if (!property.PropertyType.IsGenericType || property.PropertyType.GetGenericTypeDefinition() != typeof(ICollection<>))
                    {
                        var referenceTypeValue = typeof(ObjectExtensions).Call(
                            name: nameof(ToType),
                            parameters: new[] { value },
                            generics: property.PropertyType);

                        property.SetValue(newObj, referenceTypeValue);
                    }
                    else
                    {
                        if (value == null)
                            continue;

                        foreach (var
                            item in (IEnumerable)value)
                        {
                            var referenceTypeValue = typeof(ObjectExtensions).Call(
                                name: nameof(ToType),
                                parameters: new[] { item },
                                generics: property.PropertyType.GetGenericArguments()[0]);

                            if (property.PropertyType.GetGenericTypeDefinition() != typeof(ICollection<>))
                            {
                                property.SetValue(newObj, referenceTypeValue);
                            }
                            else
                            {
                                var field = type.GetField($"<{property.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);

                                if (field == null)
                                {
                                    field = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                                        .FirstOrDefault(x => string.Equals(x.Name, "_" + property.Name,
                                            StringComparison.OrdinalIgnoreCase));

                                    if (field == null)
                                    {
                                        throw new JingetException($"Jinget Says: Unable to find backing field for Property '{property.Name}'", 1000);
                                    }
                                }

                                if (typeof(IEnumerable).IsAssignableFrom(field.FieldType))
                                {
                                    if (field.GetValue(newObj) == null)//init the list
                                    {
                                        var instance = typeof(List<>)
                                            .MakeGenericType(property.PropertyType.GetGenericArguments()[0])
                                            .GetConstructor(Type.EmptyTypes)
                                            .Invoke(null);

                                        field.SetValue(newObj, instance);
                                    }

                                    field.FieldType.GetMethod("Add").Invoke(field.GetValue(newObj), new object[] { referenceTypeValue });
                                }
                                else
                                {
                                    field.SetValue(newObj, referenceTypeValue);
                                }
                            }
                        }
                    }
                }
            }
            return newObj;
        }

        /// <summary>
        /// check if two objects(from same/different types) have exactly same structure and values
        /// </summary>
        public static bool HasSameValuesAs(this object source, object target)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };

            return JsonConvert.SerializeObject(source, settings) == JsonConvert.SerializeObject(target, settings);
        }
    }
}