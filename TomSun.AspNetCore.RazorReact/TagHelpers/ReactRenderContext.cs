using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Razor.TagHelpers;
using TomSun.Portable.Factories;

namespace TomSun.AspNetCore.RazorReact.TagHelpers
{
    public class ReactRenderContext
    {
        public List<TagHelper> Scope { get; } = new List<TagHelper>();
        public List<string> RenderedTypes { get; } = new List<string>();

        public IDisposable AddScope(TagHelper tagHelper)
        {
            this.Scope.Add(tagHelper);
            return Api.Create.Disposable(() => this.Scope.Remove(tagHelper));
        }

        public IDisposable Process(Action<ContentReceivedEventArgs> processing)
        {
            this.ReactContentProcessors.Push(processing);
            return Api.Create.Disposable(() => this.ReactContentProcessors.Pop());
        }
        public Stack<Action<ContentReceivedEventArgs>> ReactContentProcessors = new Stack<Action<ContentReceivedEventArgs>>();
        public Action<ContentReceivedEventArgs> RazorContentReceived;
        public void AddReactContent(ReactArtifactTagHelper reactArtifactTagHelper, string content)
        {
            if (reactArtifactTagHelper?.ProducesGlobalReactContent != true && this.ReactContentProcessors.TryPeek(out var currentProcessor))
            {
                content += Environment.NewLine;
                var eventArgs = new ContentReceivedEventArgs()
                {
                    Content = content,
                    Owner = reactArtifactTagHelper,
                    Handled = false,
                };
                currentProcessor(eventArgs);
            }
            else
            {
                this.ReactContent.Add((0, content));
            }
           
        }
        public void AddRazorContent(ReactArtifactTagHelper reactArtifactTagHelper, string content)
        {
            this.RazorContent += this.ProcessContent(
                this.RazorContentReceived, reactArtifactTagHelper, content);
        }
        private string ProcessContent(Action<ContentReceivedEventArgs> receivedEvent,
            ReactArtifactTagHelper owner,
            string content)
        {
            content += Environment.NewLine;
            var eventArgs = new ContentReceivedEventArgs()
            {
                Content = content,
                Owner = owner,
                Handled = false,
            };
            receivedEvent?.Invoke(eventArgs);
            if (!eventArgs.Handled)
            {
                return content;
            }
            return null;
        }

        public IList<(int Priority, string content)> ReactContent { get; } = new List<(int Priority, string content)>();
        public string RazorContent { get; private set; } = string.Empty;
    }

    public class ContentReceivedEventArgs : EventArgs
    {
        public string Content { get; internal set; }
        public ReactArtifactTagHelper Owner { get; internal set; }

        public bool Handled { get; set; }
    }
}