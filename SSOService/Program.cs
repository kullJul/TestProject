using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using zipkin4net;
using zipkin4net.Middleware;
using zipkin4net.Tracers.Zipkin;
using zipkin4net.Transport.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var applicationName = builder.Configuration["consulConfig:serviceName"];

var app = builder.Build();

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

app.Map("/login/{username}", (string username) =>
{
    var claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };
    
    var jwt = new JwtSecurityToken(        
            issuer: builder.Configuration["JWTConfig:Issuer"],
            audience: builder.Configuration["JWTConfig:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTConfig:Key"])), SecurityAlgorithms.HmacSha256));
    var trace = Trace.Create();
    trace.Record(Annotations.ServerRecv());
    trace.Record(Annotations.ServiceName(applicationName));
    trace.Record(Annotations.Event($"Getting authorization token = {jwt}"));
    trace.Record(Annotations.ServerSend());
    return new JwtSecurityTokenHandler().WriteToken(jwt);
});

app.Run();

