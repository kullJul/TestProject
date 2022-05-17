using Consul;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using WebApi.Consul;
using WebApi.Refit;
using MediatR;
using WebApi.CQRS.Commands.RegisterAccount;
using WebApi.CQRS;

[assembly: ApiController]

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<IHostedService, ConsulHostedService>();

builder.Services.Configure<ConsulConfig>(builder.Configuration.GetSection("consulConfig"));

builder.Services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
{
    var address = builder.Configuration["consulConfig:address"];
    consulConfig.Address = new Uri(address);
}));

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

builder.Services.AddScoped(typeof(RegisterAccountCommand), typeof(RegisterAccountCommandHandler));

var handlers = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsSubclassOf(typeof(IHandler)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

