//
// Copyright (c) 2012 Joe Modjeski
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
// associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial 
// portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
// LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
namespace ModjeskiNet
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    public static partial class IndependentIQueryableExtensions
    {
        public static IQueryable<T> Include<T, TProperty>(this IQueryable<T> query, Expression<Func<T, TProperty>> pathExpression)
            where T : class
        {
            var path = (string)null;
            if (TryParsePath(pathExpression, out path))
            {
                return query.Include(path);
            }

            throw new ArgumentException("Invalid path expression: " + pathExpression.ToString(), "path");
        }

        public static IQueryable<T> Include<T>(this IQueryable<T> query, string path)
            where T : class
        {
            return query.CoreInclude(path);
        }

        public static IQueryable Include(this IQueryable query, string path)
        {
            return query.CoreInclude(path);
        }

        private static T CoreInclude<T>(this T source, string path)
            where T : IQueryable
        {
            var method = source.GetType().GetMethod("Include", new[] { typeof(string) });

            if ((method != null) && typeof(T).IsAssignableFrom(method.ReturnType))
            {
                return (T)method.Invoke(source, new object[] { path });
            }

            return source;
        }

        internal static bool TryParsePath(this Expression expression, out string path)
        {
            path = null;

            // Property Accessor - o.Child
            if (expression is MemberExpression)
            {
                return TryParsePath((MemberExpression)expression, out path);
            }
            else if (expression is MethodCallExpression)
            {
                return TryParsePath((MethodCallExpression)expression, out path);
            }
            else if (expression is LambdaExpression)
            {
                return TryParsePath(((LambdaExpression)expression).Body, out path);
            }

            return true;
        }

        private static bool TryParsePath(MemberExpression expression, out string path)
        {
            var part = String.Empty;
            string name = expression.Member.Name;
            TryParsePath(expression.Expression, out part);
            path = String.IsNullOrWhiteSpace(part) ? name : (part + "." + name);
            return true;
        }

        private static bool TryParsePath(MethodCallExpression expression, out string path)
        {
            if (expression != null)
            {
                if (expression.Method.Name == "Select" && expression.Arguments.Count == 2)
                {
                    var part = String.Empty;
                    TryParsePath(expression.Arguments[0], out path);
                    TryParsePath(expression.Arguments[1] as LambdaExpression, out part);
                    path += "." + part;
                    return true;
                }
            }
            path = null;
            return false;
        }
    }
}