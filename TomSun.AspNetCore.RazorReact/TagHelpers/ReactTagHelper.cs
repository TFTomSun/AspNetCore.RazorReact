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

//    public class ReactScriptsTagHelper : TagHelper
//    {
//        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
//        {
//            output.TagMode = TagMode.StartTagAndEndTag;
//            output.TagName = "DotNetifyScripts";
//            //            output.PostElement.SetHtmlContent(new HtmlString(

//            //                @"
//            //<script>
//            //var React = require('react');
//            //</script>"
//            //            ));

//            output.PostElement.SetHtmlContent(new HtmlString(
//                @"
//<script src=""https://cdn.jsdelivr.net/npm/es6-promise@4/dist/es6-promise.auto.js""></script>
//<script src=""https://cdnjs.cloudflare.com/ajax/libs/babel-core/5.8.34/browser.min.js""></script>
//<script src=""https://cdnjs.cloudflare.com/ajax/libs/react/15.6.1/react.js""></script>
//<script src=""https://cdnjs.cloudflare.com/ajax/libs/react/15.6.1/react-dom.js""></script>
//<script src=""https://unpkg.com/create-react-class@15.6.2/create-react-class.min.js""></script>
//<script src=""https://code.jquery.com/jquery-3.1.1.min.js""></script>       
//<script src=""https://unpkg.com/dotnetify@2.0.7-beta/dist/signalR-netcore.js""></script>
//<script src=""https://unpkg.com/dotnetify@2.0.7-beta/dist/dotnetify-react.min.js""></script> 
//"

//            ));

//            await base.ProcessAsync(context, output);
//        }

//    }

    

    internal class ReactRenderContext
    {
        public List<Func<string>> RenderInstructions { get; } = new List<Func<string>>();
        public List<IReactTagHelper> Scope { get; } = new List<IReactTagHelper>();
        public List<Type> RenderedTypes { get; } = new List<Type>();
    }

    public class ReactContainerTagHelper : TagHelper
    {
        public const string ReactRenderContextKey = "ReactRenderContext";
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagMode = TagMode.StartTagAndEndTag;

            var reactContext = new ReactRenderContext();
            var httpContext = Api.Global.CurrentContext();//.Items

            httpContext.Items.Add(ReactRenderContextKey, reactContext);
            var content = await output.GetChildContentAsync();
            context.Items.Remove(ReactRenderContextKey);

            var scriptCode = reactContext.RenderInstructions.Select(i=>i()).Aggregate(
                Environment.NewLine).SurroundWith(Environment.NewLine);


            var fullScriptCode = $@"
<script src=""https://cdn.jsdelivr.net/npm/es6-promise@4/dist/es6-promise.auto.js""></script>
<script src=""https://cdnjs.cloudflare.com/ajax/libs/babel-core/5.8.34/browser.min.js""></script>
<script src=""https://cdnjs.cloudflare.com/ajax/libs/react/15.6.1/react.js""></script>
<script src=""https://cdnjs.cloudflare.com/ajax/libs/react/15.6.1/react-dom.js""></script>
<script src=""https://unpkg.com/create-react-class@15.6.2/create-react-class.min.js""></script>
<script src=""https://code.jquery.com/jquery-3.1.1.min.js""></script>       
<script src=""https://unpkg.com/dotnetify@2.0.7-beta/dist/signalR-netcore.js""></script>
<script src=""https://unpkg.com/dotnetify@2.0.7-beta/dist/dotnetify-react.min.js""></script> 
<script type=""text/babel"">{new HtmlString(scriptCode)}</script>
";
            output.PreElement.SetHtmlContent(new HtmlString(fullScriptCode));

            await base.ProcessAsync(context, output);
        }

    }

    public interface IReactTagHelper
    {
        Type ComponentType { get; }
        bool Render { get;  }
    }
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