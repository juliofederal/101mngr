﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans;
using Orleans.Clustering.Kubernetes;
using _101mngr.Grains;
using _101mngr.Leagues;
using IHostingEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;

namespace _101mngr.Host
{
    public class SiloHost : IHostedService
    {
        private readonly ISiloHost _silo;

        public SiloHost(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            _silo = new SiloHostBuilder()
                .AddAdoNetGrainStorage("OrleansStorage", options =>
                {
                    options.Invariant = "Npgsql";
                    options.ConnectionString = connectionString;
                    options.UseJsonFormat = true;
                })
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "101mngr";
                    options.ServiceId = "101mngr";
                })
                .ConfigureEndpoints(siloPort: 11111, gatewayPort: 30000)
                .ConfigureServices(services =>
                {
                    services.AddSingleton<LeagueDbContext>();
                    services.AddSingleton<LeagueService>();
                })
                .ConfigureClustering(hostingEnvironment)
                .ConfigureApplicationParts(parts =>
                    parts.AddApplicationPart(typeof(PlayerGrain).Assembly).WithReferences())
                .ConfigureLogging(builder => builder.AddConsole())
                .UseDashboard(options => { options.CounterUpdateIntervalMs = 10000; })
                .Build();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _silo.StartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _silo.StopAsync(cancellationToken);
        }
    }

    public static class OrleansExt
    {
        public static ISiloHostBuilder ConfigureClustering(
            this ISiloHostBuilder builder, IHostingEnvironment hostingEnvironment)
        {
            if (hostingEnvironment.IsDevelopment())
            {
                return builder.UseLocalhostClustering();
            }
            else
            {
                return builder.UseKubeMembership(opt =>
                {
                    opt.Group = "orleans.dot.net";
                    opt.CanCreateResources = true;
                });
            }
        }
    }
}