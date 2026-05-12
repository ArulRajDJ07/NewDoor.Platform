using System.Linq.Expressions;
using System.Reflection;

namespace NewDoor.API.Infrastructure.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> query, string fieldName, string operatorType, string value)
        {
            if (string.IsNullOrWhiteSpace(fieldName) || string.IsNullOrWhiteSpace(value))
                return query;

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = GetProperty<T>(parameter, fieldName);

            if (property == null)
                return query;

            var propertyType = GetPropertyType(property);
            var constantValue = ConvertValue(value, propertyType);

            if (constantValue == null)
                return query;

            var constant = Expression.Constant(constantValue, propertyType);
            Expression comparison = operatorType?.ToUpper() switch
            {
                "EQUALS" or "==" or "=" => Expression.Equal(property, constant),
                "NOTEQUALS" or "!=" => Expression.NotEqual(property, constant),
                "CONTAINS" => Expression.Call(property, typeof(string).GetMethod("Contains", new[] { typeof(string) })!, constant),
                "STARTSWITH" => Expression.Call(property, typeof(string).GetMethod("StartsWith", new[] { typeof(string) })!, constant),
                "ENDSWITH" => Expression.Call(property, typeof(string).GetMethod("EndsWith", new[] { typeof(string) })!, constant),
                "GREATERTHAN" or ">" => Expression.GreaterThan(property, constant),
                "GREATERTHANOREQUAL" or ">=" => Expression.GreaterThanOrEqual(property, constant),
                "LESSTHAN" or "<" => Expression.LessThan(property, constant),
                "LESSTHANOREQUAL" or "<=" => Expression.LessThanOrEqual(property, constant),
                _ => Expression.Equal(property, constant)
            };

            var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
            return query.Where(lambda);
        }

        public static IQueryable<T> ApplyStringFilter<T>(this IQueryable<T> query, string propertyName, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return query;

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyName);
            var constant = Expression.Constant(value, typeof(string));
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var containsExpression = Expression.Call(property, containsMethod!, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(containsExpression, parameter);

            return query.Where(lambda);
        }

        public static IQueryable<T> ApplyRangeFilter<T, TValue>(this IQueryable<T> query, string propertyName, TValue? min, TValue? max)
            where TValue : struct, IComparable<TValue>
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyName);

            Expression? filter = null;

            if (min.HasValue)
            {
                var minConstant = Expression.Constant(min.Value, typeof(TValue));
                var minComparison = Expression.GreaterThanOrEqual(property, minConstant);
                filter = minComparison;
            }

            if (max.HasValue)
            {
                var maxConstant = Expression.Constant(max.Value, typeof(TValue));
                var maxComparison = Expression.LessThanOrEqual(property, maxConstant);
                filter = filter == null ? maxComparison : Expression.AndAlso(filter, maxComparison);
            }

            if (filter != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(filter, parameter);
                return query.Where(lambda);
            }

            return query;
        }

        public static IQueryable<T> ApplyDateRangeFilter<T>(this IQueryable<T> query, string propertyName, DateTime? from, DateTime? to)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyName);

            Expression? filter = null;

            if (from.HasValue)
            {
                var fromConstant = Expression.Constant(from.Value, typeof(DateTime));
                var fromComparison = Expression.GreaterThanOrEqual(property, fromConstant);
                filter = fromComparison;
            }

            if (to.HasValue)
            {
                var toConstant = Expression.Constant(to.Value, typeof(DateTime));
                var toComparison = Expression.LessThanOrEqual(property, toConstant);
                filter = filter == null ? toComparison : Expression.AndAlso(filter, toComparison);
            }

            if (filter != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(filter, parameter);
                return query.Where(lambda);
            }

            return query;
        }

        public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, string? sortBy, string? sortDirection)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return query;

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = GetProperty<T>(parameter, sortBy);

            if (property == null)
                return query;

            var lambda = Expression.Lambda(property, parameter);
            var methodName = sortDirection?.ToUpper() == "DESC" ? "OrderByDescending" : "OrderBy";
            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { typeof(T), property.Type },
                query.Expression,
                Expression.Quote(lambda));

            return query.Provider.CreateQuery<T>(resultExpression);
        }

        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int? pageNumber, int? pageSize)
        {
            if (!pageNumber.HasValue || !pageSize.HasValue || pageNumber.Value <= 0 || pageSize.Value <= 0)
                return query;

            return query.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value);
        }

        private static MemberExpression? GetProperty<T>(Expression parameter, string propertyName)
        {
            try
            {
                var properties = propertyName.Split('.');
                Expression property = parameter;

                foreach (var prop in properties)
                {
                    var propInfo = property.Type.GetProperty(prop, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propInfo == null)
                        return null;
                    property = Expression.Property(property, propInfo);
                }

                return property as MemberExpression;
            }
            catch
            {
                return null;
            }
        }

        private static Type GetPropertyType(Expression property)
        {
            return property.Type;
        }

        private static object? ConvertValue(string value, Type targetType)
        {
            try
            {
                targetType = Nullable.GetUnderlyingType(targetType) ?? targetType;

                if (targetType == typeof(string))
                    return value;

                if (targetType == typeof(int))
                    return int.Parse(value);

                if (targetType == typeof(long))
                    return long.Parse(value);

                if (targetType == typeof(decimal))
                    return decimal.Parse(value);

                if (targetType == typeof(double))
                    return double.Parse(value);

                if (targetType == typeof(bool))
                    return bool.Parse(value);

                if (targetType == typeof(DateTime))
                    return DateTime.Parse(value);

                if (targetType == typeof(Guid))
                    return Guid.Parse(value);

                return Convert.ChangeType(value, targetType);
            }
            catch
            {
                return null;
            }
        }
    }
}
