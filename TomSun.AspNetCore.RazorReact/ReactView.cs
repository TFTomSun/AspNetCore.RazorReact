using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using DotNetify;

namespace TomSun.AspNetCore.RazorReact
{
    public sealed class ReactView<TState, TProps> : ReactView<ReactView<TState, TProps>, TState, TProps>
        where TState : BaseVM
    {

    }

    public abstract class ReactView
    {
        public dynamic state { get; }

        public dynamic props { get; }

        protected static void alert(object x)
        {

        }
    }
    public abstract class ReactView<TSelf, TState, TProps>: ReactView
    {
        public new TState state { get; }

        public new TProps props { get; }

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

    public class ActionDefinition : ActionDefinition<object>
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