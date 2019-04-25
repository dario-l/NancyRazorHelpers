using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Nancy.Razor.Helpers.Extensions
{
    public static class ExpressionExtensions
    {
        public static PropertyInfo AsPropertyInfo<T, TR>(this Expression<Func<T, TR>> expr)
        {
            return expr.Body is MemberExpression memExpr ? memExpr.Member as PropertyInfo : null;
        }
    }
}
