using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace CSI.Core.Extensions
{
    public static class EntityObjectExtensions
    {
        public static T ConvertEmptyStringToNull<T>(this T criteria, Expression<Func<T, string>> expression)            
        {
            var member = expression.GetMember();
            var propertyInfo = typeof(T).GetProperty(member.Name);
            if (propertyInfo != null)
            {
                string value = propertyInfo.GetValue(criteria, null) as string;
                if (String.IsNullOrEmpty(value))
                {
                    propertyInfo.SetValue(criteria, null, null);
                }
            }
            return criteria;
        }

        public static T TrimString<T>(this T criteria, Expression<Func<T, string>> expression)
        {
            var member = expression.GetMember();
            var propertyInfo = typeof(T).GetProperty(member.Name);
            if (propertyInfo != null)
            {
                string value = propertyInfo.GetValue(criteria, null) as string;
                if (!String.IsNullOrEmpty(value))
                {
                    propertyInfo.SetValue(criteria, value.Trim(), null);
                }
            }
            return criteria;
        }

        public static T UpperString<T>(this T criteria, Expression<Func<T, string>> expression)
        {
            var member = expression.GetMember();
            var propertyInfo = typeof(T).GetProperty(member.Name);
            if (propertyInfo != null)
            {
                string value = propertyInfo.GetValue(criteria, null) as string;
                if (!String.IsNullOrEmpty(value))
                {
                    propertyInfo.SetValue(criteria, value.ToUpper(), null);
                }
            }
            return criteria;
        }

        public static T ConvertNullOnGreaterThan<T>(this T criteria, Expression<Func<T, decimal?>> expression,decimal maxValue)
        {
            var member = expression.GetMember();
            var propertyInfo = typeof(T).GetProperty(member.Name);
            if (propertyInfo != null)
            {
                decimal? value = propertyInfo.GetValue(criteria, null) as decimal?;
                if (value.HasValue && value.Value > maxValue) 
                {
                    propertyInfo.SetValue(criteria, null, null);
                }                
            }
            return criteria;
        }

        public static T ConvertMinDateToNull<T>(this T model, Expression<Func<T, DateTime?>> expression)
        {
            var member = expression.GetMember();
            var propertyInfo = typeof(T).GetProperty(member.Name);
            if (propertyInfo != null)
            {
                var value = propertyInfo.GetValue(model, null) as DateTime?;
                if (value.HasValue && value.Value == DateTime.MinValue)
                {
                    propertyInfo.SetValue(model, null, null);
                }
            }
            return model;
        }
    }

    
}
