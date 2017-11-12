using System;
using System.Linq;
using System.Linq.Expressions;
using DotNetify;

namespace TomSunn.DotNetify.SampleWebApp
{
    public abstract class ReactComponent<TSelf> :BaseVM
        where TSelf: ReactComponent<TSelf>
    {
        public static string Bind<TResult>(Expression<Func<ReactComponentClient<TSelf>, TResult>> expression)
        {
            var body = expression.Body.ToString().Replace(expression.Parameters.Single().Name+".","this.");

            return $"{{{body}}}";
        }

        //public static ReactComponentClient<TSelf> React { get; } = new ReactComponentClient<TSelf>();

    }
}