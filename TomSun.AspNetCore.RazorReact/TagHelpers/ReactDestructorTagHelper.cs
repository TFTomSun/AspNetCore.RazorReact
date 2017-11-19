namespace TomSun.AspNetCore.RazorReact.TagHelpers
{
    public class ReactDestructorTagHelper : ReactArtifactTagHelper
    {
        protected override bool HandleChildReactContent { get; } = true;
        protected override string GetReactContent(ReactArtifactContext context)
        {
            var modified = 
$@"
componentWillUnmount()
{{
{context.ChildrenRenderContent.FixIndent()}
}}";
            return modified;
        }
    }
}