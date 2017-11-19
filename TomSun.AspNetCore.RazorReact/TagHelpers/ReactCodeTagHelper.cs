namespace TomSun.AspNetCore.RazorReact.TagHelpers
{
    public class ReactCodeTagHelper : ReactArtifactTagHelper
    {
        protected override string GetReactContent(ReactArtifactContext context)
        {
            return context.ChildrenRenderContent.FixIndent();
        }
    }
}