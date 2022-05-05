using Consul;
using Microsoft.AspNetCore.Mvc;
using MockService.Consul;
using StackExchange.Redis;

[assembly: ApiController]

var builder = WebApplication.CreateBuilder(args);

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

app.UseAuthorization();

app.MapControllers();

app.Run();
