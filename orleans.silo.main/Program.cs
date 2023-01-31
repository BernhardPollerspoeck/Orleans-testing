
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using System.Net;
using orleans.extensions;

var builder = Host.CreateDefaultBuilder(args);
builder.UseOrleans(siloBuilder =>
{
	siloBuilder
		.ConfigureStaticClusterPrimarySilo(
			advestisedIp: IPAddress.Loopback,
			siloPort: 25000,
			gatewayPort: 26000)
		.Configure<SiloOptions>(o =>
		{
			o.SiloName = "Confused Pidgeon";
		})
		.Configure<ClusterOptions>(o =>
		{
			o.ClusterId = "apsiba.workers";
			o.ServiceId = "apsiba";
		})
		.UseDashboard(x => x.HostSelf = true);
});
var host = builder.Build();
await host.RunAsync();



