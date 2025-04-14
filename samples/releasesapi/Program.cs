using System.Text.Json.Serialization;
using System.Diagnostics;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Services.AddHealthChecks();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();

app.MapHealthChecks("/healthz");

app.MapGet("/releases", async () => await ReleaseReport.Generator.MakeReportAsync());

Stopwatch stopwatch = new Stopwatch();
stopwatch.Start();

Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Releases-API Begins...");

long managedMemory = GC.GetTotalMemory(false);
//managedMemory = ((managedMemory / 1024) / 1024); // Convert to MB

Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Managed Memory Used (MB): " + managedMemory);

Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Releases-API-StartUp Ends...");
stopwatch.Stop();

var elapsedTime = stopwatch.ElapsedMilliseconds;
Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Releases-API-StartUp Elapsed Time: " + elapsedTime); 

Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Releases-API Ends...");
app.Run();


[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.KebabCaseLower)]
[JsonSerializable(typeof(ReportJson.Report))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}
