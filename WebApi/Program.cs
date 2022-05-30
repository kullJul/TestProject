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

[assembly: ApiController]

var builder = WebApplication.CreateBuilder(args);

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

