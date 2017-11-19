using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using TomSun.AspNetCore.Extensions.SharpComponents;
using TomSun.AspNetCore.RazorReact.TagHelpers;

namespace TomSun.AspNetCore.RazorReact
{
    public abstract class ReactViewComponent<TSelf, TView, TProps, TState> : ReactViewComponent<TSelf, TView, TProps>,
        IReactViewComponentWithViewModel
        where TSelf : ReactViewComponent<TSelf, TView, TProps, TState>
        where TView : ReactView<TView, TState, TProps>, new()
        where TProps : new()
        where TState : new()
    {
        public abstract class Page : ReactRazorViewComponentPage<TSelf, TView, TProps,TState>
        {
            
        }


        public new class BaseView : ReactView<TView, TState, TProps>
        {

        }
    }

    internal interface IReactViewComponentWithViewModel : IReactViewComponent
    {

    }

    internal interface IReactViewComponent
    {
    }

    public abstract class ReactViewComponent<TSelf, TView, TProps> : SharpViewComponent<TSelf, TProps>,
        IReactViewComponent
        where TSelf : ReactViewComponent<TSelf, TView, TProps>
        where TView : new()
        where TProps : new()
    {
        public new static TView View { get; } = new TView();

        public class BaseView : ReactView<TView, object, TProps>
        {

        }

        public async Task<IViewComponentResult> DoInvokeAsync(ITuple parameter)
        {
            return await base.DoInvokeAsync(parameter.Convert<TProps>());
        }
    }
}