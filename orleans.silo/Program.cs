
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using Orleans.Hosting;
using System.Net;

var builder = Host.CreateDefaultBuilder(args);

builder.UseOrleans(siloBuilder =>
{
	siloBuilder.ConfigureEndpoints(IPAddress.Loopback, 25500, 25000);
	siloBuilder.UseDevelopmentClustering((IPEndPoint?)null);
	siloBuilder.Configure<ClusterOptions>(o =>
	{
		o.ClusterId = "apsiba.workers";
		o.ServiceId = "apsiba";
	});
});

var host = builder.Build();

await host.RunAsync();



