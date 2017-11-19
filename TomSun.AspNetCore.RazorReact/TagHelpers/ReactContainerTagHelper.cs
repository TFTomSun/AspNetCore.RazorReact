using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using TomSun.Common.SystemExtensionMethods.Explicit;
using TomSun.Portable.Factories;

namespace TomSun.AspNetCore.RazorReact.TagHelpers
{
    public class ReactContainerTagHelper : TagHelper
    {
        public const string ReactRenderContextKey = "ReactRenderContext";

        private ReactRenderContext RenderContext { get; } = new ReactRenderContext();
        public override void Init(TagHelperContext context)
        {
            var httpContext = Api.Global.CurrentContext();//.Items

            httpContext.Items.Add(ReactRenderContextKey, this.RenderContext);
        }
      

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagMode = TagMode.StartTagAndEndTag;

            //var reactContext = new ReactRenderContext();
            //var httpContext = Api.Global.CurrentContext();//.Items

            //httpContext.Items.Add(ReactRenderContextKey, reactContext);
            var content = (await output.GetChildContentAsync(NullHtmlEncoder.Default));//.NotNull();
            //context.Items.Remove(ReactRenderContextKey);

            var scriptCode = this.RenderContext.ReactContent.Where(
                x=>x.content != null).OrderBy(x=>x.Priority).Select(
                x=>x.content).Aggregate(Environment.NewLine).SurroundWith(Environment.NewLine);


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
            output.Content.SetHtmlContent(content);
            //output.Content.Clear();
            //output.TagName = null;
//            await base.ProcessAsync(context, output);
        }

    }
}