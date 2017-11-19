using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using DotNetify;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using TomSun.AspNetCore.RazorReact.TagHelpers;
using TomSun.Portable.Factories;
using System.Linq;
using System.Linq.Expressions;
using TomSun.AspNetCore.RazorReact;

// ReSharper disable once CheckNamespace
public static class Extensions
{
    public static ActionDefinition<T> Handle<T, TView, TState, TProps>(this ActionDefinition<T> reactEvent,
        ReactView<TView, TState, TProps> context,
        Expression<Action<TView, T>> expression) 
        where TState : new() where TProps : new()
    {
       return context.HandleEvent( expression);
    }
    public static IEnumerable<string> FixIndent(this IEnumerable<string> codeLines)
    {
        var defaultIndent = 0;
        var result = codeLines.Select((l, i) =>
        {

            if (i == 0)
            {
                var line = l.TrimStart(' ');
                defaultIndent = l.Length - line.Length;
                return line;
            }
            else
            {
                return l.Length <= defaultIndent ? string.Empty : l.Substring(defaultIndent);
            }
        });
        return result;
    }

    public static string ToPascalCase(this string camelOrKebabCase)
    {
        var pascalCase = camelOrKebabCase.Split(
            '-', StringSplitOptions.RemoveEmptyEntries).Select(
            x => x.Remove(0, 1).Insert(0, char.ToUpper(x[0]).ToString())).Aggregate(string.Empty);
        return pascalCase;
    }
    public static  string FixIndent(this string rawCode)
    {
        var result = rawCode.Split(Environment.NewLine, 
            StringSplitOptions.RemoveEmptyEntries).FixIndent().Aggregate(Environment.NewLine);
        return result;
    }


    public static void Remove(this TagHelperAttributeList list, params string[] attributeNamesToRemove)
    {
        list.Where(a => attributeNamesToRemove.Contains(a.Name)).ForEach(a => list.Remove(a));
    }
    internal static ReactRenderContext ReactRenderContext(this TagHelper tagHelper)
    {
        var httpContext = Api.Global.CurrentContext(); //.Items
        httpContext.Items.TryGetValue(ReactContainerTagHelper.ReactRenderContextKey, out var tmp);
        var reactContext = (ReactRenderContext)tmp;
        return reactContext;
    }

    public static string ToKebabCase(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return Regex.Replace(
                    value,
                    "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])",
                    "-$1",
                    RegexOptions.Compiled)
                .Trim('-')
                .ToLower();
        }
    

    // If you want to implement "*" only
    public static String ToRegExPattern(this String wildcardBasedPattern)
    {
        return "^" + Regex.Escape(wildcardBasedPattern).Replace("\\*", ".*") + "$";
    }

    public static T Convert<T>(this ITuple tuple)
            where T : new()
        {
            var typedProperties = typeof(T).GetProperties();
            var tupleFields = tuple.GetType().GetFields();
            if (typedProperties.Length != tupleFields.Length)
            {
                throw Api.Create.Exception("Properties count not matching");
            }
            var typedInstance = new T();
            for (var i = 0; i < typedProperties.Length; ++i)
            {
                var typedProperty = typedProperties[i];
                var tupleField = tupleFields[i];
                typedProperty.SetValue(typedInstance, tupleField.GetValue(tuple));
            }
            return typedInstance;
        }
    

    public static void UseRazorReact(this IServiceCollection services)
    {
        services.AddNodeServices();
        services.AddSignalR();
        services.AddDotNetify();
        services.AddSpaServicesAndComponents();

    }

    public static void UseRazorReact(this IApplicationBuilder app, IServiceProvider serviceProvider)
    {
        app.UseWebSockets();
        app.UseSignalR(routes => routes.MapDotNetifyHub());
        app.UseDotNetify();
        app.UseSpaFramework(serviceProvider);
    }
}

