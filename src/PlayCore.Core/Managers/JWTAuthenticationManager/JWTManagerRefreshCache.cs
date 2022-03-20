using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PlayCore.Core.Managers.JWTAuthenticationManager
{
    class JwtManagerRefreshCache : IHostedService
    {
        private Timer _timer;
        private readonly IJwtAuthenticationManager _jwtAuthenticationManager;

        public JwtManagerRefreshCache(IJwtAuthenticationManager jwtAuthenticationManager)
        {
            _jwtAuthenticationManager = jwtAuthenticationManager;
        }
        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(60));
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            _timer?.Dispose();
            return Task.CompletedTask;
        }
        private void DoWork(object state)
        {
            _jwtAuthenticationManager.RemoveExpiredRefreshTokens(DateTime.Now);
        }
    }
}
