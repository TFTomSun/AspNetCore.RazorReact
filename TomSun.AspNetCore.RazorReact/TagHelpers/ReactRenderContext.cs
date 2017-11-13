using System;
using System.Collections.Generic;

namespace TomSun.AspNetCore.RazorReact.TagHelpers
{
    internal class ReactRenderContext
    {
        public List<Func<string>> RenderInstructions { get; } = new List<Func<string>>();
        public List<IReactTagHelper> Scope { get; } = new List<IReactTagHelper>();
        public List<Type> RenderedTypes { get; } = new List<Type>();
    }
}