using Consul;
using Microsoft.AspNetCore.Mvc;
using VaultService.Consul;

[assembly: ApiController]

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<IHostedService, ConsulHostedService>();

builder.Services.Configure<ConsulConfig>(builder.Configuration.GetSection("consulConfig"));

builder.Services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
{
    var address = builder.Configuration["consulConfig:address"];
    consulConfig.Address = new Uri(address);
}));

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseAuthorization();

app.MapControllers();

app.Run();