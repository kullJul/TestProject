using Microsoft.AspNetCore.Mvc;
using zipkin4net;

namespace WebApi.Controllers
{
    public class MyController : ControllerBase
    {
        protected readonly IConfiguration configuration;
        public MyController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public void TraceLog(string method, string? message = null)
        {
            var trace = Trace.Create();
            trace.Record(Annotations.ServerRecv());
            trace.Record(Annotations.ServiceName(configuration["consulConfig:serviceName"]));
            trace.Record(Annotations.Rpc(method));
            trace.Record(Annotations.Event(message));
            trace.Record(Annotations.ServerSend());
        }
    }
}
