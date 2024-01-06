using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace ExpressionGeneratorApp;

public class JsonExpressionParser
{
    private readonly string In = nameof(In).ToLowerInvariant();
    private readonly string And = nameof(And).ToLowerInvariant();
    private static MethodInfo methodContains = typeof(Enumerable).GetMethods(
                   BindingFlags.Static | BindingFlags.Public)
                   .Single(m => m.Name == nameof(Enumerable.Contains)
                   && m.GetParameters().Length == 2);

    private delegate Expression Binder(Expression left, Expression right);

    public Func<T, bool> ParsePredicateOf<T>(JsonDocument doc)
    {
        var query = ParseExpressionOf<T>(doc);
        return query.Compile();
    }

    public Expression<Func<T, bool>> ParseExpressionOf<T>(JsonDocument doc)
    {
        var itemExpression = Expression.Parameter(typeof(T));
        var conditions = ParseTree<T>(doc.RootElement, itemExpression);
        if (conditions.CanReduce)
            conditions = conditions.ReduceAndCheck();

        var query = Expression.Lambda<Func<T, bool>>(conditions, itemExpression);
        return query;
    }

    private Expression ParseTree<T>(
    JsonElement condition,
    ParameterExpression parm)
    {
        Expression left = null;
        var gate = condition.GetProperty(nameof(condition)).GetString();

        JsonElement rules = condition.GetProperty(nameof(rules));

        Binder binder = gate == And ? (Binder)Expression.And : Expression.Or;

        Expression bind(Expression left, Expression right) =>
            left == null ? right : binder(left, right);

        foreach (var rule in rules.EnumerateArray())
        {
            if (rule.TryGetProperty(nameof(condition), out JsonElement check))
            {
                var right = ParseTree<T>(rule, parm);
                left = bind(left, right);
                continue;
            }

            string @operator = rule.GetProperty(nameof(@operator)).GetString();
            string type = rule.GetProperty(nameof(type)).GetString();
            string field = rule.GetProperty(nameof(field)).GetString();

            JsonElement value = rule.GetProperty(nameof(value));

            var property = GetMemberProperty(parm, field);
            if (@operator == ComparisonOperator.@in.ToString())
            {
                var right = CreateInExpression(property, value, type);
                left = bind(left, right);
            }
            else if (@operator == ComparisonOperator.contains.ToString())
            {
                var right = CreateContainsExpression(property, value, type);
                left = bind(left, right);
            }
            else
            {
                object val = GetTypeValue(type, value);
                var right = GetExpressionComparison(property, val, @operator);
                left = bind(left, right);
            }
        }
        
        return left;
    }

    private static Expression GetMemberProperty(
    ParameterExpression param, 
    string field)
    {
        if (!field.Contains("."))
        {
            return Expression.Property(param, field);
        }
        else
        {
            Expression member = param;
            foreach (var namePart in field.Split('.'))
                member = Expression.Property(member, namePart);

            return member;
        }
    }

    private static MethodCallExpression CreateInExpression(
    Expression prop,
    JsonElement val,
    string type)
    {
        object value = GetTypeEnumeratedValue(val, type);

        return Expression.Call(
               methodContains.MakeGenericMethod(GetType(type)),
               Expression.Constant(value),
               prop);
    }

    private static MethodCallExpression CreateContainsExpression(
    Expression prop,
    JsonElement val,
    string type)
    {
        object value = GetTypeValue(type, val);
        return Expression.Call(
               prop,
               nameof(string.Contains),
               Type.EmptyTypes,
               Expression.Constant(value));
    }

    private BinaryExpression? GetExpressionComparison(
    Expression property, 
    object val, 
    string @operator)
    {
        var comparisonParse = Enum.TryParse(@operator, out ComparisonOperator comparisonOperator);
        if (!comparisonParse)
            throw new ArgumentException("Comparison parse failed");

        var toCompare = Expression.Constant(val);
        if (property.Type.IsEnum)
        {
            toCompare = Expression.Constant(Enum.ToObject(property.Type, toCompare.Value));
            return Expression.Equal(property, toCompare);
        }
        else
        {
            return comparisonOperator switch
            {
                ComparisonOperator.equal => Expression.Equal(property, toCompare),
                ComparisonOperator.notequal => Expression.NotEqual(property, toCompare),
                ComparisonOperator.greater => Expression.GreaterThan(property, toCompare),
                ComparisonOperator.greaterorequal => Expression.GreaterThanOrEqual(property, toCompare),
                ComparisonOperator.less => Expression.LessThan(property, toCompare),
                ComparisonOperator.lessorequal => Expression.LessThanOrEqual(property, toCompare)
            };
        }
    }

    private static object? GetTypeValue(
    string type, 
    JsonElement value)
    {
        var valueType = GetType(type);
        TypeCode typeCode = Type.GetTypeCode(valueType);

        return typeCode switch
        {
            TypeCode.String => value.GetString(),
            TypeCode.Boolean => value.GetBoolean(),
            TypeCode.Int32 => value.GetInt32(),
            TypeCode.Int16 => value.GetInt16(),
            TypeCode.Int64 => value.GetInt64(),
            TypeCode.Byte => value.GetByte(),
            TypeCode.Single => value.GetSingle(),
            TypeCode.Decimal => value.GetDecimal(),
            TypeCode.DateTime => value.GetDateTime(),
            _ => TypeCode.Empty
        };
    }

    private static object GetTypeEnumeratedValue(
    JsonElement val,
    string type)
    {
        var valueType = GetType(type);
        TypeCode typeCode = Type.GetTypeCode(valueType);

        var enumeratedArray = val.EnumerateArray();
        return typeCode switch
        {
            TypeCode.String => enumeratedArray.Select(s => s.GetString()),
            TypeCode.Int32 => enumeratedArray.Select(s => s.GetInt32()),
            TypeCode.Int16 => enumeratedArray.Select(s => s.GetInt16()),
            TypeCode.Single => enumeratedArray.Select(s => s.GetSingle()),
            TypeCode.Decimal => enumeratedArray.Select(s => s.GetDecimal()),
        };
    }

    private static Type GetType(string type)
    {
        string StringStr = TypeCode.String.ToString().ToLowerInvariant();
        string BooleanStr = TypeCode.Boolean.ToString().ToLowerInvariant();
        string IntStr = TypeCode.Int32.ToString().Substring(0, 3).ToLowerInvariant();
        string DecimalStr = TypeCode.Decimal.ToString().ToLowerInvariant();
        string DateTimeStr = TypeCode.DateTime.ToString().ToLowerInvariant();

        if (type == StringStr)
            return typeof(string);
        else if (type == BooleanStr)
            return typeof(bool);
        else if (type == IntStr)
            return typeof(int);
        else if (type == DecimalStr)
            return typeof(decimal);
        else if (type == DateTimeStr)
            return typeof(DateTime);

        return null;
    }
}
