using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(builder.Configuration["JWTConfig:AuthenticationProviderKey"], options =>
    {
       // options.Authority = builder.Configuration["JWTConfig:Authority"];
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JWTConfig:Issuer"],
            ValidateAudience = true,
            ValidAudience = "webapi", 
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTConfig:Key"])),
            ValidateIssuerSigningKey = true
        };
    });

builder.Configuration.AddJsonFile("ocelot.json");
builder.Services.AddOcelot();

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseOcelot();

app.Run();
