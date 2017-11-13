using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using TomSun.Common.SystemExtensionMethods.Explicit;
using TomSun.Portable.Factories;

namespace TomSun.AspNetCore.RazorReact.TagHelpers
{
    public class ReactTagHelper : TagHelper, IReactTagHelper
    {
        public Type ComponentType { get; set; }

        public bool Render { get; set; } = true;
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            await DoProcessAsync(this,context, output, async()=> (await output.GetChildContentAsync()).GetContent());

            await base.ProcessAsync(context, output);
        }

        public static async Task DoProcessAsync(IReactTagHelper tagHelper, TagHelperContext context, TagHelperOutput output, Func<Task<string>> getContent)
        {
            var componentType = tagHelper.ComponentType;
            componentType.NotNull($"The {nameof(componentType)} must not be null");

            var idSpecified = output.Attributes.TryGetAttribute("id", out var idAttribute);
            var id = idSpecified ? idAttribute.Value.ToString() : Guid.NewGuid().ToString();

            var httpContext = Api.Global.CurrentContext();//.Items
            httpContext.Items.TryGetValue(ReactContainerTagHelper.ReactRenderContextKey, out var tmp);
            var reactContext = ((ReactRenderContext)tmp).NotNull();

            // Process the content, child tag helpers will be processed,
            // They can access the items information we passed here.
            string renderContent;
            try
            {
                reactContext.Scope.Add(tagHelper);
                renderContent = await getContent();
            }
            finally
            {
                reactContext.Scope.Remove(tagHelper);
            }

            var componentName = componentType.Name;
            var isSubComponentRendering = reactContext.Scope.Any();
            if (isSubComponentRendering)
            {
                // We are a subcomponent and need to render our name as tag.
                output.TagName = componentName;
            }
            else
            {
                if (!idSpecified)
                {
                    output.Attributes.Add("id", id);
                }
                // We are a root component and need to render us in the global script.
                // and provide a div with our id.
                output.TagMode = TagMode.StartTagAndEndTag;
                output.TagName = "div";

                output.Content.SetContent(string.Empty); // default content

                if (!reactContext.RenderedTypes.Contains(componentType))
                {
                    reactContext.RenderedTypes.Add(componentType);
                    var componentDefinition = Template
                        .Replace("[ComponentName]", Path.GetFileNameWithoutExtension(componentName))
                        .Replace("[ComponentContent]", renderContent);
                    reactContext.RenderInstructions.Add(() => componentDefinition);
                }
                if (tagHelper.Render)
                {
                    var componentRendering = $@"ReactDOM.render(
<{componentName} />,
document.getElementById('{id}'));".SurroundWith(Environment.NewLine);
                    reactContext.RenderInstructions.Add(() => componentRendering);
                }

            }
        }

        public static string Template{ get; } = typeof(ReactTagHelper).Assembly.GetResourceFileContent("ReactComponentTemplate.jsx");

    }
}