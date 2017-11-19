using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using TomSun.Common.SystemExtensionMethods.Explicit;
using TomSun.Portable.Factories;

namespace TomSun.AspNetCore.RazorReact.TagHelpers
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class ReactViewComponentProviderAttribute : Attribute
    {
        
    }
    [HtmlTargetElement("*")]
    public class ReactViewComponentTagHelper : TagHelper
    {
        public override int Order => 10;
        private static ExtendableObject StaticCache { get; } = new ExtendableObject();
        
        private IDictionary<string, Type> ReactViewComponentMap => StaticCache.GetMemberLazy(() =>
        {
            var componentAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(
                a=>a.GetCustomAttribute<ReactViewComponentProviderAttribute>() != null).Concat(
                Assembly.GetEntryAssembly()).ToArray();

            var types = componentAssemblies.SelectMany(a =>
                a.GetTypes().Where(t => typeof(IReactViewComponent).IsAssignableFrom(t))).ToArray();
            return types.ToDictionary(t => "vc:"+ t.Name.ToKebabCase());
        });
        public override void Init(TagHelperContext context)
        {
            if (this.ReactViewComponentMap.TryGetValue(context.TagName, out var componentType))
            {
                this.ComponentType = componentType;
                this.HasViewModel = typeof(IReactViewComponentWithViewModel).IsAssignableFrom(componentType);
            }

            base.Init(context);
        }
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (this.ComponentType != null)
            {
                (await output.GetChildContentAsync()).NotNull();
                var renderAttributes = context.AllAttributes
                    .Where(a => a.Value != null && (a.Value as string)?.IsNullOrEmpty() != true).Select(
                        a =>
                        {
                            var value = a.Value;
                            var pascalCase = a.Name.ToPascalCase();
                            return new TagHelperAttribute(pascalCase, value);
                        });

                var className = this.ComponentType.Name;
                if (this.ReactRenderContext().Scope.OfType<ReactRenderFuncionTagHelper>().Any())
                {
                    var renderTagHelperContext = new TagHelperContext(className,
                        new TagHelperAttributeList(renderAttributes),
                        context.Items, Guid.NewGuid().ToString());

                    var tag = ReactRenderTagHelper.RenderComponentTag(renderTagHelperContext, className);
                    output.PostElement.SetHtmlContent(new HtmlString(tag));
                    output.TagName = null;
                    output.Content.SetContent(null);
                }
                else
                {
                    var renderTagHelper = new ReactRenderTagHelper
                    {
                        ClassName = className
                    };

                    var renderTagHelperContext = new TagHelperContext("react-render",
                        new TagHelperAttributeList(renderAttributes),
                        context.Items, Guid.NewGuid().ToString());

                    await renderTagHelper.ProcessAsync(renderTagHelperContext, output);
                }
            }
        }
        public Type ComponentType { get; private set; }
        public bool HasViewModel { get; private set; }
    }
}