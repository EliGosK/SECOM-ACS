using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace CSI.Core.Extensions
{
    public static class ExpressionExtensions
    {
        internal static void Guard(this object obj, string message)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(message);
            }
        }

        internal static void Guard(this string str, string message)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentNullException(message);
            }
        }

        /// <summary>
        /// Gets a MemberInfo from a member expression.
        /// </summary>
        public static MemberInfo GetMember(this LambdaExpression expression)
        {
            var memberExp = RemoveUnary(expression.Body);

            if (memberExp == null)
            {
                return null;
            }

            return memberExp.Member;
        }

        /// <summary>
        /// Gets a MemberInfo from a member expression.
        /// </summary>
        public static MemberInfo GetMember<T, TProperty>(this Expression<Func<T, TProperty>> expression)
        {
            var memberExp = RemoveUnary(expression.Body);

            if (memberExp == null)
            {
                return null;
            }

            return memberExp.Member;
        }

        private static MemberExpression RemoveUnary(Expression toUnwrap)
        {
            if (toUnwrap is UnaryExpression)
            {
                return ((UnaryExpression)toUnwrap).Operand as MemberExpression;
            }

            return toUnwrap as MemberExpression;
        }


        /// <summary>
        /// Splits pascal case, so "FooBar" would become "Foo Bar"
        /// </summary>
        public static string SplitPascalCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            return Regex.Replace(input, "([A-Z])", " $1").Trim();
        }
        /// <summary>
        /// Helper method to construct a constant expression from a constant.
        /// </summary>
        /// <typeparam name="T">Type of object being validated</typeparam>
        /// <typeparam name="TProperty">Type of property being validated</typeparam>
        /// <param name="valueToCompare">The value being compared</param>
        /// <returns></returns>
        internal static Expression<Func<T, TProperty>> GetConstantExpresionFromConstant<T, TProperty>(TProperty valueToCompare)
        {
            Expression constant = Expression.Constant(valueToCompare, typeof(TProperty));
            ParameterExpression parameter = Expression.Parameter(typeof(T), "t");
            return Expression.Lambda<Func<T, TProperty>>(constant, parameter);
        }

        internal static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        public static Func<object, object> CoerceToNonGeneric<T, TProperty>(this Func<T, TProperty> func)
        {
            return x => func((T)x);
        }

        public static Func<object, bool> CoerceToNonGeneric<T>(this Func<T, bool> func)
        {
            return x => func((T)x);
        }

        public static Action<object> CoerceToNonGeneric<T>(this Action<T> action)
        {
            return x => action((T)x);
        }
    }
}
