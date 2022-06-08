using Consul;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Sockets;
using System.Text;
using WebApi.Consul;
using WebApi.Refit;
using zipkin4net;
using zipkin4net.Middleware;
using zipkin4net.Tracers.Zipkin;
using zipkin4net.Transport.Http;

var builder = WebApplication.CreateBuilder(args);

var applicationName = builder.Configuration["consulConfig:serviceName"];

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(builder.Configuration["JWTConfig:AuthenticationProviderKey"], options =>
{
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWTConfig:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWTConfig:Audience"],
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTConfig:Key"])),
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddControllers();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

