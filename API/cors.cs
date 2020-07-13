using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.MiddlewareAnalysis;

using Microsoft.AspNetCore.Http;


public class CorsMiddleware
{
    private readonly RequestDelegate _next;

    public CorsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = 200;
        httpContext.Response.Headers.Add("Access-Control-Allow-Headers", "content-type");
        httpContext.Response.Headers.Add("Access-Control-Allow-Origin", httpContext.Response.Headers["Origin"]);
        httpContext.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
        httpContext.Response.Headers.Add("Access-Control-Allow-Methods", "POST,GET,OPTIONS");
        httpContext.Response.Headers.Add("Content-Type", "application/json");
        return _next(httpContext);
    }
}

// Extension method used to add the middleware to the HTTP request pipeline.
public static class CorsMiddlewareExtensions
{
    public static IApplicationBuilder UseCorsMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CorsMiddleware>();
    }
}