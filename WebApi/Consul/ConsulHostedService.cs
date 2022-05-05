using Consul;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Sockets;

namespace WebApi.Consul
{
    public class ConsulHostedService : IHostedService
    {
        private Task _executingTask;
        private CancellationTokenSource _cts;
        private readonly IConsulClient _consulClient;
        private readonly IOptions<ConsulConfig> _consulConfig;
        private readonly ILogger<ConsulHostedService> _logger;
        private readonly IServer _server;
        private string _registrationID;

        public ConsulHostedService(IConsulClient consulClient, IOptions<ConsulConfig> consulConfig, ILogger<ConsulHostedService> logger, IServer server)
        {
            _server = server;
            _logger = logger;
            _consulConfig = consulConfig;
            _consulClient = consulClient;

        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Create a linked token so we can trigger cancellation outside of this token's cancellation
            var port = 5194;
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _registrationID = $"{_consulConfig.Value.ServiceID}-{port}";

            var registration = new AgentServiceRegistration()
            {
                ID = _registrationID,
                Name = _consulConfig.Value.ServiceName,
                Address = "http://localhost",
                Port = port
            };

            _logger.LogInformation("Registering in Consul");
            await _consulClient.Agent.ServiceDeregister(registration.ID, _cts.Token);
            await _consulClient.Agent.ServiceRegister(registration, _cts.Token);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cts.Cancel();
            _logger.LogInformation("Deregistering from Consul");
            try
            {
                await _consulClient.Agent.ServiceDeregister(_registrationID, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Deregisteration failed");
            }
        }
    }
}
