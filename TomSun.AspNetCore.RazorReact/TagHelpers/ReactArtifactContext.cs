using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TomSun.AspNetCore.RazorReact.TagHelpers
{
    public class ReactArtifactContext
    {
        public ReactRenderContext ReactContext { get; internal set; }
        public TagHelperContext TagHelperContext { get; internal set; }
        public TagHelperOutput TagHelperOutput { get; internal set; }
        public string ChildrenReactContent { get; set; }
        public string ChildrenRenderContent { get; set; }
    }
}