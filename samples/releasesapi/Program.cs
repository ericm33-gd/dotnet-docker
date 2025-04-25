
using System;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.Threading;

using System.Text.Json.Serialization;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using ReleaseApi;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace ReleaseApi
{
    [ExcludeFromCodeCoverage(Justification = "just program startup")]
    public static class Program
    {
        private static readonly NLog.ILogger Logger = NLog.LogManager.GetCurrentClassLogger();
         
        public static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var builder = WebApplication.CreateBuilder(args);

            // load config
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables($"{AppDomain.CurrentDomain.FriendlyName}:")
                .AddJsonFile("/config/app-config.json", optional: true, reloadOnChange: true); // azure app config mounts here
            
            // adjust thread pool size
            //ThreadPoolHelper.AdjustThreadPoolSize(builder.Configuration);


            // add logging
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddNLog(builder.Configuration);
            });

            // register logger
            builder.Services.AddSingleton(Logger);

            Logger.Info("ReleasesAPI-CN-StartUp Begins...");


            var app = builder.Build();

            //app.MapHealthChecks("/healthz");

            app.MapGet("/releases", async () => await ReleaseReport.Generator.MakeReportAsync());


            // Configure the HTTP request pipeline.
            // app.UseDefaultHealthCheckUrls();
            // app.UseSwagger();
            // app.UseSwaggerUI();

            // app.UseAuthentication();
            // app.UseAuthorization();
            // app.MapControllers();

            // Get managed memory usage
            long managedMemory = GC.GetTotalMemory(false);
            Logger.Info("ReleasesAPI-CN-StartUp-ManagedMemoryUsedRaw: {managedMemory}", managedMemory);            
            managedMemory = ((managedMemory / 1024) / 1024); // Convert to MB
            Logger.Info("ReleasesAPI-CN-StartUp-ManagedMemoryUsedMB: {managedMemory}", managedMemory);
            Logger.Info("ReleasesAPI-CN-StartUp-Ends...");

            stopwatch.Stop();
            var elapsedTime = stopwatch.ElapsedMilliseconds;

            Logger.Info("ReleasesAPI-CN-Startup-ElapsedTime: {elapsedTime}", elapsedTime); 

            app.Run();
        }
    }
}


// using System.Text.Json.Serialization;
// using System.Diagnostics;
// using Microsoft.Extensions.Logging;
// using NLog.Extensions.Logging;

// using Microsoft.AspNetCore.Builder;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;
// using System.Text.Json;
// using System.Text.Json.Serialization;
// using System.Diagnostics;
// using ReleaseApi;




// var builder = WebApplication.CreateSlimBuilder(args);
// builder.Services.AddHealthChecks();

// builder.Services.ConfigureHttpJsonOptions(options =>
// {
//     options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
// });

// var app = builder.Build();

// app.MapHealthChecks("/healthz");

// app.MapGet("/releases", async () => await ReleaseReport.Generator.MakeReportAsync());

// Stopwatch stopwatch = new Stopwatch();
// stopwatch.Start();

// Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Releases-API Begins...");

// long managedMemory = GC.GetTotalMemory(false);
// //managedMemory = ((managedMemory / 1024) / 1024); // Convert to MB

// Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Managed Memory Used (MB): " + managedMemory);

// Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Releases-API-StartUp Ends...");
// stopwatch.Stop();

// var elapsedTime = stopwatch.ElapsedMilliseconds;
// Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Releases-API-StartUp Elapsed Time: " + elapsedTime); 

// Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Releases-API Ends...");
// app.Run();


// [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.KebabCaseLower)]
// [JsonSerializable(typeof(ReportJson.Report))]
// internal partial class AppJsonSerializerContext : JsonSerializerContext
// {
// }
