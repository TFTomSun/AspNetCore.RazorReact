using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using TomSun.Common.SystemExtensionMethods.Explicit;

namespace TomSun.AspNetCore.Extensions.TagHelpers
{

    public class ReactScriptsTagHelper : TagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "DotNetifyScripts";
            output.PostElement.SetHtmlContent(new HtmlString(
                @"
<script src=""https://cdnjs.cloudflare.com/ajax/libs/babel-core/5.8.34/browser.min.js""></script>
<script src=""https://cdnjs.cloudflare.com/ajax/libs/react/15.6.1/react.js""></script>
<script src=""https://cdnjs.cloudflare.com/ajax/libs/react/15.6.1/react-dom.js""></script>
<script src=""https://unpkg.com/create-react-class@15.6.2/create-react-class.min.js""></script>
<script src=""https://code.jquery.com/jquery-3.1.1.min.js""></script>       
<script src=""https://unpkg.com/dotnetify@2.0.7-beta/dist/signalR-netcore.js""></script>
<script src=""https://unpkg.com/dotnetify@2.0.7-beta/dist/dotnetify-react.min.js""></script> 
"

            ));//result.GetHtmlString()

            await base.ProcessAsync(context, output);
        }

    }

    public class ReactTagHelper : TagHelper
    {
        public Type ComponentType { get; set; }

   
        public string Id { get; set; }
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            //this.Id.NotNull($"The {nameof(this.Id)} must not be null");
            this.ComponentType.NotNull($"The {nameof(this.ComponentType)} must not be null");

            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "div";
            if (this.Id == null)
            {
                this.Id = Guid.NewGuid().ToString();
                output.Attributes.Add("id", this.Id);
            }

            var content = await  output.GetChildContentAsync();
            output.Content.SetContent(string.Empty); // default content
            var renderContent = content.GetContent();
//            var renderContent = @"
// <div>
//    {this.state.Greetings}<br />
//    Server time is: {this.state.ServerTime}
//</div>";
            var component = Template
                .Replace("[ComponentName]", Path.GetFileNameWithoutExtension(this.ComponentType.Name))
                .Replace("[ComponentContent]", renderContent)
                .Replace("[TargetId]", this.Id);

            var componentScript = 
                $@"
<script type=""text/babel"">
{component}
</script>";
            output.PostElement.SetHtmlContent(new HtmlString(componentScript));

            await base.ProcessAsync(context, output);
        }

        public static string Template{ get; } = typeof(ReactTagHelper).Assembly.GetResourceFileContent("ReactComponentTemplate.jsx");

    }
}