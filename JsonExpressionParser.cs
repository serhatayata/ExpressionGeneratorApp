using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace ExpressionGeneratorApp;

public class JsonExpressionParser
{
    private readonly string StringStr = nameof(String).ToLowerInvariant();
    private readonly string BooleanStr = nameof(Boolean).ToLowerInvariant();
    private readonly string Int32Str = nameof(Int32).ToLowerInvariant();
    private readonly string DecimalStr = nameof(Decimal).ToLowerInvariant();
    private readonly string In = nameof(In).ToLowerInvariant();
    private readonly string And = nameof(And).ToLowerInvariant();

    private readonly MethodInfo MethodContains = typeof(Enumerable).GetMethods(
                    BindingFlags.Static | BindingFlags.Public)
                    .Single(m => m.Name == nameof(Enumerable.Contains)
                        && m.GetParameters().Length == 2);

    private delegate Expression Binder(Expression left, Expression right);

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

            var property = Expression.Property(parm, field);

            if (@operator == In)
            {
                var contains = MethodContains.MakeGenericMethod(typeof(string));
                object val = value.EnumerateArray().Select(e => e.GetString())
                    .ToList();
                var right = Expression.Call(
                    contains,
                    Expression.Constant(val),
                    property);
                left = bind(left, right);
            }
            else
            {
                object val = GetTypeValue(type, value);
                var toCompare = Expression.Constant(val);
                var right = GetExpressionEquality(property, toCompare);
                left = bind(left, right);
            }
        }

        return left;
    }

    BinaryExpression? GetExpressionEquality(MemberExpression property, ConstantExpression toCompare)
    {
        if (property.Type.IsEnum)
        {
            toCompare = Expression.Constant(Enum.ToObject(property.Type, toCompare.Value));
            return Expression.Equal(property, toCompare);
        }
        else
        {
            return Expression.Equal(property, toCompare);
        }
    }

    public Expression<Func<T, bool>> ParseExpressionOf<T>(JsonDocument doc)
    {
        var itemExpression = Expression.Parameter(typeof(T));
        var conditions = ParseTree<T>(doc.RootElement, itemExpression);
        if (conditions.CanReduce)
        {
            conditions = conditions.ReduceAndCheck();
        }

        Console.WriteLine(conditions.ToString());

        var query = Expression.Lambda<Func<T, bool>>(conditions, itemExpression);
        return query;
    }

    public object GetTypeValue(string type, JsonElement value)
    {
        if (type == StringStr || type == BooleanStr)
            return (object)value.GetString();
        else if (type == Int32Str)
            return (object)value.GetInt32();
        else if (type == DecimalStr)
            return (object)value.GetDecimal();

        return null;
    }

    public Func<T, bool> ParsePredicateOf<T>(JsonDocument doc)
    {
        var query = ParseExpressionOf<T>(doc);
        return query.Compile();
    }
}
