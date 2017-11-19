using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using TomSun.NetStandard.Extensions.EventHandling;
using HtmlString = Microsoft.AspNetCore.Html.HtmlString;
using System.Linq;
using TomSun.Portable.Factories;

namespace TomSun.AspNetCore.RazorReact.TagHelpers
{
    public abstract class ReactArtifactTagHelper : TagHelper
    {
        protected virtual bool Initialize(ReactRenderContext reactContext)
        {
            return true;
        }

        protected internal virtual bool ProducesGlobalReactContent { get; } = false;

        protected  virtual bool HandleChildReactContent { get; } = false;
        protected virtual string GetReactContent(ReactArtifactContext context)
        {
            return context.ChildrenReactContent;
        }

        protected virtual string GetRazorContent(ReactArtifactContext context)
        {
            return null;
        }


        private IDisposable Process(Action<string> onContentReceived)
        {
            var reactContext = this.ReactRenderContext();
            if (this.HandleChildReactContent)
            {
                return reactContext.Process(args => onContentReceived(args.Content));
            }
            return Api.Create.Disposable(() => { });
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var reactContext = this.ReactRenderContext();
            if (!this.Initialize(reactContext))
            {
                output.SuppressOutput();
                output.Content.SetContent(string.Empty);
                output.TagName = null;
                return;
            }

            
                string childrenRenderContent;
                string childrenReactContent = null;
                using (reactContext.AddScope(this))
                using (this.Process(c => childrenReactContent += c))
                {
                    childrenRenderContent =
                        (await output.GetChildContentAsync(NullHtmlEncoder.Default))
                        .GetContent(NullHtmlEncoder.Default);
                }


            var artifactContext = new ReactArtifactContext
                {
                    ReactContext = reactContext,
                    TagHelperContext = context,
                    TagHelperOutput = output,
                    ChildrenReactContent = childrenReactContent,
                    ChildrenRenderContent = childrenRenderContent,
                };

                var reactContent = this.GetReactContent(artifactContext);
                reactContent = reactContent.Indent(reactContext.Scope.Count, '\t');
                reactContext.AddReactContent(this, reactContent);
                
                var razorContent = this.GetRazorContent(artifactContext) ?? string.Empty;
               // output.PreElement.SetHtmlContent(new HtmlString(razorContent)); // default content
                output.Content.SetHtmlContent(new HtmlString(razorContent));
                output.TagName = null;

            

        }

        
    }
}