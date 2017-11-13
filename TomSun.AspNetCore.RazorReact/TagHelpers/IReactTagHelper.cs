using System;

namespace TomSun.AspNetCore.RazorReact.TagHelpers
{
    public interface IReactTagHelper
    {
        Type ComponentType { get; }
        bool Render { get;  }
    }
}