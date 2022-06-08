using Consul;
using Microsoft.AspNetCore.Mvc;
using MockService.Consul;
using StackExchange.Redis;
using zipkin4net;
using zipkin4net.Middleware;
using zipkin4net.Tracers.Zipkin;
using zipkin4net.Transport.Http;

var builder = WebApplication.CreateBuilder(args);

var applicationName = builder.Configuration["consulConfig:serviceName"];

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IDatabase>(x =>
{
    var db = ConnectionMultiplexer.Connect("redis");
    return db.GetDatabase();
});

builder.Services.AddSingleton<IHostedService, ConsulHostedService>();

builder.Services.Configure<ConsulConfig>(builder.Configuration.GetSection("consulConfig"));
builder.Services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
{
    var address = builder.Configuration["consulConfig:address"];
    consulConfig.Address = new Uri(address);
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

var lifetime = app.Services.GetService<IHostApplicationLifetime>();
lifetime.ApplicationStarted.Register(() => {
    TraceManager.SamplingRate = 1.0f;
    var logger = new TracingLogger(app.Services.GetRequiredService<ILoggerFactory>(), "zipkin4net");
    var httpSender = new HttpZipkinSender("http://zipkin:9411", "application/json");
    var tracer = new ZipkinTracer(httpSender, new JSONSpanSerializer());
    TraceManager.RegisterTracer(tracer);
    TraceManager.Start(logger);
});
lifetime.ApplicationStopped.Register(() => TraceManager.Stop());
app.UseTracing(applicationName);

app.UseAuthorization();

app.MapControllers();

app.Run();
