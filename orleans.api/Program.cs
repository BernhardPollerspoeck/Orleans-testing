using orleans.grains.abstraction;
using Orleans.Configuration;
using Orleans.Runtime;
using System.Dynamic;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleansClient(clientBuilder =>
{
	clientBuilder.UseStaticClustering(IPEndPoint.Parse("127.0.0.1:26000"));
	clientBuilder.Configure<ClusterOptions>(o =>
	{
		o.ClusterId = "apsiba.workers";
		o.ServiceId = "apsiba";
	});
});


var app = builder.Build();

app.MapGet("/add/{a}/{b}",
	async (
		IClusterClient grains,
		int a,
		int b) =>
	{
		var addGrain = grains.GetGrain<IMathGrain>("1");
		var result = await addGrain.Add(a, b);

		var manager = grains.GetGrain<IManagementGrain>(111);
		var hosts = await manager.GetDetailedHosts();

		var g = await manager.GetSimpleGrainStatistics();
		var gg = await manager.GetDetailedGrainStatistics();
		var ggg = await manager.GetActivationAddress(addGrain);

		return Results.Json(new
		{
			MathResult = result,
			Silos = hosts.Select(h => new
			{
				Name = h.SiloName,
				Adress = h.SiloAddress,
				Status = h.Status.ToString(),
				Role = h.RoleName,
			}),
			ExecutingSilo = hosts.First(h => h.SiloAddress.Endpoint == ggg.Endpoint).SiloName,
		});
	});

app.Run();
