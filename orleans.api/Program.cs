using orleans.grains.abstraction;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleansClient(clientBuilder =>
{
	clientBuilder.UseLocalhostClustering(25000, "apsiba", "apsiba.workers");
});


var app = builder.Build();

app.MapGet("/add/{a}/{b}",
	async (IClusterClient grains, HttpRequest request, int a, int b) =>
	{
		var addGrain = grains.GetGrain<IMathGrain>("1");
		var result = await addGrain.Add(a, b);
		return Results.Ok(result);
	});

app.Run();
