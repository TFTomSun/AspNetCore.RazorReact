using System;
using System.Collections.Generic;
using System.Text;
using DotNetify;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;


public static class Extensions
{
    public static void UseRazorReact(this IServiceCollection services)
    {
        services.AddSignalR();
        services.AddDotNetify();
    }

    public static void UseRazorReact(this IApplicationBuilder app)
    {
        app.UseWebSockets();
        app.UseSignalR(routes => routes.MapDotNetifyHub());
        app.UseDotNetify();
    }
}

