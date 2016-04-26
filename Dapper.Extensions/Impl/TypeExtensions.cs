﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Dapper
{
    static class TypeExtension
    {
        public static bool IsSimpleType(this Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            var simpleTypesList = new List<Type>
            {
                typeof(byte),
                typeof(sbyte),
                typeof(short),
                typeof(ushort),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong),
                typeof(float),
                typeof(double),
                typeof(decimal),
                typeof(bool),
                typeof(string),
                typeof(char),
                typeof(Guid),
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(byte[])
            };

            return simpleTypesList.Contains(type) || type.IsEnum;
        }

        public static TAttribute GetAttribute<TAttribute>(this Type type) where TAttribute : class
        {
            var attributeName = typeof(TAttribute).Name;

            return type.GetCustomAttributes(true).SingleOrDefault(attr => attr.GetType().Name == attributeName) as TAttribute;
        }

        public static bool HasAttribute<TAttribute>(this Type type)
        {
            var attributeName = typeof(TAttribute).Name;

            return type.GetCustomAttributes(true).Any(attr => attr.GetType().Name == attributeName);
        }

        public static IEnumerable<PropertyInfo> GetPropertiesWithAttribute<TAttribute>(this Type type)
        {
            return type.GetProperties().Where(p => p.HasAttribute<TAttribute>());
        }

        public static IEnumerable<PropertyInfo> GetProperties(this Type type, Func<PropertyInfo, bool> predicate)
        {
            return type.GetProperties().Where(predicate);
        }

        public static IEnumerable<PropertyInfo> GetEditableProperties(this Type type)
        {
            var properties = type.GetPropertiesWithAttribute<EditableAttribute>();

            properties = properties.Where(a => a.IsEditable());

            properties = properties.Where(a => !a.IsReadOnly());

            properties = properties.Where(a => !a.IsKey());

            properties = properties.Where(a => a.IsMapped());

            return properties;
        }
    }
}
