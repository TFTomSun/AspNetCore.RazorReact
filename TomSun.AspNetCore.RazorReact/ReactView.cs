using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using AgileObjects.ReadableExpressions;
using DotNetify;
using Microsoft.AspNetCore.Mvc.Razor;

namespace TomSun.AspNetCore.RazorReact
{
    public abstract class ReactRazorViewComponentPage<TComponent, TView, TProps, TState> : RazorPage<object> // must be object
        where TView : ReactView<TView,TState,TProps>,new ()
        where TProps : new() 
        where TState : new()
    {
        public string ReactClassName => typeof(TComponent).Name;

        public string Bind<TResult>(Expression<Func<TView, TResult>> binding)
        {
            return new TView().Bind(binding);
        }
    }
    public abstract class ReactRazorPage<TModel> : Microsoft.AspNetCore.Mvc.RazorPages.Page
    {
        public int Test { get; set; }
    }
    public sealed class ReactView<TState, TProps> : ReactView<ReactView<TState, TProps>, TState, TProps> where TState : new() where TProps : new()
    {

    }

    public abstract class ReactView
    {
        public dynamic state { get; }

        public dynamic props { get; }

        protected static void alert(object x)
        {

        }

        //public void setState(object value)
        //{
            
        //}
    }

    public abstract class ReactView<TSelf, TState, TProps> : ReactView
        where TProps : new()
        where TState : new()
    {
        public new TState state { get; } = new TState();

        public new TProps props { get; } = new TProps();

        public new void setState(TState value)
        {

        }

        public ActionDefinition<T> HandleEvent<T>(Expression<Action<TSelf,T>> expression)
        {
            var stateType = typeof(TState);
            var typeName = stateType.Name;
            if (stateType.IsNested)
            {
                typeName = stateType.DeclaringType.Name + "." + stateType.Name;
            }
            var viewParameterName = expression.Parameters.First().Name;
            var body = expression.ToReadableString()
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Select(l=>l.Trim()).Aggregate(string.Empty)
                .Replace($"({viewParameterName},","")
                .Replace(") =>", " =>")
                .Replace(viewParameterName + ".", "this.")
                .Replace($"new {typeName}{{", "{")
                .Replace(" = ", ": ");
             
            var value = body;
            return $"{{{value}}}";
        }

        public string Bind<TResult>(Expression<Func<TSelf, TResult>> expression)
        {
            var body = expression.Body.ToString().Replace(expression.Parameters.Single().Name + ".", "this.");
            var value = body;
            return $"{{{value}}}";
        }

        protected FunctionDefinition<TResult> Call<TResult>(Func<TResult> function, [CallerMemberName] string memberName = null,
            [CallerFilePath] string codeFilePath = null, [CallerLineNumber] int line = 0)
        {
            
            return FunctionDefinition<TResult>.Create(function,
                memberName, codeFilePath, line);
        }

    }

    public abstract class MethodDefinition<TSelf, T>
        where TSelf : MethodDefinition<TSelf, T>, new()

    {
        public static implicit operator MethodDefinition<TSelf, T>(string value)
        {
            return Create(value);
        }

        protected const string UseFactoryMessage = "Use Class.Create instead";
        [Obsolete(UseFactoryMessage, true)]
        protected MethodDefinition()
        {
        }

        public T Function { get; private set; }
        public string MemberName { get; private set; }
        public string CodeFilePath { get; private set; }
        public int Line { get; private set; }
        public Assembly CallingAssembly { get; private set; }

   
        private string GetContent()
        {
            var sourceFileContent = this.CallingAssembly.GetResourceFileContent(
                Path.GetFileName(this.CodeFilePath));

            var lines = sourceFileContent.Split(Environment.NewLine).Skip(
                this.Line-1);

            int bracked = 0;
            string code = string.Empty;

            bool done = false;
            bool begin = false;
            foreach (var line in lines)
            {
                foreach (var character in line)
                {
                    if (character == ')')
                    {
                        --bracked;
                    }
                    if (bracked > 0)
                    {
                        code += character;
                    }
                    if (character == '(')
                    {
                        begin = true;
                        ++bracked;
                    }
                    
                    if (bracked == 0)
                    {
                        if (!begin)
                        {
                            continue;
                        }
                        done = true;
                        break;
                    }
                }
                if (done)
                {
                    break;
                }
            }
           
           // var functionContent = lines.FixIndent().Aggregate(Environment.NewLine);
            return $"{{{code.Trim()}}}";
        }

        public override string ToString()
        {
            return this.StringContent ?? this.GetContent();
        }

        private string StringContent { get; set; }

        public static TSelf Create(string value)
        {
            return new TSelf {StringContent = value};
        }
        public static TSelf Create(T function, [CallerMemberName] string memberName = null,
            [CallerFilePath] string codeFilePath = null, [CallerLineNumber] int line = 0)
        {
            var self = new TSelf
            {
                Function = function,
                MemberName = memberName,
                CodeFilePath = codeFilePath,
                Line = line,
                CallingAssembly = Assembly.GetCallingAssembly() ,

            };

            return self;
        }
    }

    public class ActionDefinition : ActionDefinition<string>
    {
        [Obsolete(UseFactoryMessage, true)]
        public ActionDefinition()
        {
        }
    }
    public class ActionDefinition<TParameter> : MethodDefinition<ActionDefinition<TParameter>, Action<TParameter>>
    {
        public static implicit operator ActionDefinition<TParameter>(string value)
        {
            return Create(value);
        }

        [Obsolete(UseFactoryMessage, true)]
        public ActionDefinition()
        {
        }
    }
    public class FunctionDefinition<TResult> : MethodDefinition<FunctionDefinition<TResult>, Func<TResult>>
    {
        [Obsolete(UseFactoryMessage, true)]
        public FunctionDefinition()
        {
        }
    }
}