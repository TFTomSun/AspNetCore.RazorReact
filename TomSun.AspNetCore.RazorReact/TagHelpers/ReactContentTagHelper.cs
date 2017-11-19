using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TomSun.AspNetCore.RazorReact.TagHelpers
{
    [HtmlTargetElement("*")]
    public class ReactContentTagHelper : TagHelper
    {
        public override void Init(TagHelperContext context)
        {
        }
        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (this.ReactRenderContext() != null)
            {
                foreach (var attribute in output.Attributes.ToArray())
                {
                    TagHelperAttribute replacement = null;
                    if (attribute.Value is HtmlString htmlString)
                    {
                        var stringValue = htmlString.Value;
                        if (stringValue.StartsWith('{') && stringValue.EndsWith('}'))
                        {
                            output.Attributes.Remove(attribute);
                            replacement = new TagHelperAttribute(attribute.Name,
                                stringValue, HtmlAttributeValueStyle.NoQuotes);
                        }


                    }

                    if (replacement != null)
                    {
                        output.Attributes.Remove(attribute);
                        output.Attributes.Add(replacement);
                    }
                }
            }


            return base.ProcessAsync(context, output);
        }
    }
}