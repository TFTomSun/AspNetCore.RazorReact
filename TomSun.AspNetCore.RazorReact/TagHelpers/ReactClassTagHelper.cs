using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TomSun.AspNetCore.RazorReact.TagHelpers
{
    public class ReactClassTagHelper : ReactArtifactTagHelper
    {
        protected internal override bool ProducesGlobalReactContent => true;
        private bool Handle { get; set; } = true;
        protected override bool HandleChildReactContent
        {
            get { return this.Handle; }
        }
        protected override bool Initialize(ReactRenderContext reactContext)
        {
            if (!reactContext.RenderedTypes.Contains(this.ClassName))
            {
                reactContext.RenderedTypes.Add(this.ClassName);
                return true;
            }
            this.Handle = false;
            return false;
        }
        
        protected override string GetReactContent(ReactArtifactContext context)
        {
            var classContent = context.ChildrenReactContent;
            var classContentDefintion =
                $"class {this.ClassName} extends React.Component {{{classContent}}}";
            return classContentDefintion;
        }

        public string ClassName { get; set; }
    }
}