namespace TomSun.AspNetCore.RazorReact.TagHelpers
{
    public class ReactConstructorTagHelper : ReactArtifactTagHelper
    {
        protected override bool HandleChildReactContent { get; } = true;

        protected override string GetReactContent(ReactArtifactContext context)
        {
            var modified = $@"
constructor(props)
{{
    super(props);
{context.ChildrenRenderContent.FixIndent()}
}}
";
            return modified;
        }
    }
}