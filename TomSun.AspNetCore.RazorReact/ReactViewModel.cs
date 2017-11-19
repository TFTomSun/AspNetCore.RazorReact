using DotNetify;

namespace TomSun.AspNetCore.RazorReact
{
    public abstract class ReactViewModel<TSelf> :BaseVM
        where TSelf: ReactViewModel<TSelf>
    {
    }
}