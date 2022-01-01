using IdentityDemoAPI.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace IdentityDemoAPI.BackgroundServices
{
    public class TimedHostedServiceTest : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly string connectionString;
        private Timer Timer { set; get; }
        public TimedHostedServiceTest(ILogger<TimedHostedServiceTest> logger, IConfiguration config)
        {
            _logger = logger;
            connectionString = config.GetConnectionString("AppDbContextConnection");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var serviceInfo = new
                {
                    Id = Guid.NewGuid(),
                    Content = "Timed background service fired ",
                    FiredTime = DateTime.Now,
                    Name = "Test service"
                };
                string sql =
                    "INSERT INTO BackgroundServices (Id, Content, FiredTime, Name)" +
                    " VALUES (@id, @content, @firedTime, @name) ";
                connection.Execute(sql, serviceInfo);
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            Timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
        public void Dispose()
        {
            Timer.Dispose();
        }
    }
}
