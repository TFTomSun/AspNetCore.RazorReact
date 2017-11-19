using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TomSun.AspNetCore.RazorReact.TagHelpers
{
    public class ReactRenderTagHelper : ReactArtifactTagHelper
    {
        public string ClassName { get; set; }
        public string TargetId { get; set; }

        private Guid Guid { get; } = Guid.NewGuid();
        private string FinalTargetId => this.TargetId ?? this.Guid.ToString();

        //public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        //{
        //    if (this.ReactRenderContext().Scope.OfType<ReactRenderFuncionTagHelper>().Any())
        //    {
        //        return;
        //    }

        //    await base.ProcessAsync(context, output);
        //}

        internal static string RenderComponentTag(TagHelperContext context, string componentName)
        {
            var propertiesToExclude = new[] { nameof(ClassName), nameof(TargetId) }.Select(
                x => x.ToKebabCase()).ToArray();
            var attributes = context.AllAttributes.Where(
                a => !propertiesToExclude.Contains(a.Name)).Select(
                a =>
                {
                    var value = a.Value;
                    if (value is HtmlString html)
                    {
                        value = html.Value;
                    }
                    if (value is string x && !(x.StartsWith("{") && x.EndsWith("}")))
                    {
                        value = x.SurroundWithQuotes();
                    }

                    return $"{a.Name}={value}";
                }).Aggregate(" ");
            return $"<{componentName} {attributes}/>";
        }
        protected override string GetReactContent(ReactArtifactContext context)
        {

            var componentTag = RenderComponentTag(context.TagHelperContext, this.ClassName);
            return $@"ReactDOM.render(
{componentTag},
document.getElementById('{this.FinalTargetId}'));";
        }

        protected override string GetRazorContent(ReactArtifactContext context)
        {
            if (this.TargetId == null)
            {
                return $@"<div id=""{this.FinalTargetId}""></div>";
            }
            return base.GetRazorContent(context);
        }        
    }
}