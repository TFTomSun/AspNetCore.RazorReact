using System.Net;

namespace TomSun.AspNetCore.RazorReact.TagHelpers
{
    public class ReactRenderFuncionTagHelper : ReactArtifactTagHelper
    {
        protected override string GetReactContent(ReactArtifactContext context)
        {
            var modified = $@"
render()
{{
    return (
{WebUtility.HtmlDecode(context.ChildrenRenderContent)}
            );
}}";
            return modified;
        }
    }
}