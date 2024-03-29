﻿


using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using System.Net;
using orleans.extensions;

var builder = Host.CreateDefaultBuilder(args);

builder.UseOrleans(siloBuilder =>
{
	siloBuilder
		.ConfigureStaticClusterSecondarySilo(
			advestisedIp: IPAddress.Parse("127.0.0.1"),
			siloPort: 25001,
			gatewayPort: 26000,
			primarySiloPort: IPEndPoint.Parse("127.0.0.1:25000"))
		.Configure<SiloOptions>(o =>
		{
			o.SiloName = "Undefeated Lunch";
		})
		.Configure<ClusterOptions>(o =>
		{
			o.ClusterId = "apsiba.workers";
			o.ServiceId = "apsiba";
		});
});

var host = builder.Build();

await host.RunAsync();

