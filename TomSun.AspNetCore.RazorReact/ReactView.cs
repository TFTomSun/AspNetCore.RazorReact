using DotNetify;

namespace TomSun.AspNetCore.RazorReact
{
    public class ReactView<T>
        where T:BaseVM
    {
        public T state { get; }

    }
}