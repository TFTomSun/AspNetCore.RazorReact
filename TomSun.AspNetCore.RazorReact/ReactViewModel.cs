using System;
using System.Linq;
using System.Linq.Expressions;
using DotNetify;

namespace TomSun.AspNetCore.RazorReact
{
    public sealed class ReactView<TState, TProps> : ReactView<ReactView<TState, TProps>, TState, TProps> where TState : BaseVM
    {
        
    }
    public abstract class ReactView<TSelf,  TState, TProps>
        where TState: BaseVM
    {
        public TState state { get; }

        public TProps props { get; }
      
        public  string Bind<TResult>(Expression<Func<TSelf, TResult>> expression)
        {
            var body = expression.Body.ToString().Replace(expression.Parameters.Single().Name + ".", "this.");

            return $"{{{body}}}";
        }
    }
    public abstract class ReactViewModel<TSelf> :BaseVM
        where TSelf: ReactViewModel<TSelf>
    {

        //[Obsolete("Use .View.Bind instead")]
        //public static string Bind<TResult>(Expression<Func<ReactView<TSelf>, TResult>> expression)
        //{
        //    var body = expression.Body.ToString().Replace(expression.Parameters.Single().Name+".","this.");

        //    return $"{{{body}}}";
        //}

        //public static ReactComponentClient<TSelf> React { get; } = new ReactComponentClient<TSelf>();

    }
}