using System;
using System.Threading.Tasks;
using DotNetify;
using Microsoft.AspNetCore.Razor.TagHelpers;
using TomSun.AspNetCore.Extensions.SharpComponents;
using TomSun.AspNetCore.RazorReact.TagHelpers;

namespace TomSun.AspNetCore.RazorReact
{
    public abstract class ReactViewComponent<TSelf, TParameter, TViewModel> : SharpViewComponent<TSelf, TParameter>
        where TSelf: ReactViewComponent<TSelf, TParameter, TViewModel> where TViewModel : BaseVM
    {
        public new static ViewAbstraction View { get; } = new ViewAbstraction();
        public class ViewAbstraction : ReactView<ViewAbstraction, TViewModel, TParameter>
        {
            
        }

        public new class ComponentTagHelper : SharpViewComponent<TSelf, TParameter>.ComponentTagHelper, IReactTagHelper
        {
            public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
            {
                await ReactTagHelper.DoProcessAsync(this, context, output,
                    async ()=> (await RenderSync(this.Parameter)).GetHtmlString());
            }

            Type IReactTagHelper.ComponentType { get; } = typeof(TViewModel);
            bool IReactTagHelper.Render { get; } = true;
        }
    }
}