using System;
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace ExcelAsSQLdataSourse.Extensions
{
    public static class Extension
    {
        private static int? GetMinLength<T>(Expression<Func<T, string>> propertyExpression)
        {
            int? result = GetPropertyAttributeValue<T, string, StringLengthAttribute, int?>
                    (propertyExpression, attr => attr.MinimumLength);
            return result;
        }

        public static int? GetMinLength<T>(this T instance, Expression<Func<T, string>> propertyExpression)
        {
            return GetMinLength<T>(propertyExpression);
        }

        private static int? GetMaxLength<T>(Expression<Func<T, string>> propertyExpression)
        {
            int? result = GetPropertyAttributeValue<T, string, StringLengthAttribute, int?>
                    (propertyExpression, attr => attr.MaximumLength);
            return result;
        }   

        public static int? GetMaxLength<T>(this T instance, Expression<Func<T, string>> propertyExpression)
        {
            return GetMaxLength<T>(propertyExpression);
        }

        private static double? GetMinRange<T>(Expression<Func<T, double?>> propertyExpression)
        {
            double? result = GetPropertyAttributeValue<T, double?, RangeAttribute, double?>
                    (propertyExpression, attr => (double?)attr.Minimum);
            return result;
        }

        public static double? GetMinRange<T>(this T instance, Expression<Func<T, double?>> propertyExpression)
        {
            return GetMinRange<T>(propertyExpression);
        }

        private static double? GetMaxRange<T>(Expression<Func<T, double?>> propertyExpression)
        {
            double? result = GetPropertyAttributeValue<T, double?, RangeAttribute, double?>
                    (propertyExpression, attr => (double?)attr.Maximum);
            return result;
        }

        public static double? GetMaxRange<T>(this T instance, Expression<Func<T, double?>> propertyExpression)
        {
            return GetMaxRange<T>(propertyExpression);
        }

        private static TValue GetPropertyAttributeValue<T, TOut, TAttribute, TValue>
            (Expression<Func<T, TOut>> propertyExpression, Func<TAttribute, TValue> valueSelector) where TAttribute : Attribute
        {
            var expression = (MemberExpression)propertyExpression.Body;
            string mName = expression.Member.Name;
            Type type = typeof(T);
            MemberInfo member = type.GetMember(mName).FirstOrDefault();
            var attr = member.GetCustomAttribute<TAttribute>(inherit: true);
            if (attr != null)
            {
                return valueSelector(attr);
            }
            else
            {
                var mdTypeAttr = (MetadataTypeAttribute)type.GetCustomAttribute<MetadataTypeAttribute>(inherit: true);
                type = mdTypeAttr.MetadataClassType;
                member = type.GetMember(mName).FirstOrDefault();
                attr = member.GetCustomAttribute<TAttribute>(inherit: true);
                return (attr == null ? default(TValue) : valueSelector(attr));
            }
        }
    }
}