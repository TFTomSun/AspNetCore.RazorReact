using DotNetify;

namespace TomSunn.DotNetify.SampleWebApp
{
    public class ReactComponentClient<T>
        where T:BaseVM
    {
        public T state { get; }

    }
}