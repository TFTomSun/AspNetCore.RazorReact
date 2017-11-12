#Razor React 

This project is based on DotNetify.React and provides tag helpers for defining react components within ASP.NET Core Razor Pages. 

Getting Started

* Create a ASP.NET Core 2.0 Web Application (No React, No MVC, simple Web Application)
* Install this nuget package
* Extend the Startup.cs file:

    ```csharp
        public void ConfigureServices(IServiceCollection services)
        {
            services.UseRazorReact(); // <--- Add this line
            services.AddMvc();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseRazorReact(); // <--- Add this line

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
            
        }
    ```
    
    * Create the react component HelloWorld.cs
    ```csharp
         public class HelloWorld : ReactComponent<HelloWorld>
    {
        private Timer _timer;
        public string Greetings => "Hello World!";
        public DateTime ServerTime => DateTime.Now;

        public HelloWorld()
        {
            _timer = new Timer(state =>
            {
                Changed(nameof(ServerTime));
                PushUpdates();
            }, null, 0, 1000);
        }
        public override void Dispose() => _timer.Dispose();
    }
    ```
    
    * Replace the content of the Index.cshtml razor file
    ```html
    @page
    @addTagHelper "*, TomSun.AspNetCore.RazorReact"
    @model IndexModel
    @{
        ViewData["Title"] = "Home page";
    }
    <react-scripts/>

    <react component-type="@typeof(HelloWorld)">
        <div>
            Hi @HelloWorld.Bind(x => x.state.Greetings)<br />
            Server time is: @HelloWorld.Bind(x => x.state.ServerTime)
        </div>
    </react>
    ```
