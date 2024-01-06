﻿using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace ExpressionGeneratorApp;

public static class QueryHelper
{
    private static readonly MethodInfo OrderByMethod =
        typeof(Queryable).GetMethods().Single(method =>
        method.Name == "OrderBy" && method.GetParameters().Length == 2);

    private static readonly MethodInfo OrderByDescendingMethod =
        typeof(Queryable).GetMethods().Single(method =>
        method.Name == "OrderByDescending" && method.GetParameters().Length == 2);

    public static bool PropertyExists<T>(this IQueryable<T> source, string propertyName)
    {
        return typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase |
            BindingFlags.Public | BindingFlags.Instance) != null;
    }

    public static IQueryable<T> ProcessByProperty<T>(
       this IQueryable<T> source, JsonDocument doc) where T : class
    {
        return source.IncludeProperty(doc)
                     .OrderByProperty(doc);
                     
    }

    private static IQueryable<T> OrderByProperty<T>(
        this IQueryable<T> source, JsonDocument doc) where T : class
    {
        var data = doc.RootElement;
        JsonElement order = data.GetProperty(nameof(order));

        string field = order.GetProperty(nameof(field)).GetString();
        string sort = order.GetProperty(nameof(sort)).GetString().Capitalize();

        if (field == null)
            return source;

        var sortTypeParse = Enum.TryParse(sort, out ListSortDirection listSort);
        if (!sortTypeParse)
            throw new ArgumentException("Sort type parse failed");

        if (typeof(T).GetProperty(field, BindingFlags.IgnoreCase |
            BindingFlags.Public | BindingFlags.Instance) == null)
            return null;

        ParameterExpression paramExpression = Expression.Parameter(typeof(T));
        Expression orderByProperty = Expression.Property(paramExpression, field);
        LambdaExpression lambda = Expression.Lambda(orderByProperty, paramExpression);

        MethodInfo genericMethod;
        if (listSort == ListSortDirection.Ascending)
            genericMethod = OrderByMethod.MakeGenericMethod(typeof(T), orderByProperty.Type);
        else
            genericMethod = OrderByDescendingMethod.MakeGenericMethod(typeof(T), orderByProperty.Type);

        object ret = genericMethod.Invoke(null, new object[] { source, lambda });
        return (IQueryable<T>)ret;
    }

    private static IQueryable<T> IncludeProperty<T>(
        this IQueryable<T> source, JsonDocument doc) where T : class
    {
        var data = doc.RootElement;
        JsonElement rules = data.GetProperty(nameof(rules));

        List<string> fields = new List<string>();
        foreach (var rule in rules.EnumerateArray())
        {
            string field = rule.GetProperty(nameof(field)).GetString();
            var fieldItems = field.Split(".");
            if (fieldItems.Count() > 0)
            {
                var foreignObjects = fieldItems.Take(fieldItems.Count() - 1);
                if (foreignObjects.Count() > 0)
                {
                    var fieldValues = string.Join(".", foreignObjects);
                    if (!fields.Any(f => f == fieldValues))
                        fields.Add(fieldValues);
                }
            }
        }

        foreach (var field in fields)
            source = source.Include(field);

        return source;
    }
}
