
# Introduction

This guide aims to show the basic steps, required to build a static cluster of silos.
This scenario requires one silo to be the primary silo, that all other silos and clients connect to. To keep the load of this single point low, this primary silo just acts as a gateway and doesnt host grains on itself.

The whole guide consists of 5 Projects, that are described in the following chapters.


# Grains Projects
There are 2 grains projects. *orleans.grains.abstraction* for the grain interfaces and *orleans.grains* for the actual implementations.
The interfaces and implementations are seperated, to be able to restrict access to the implementations for the client and primary silo.
> IMPORTANT: both projects need to reference **Microsoft.Orleans.Sdk** for the silos be able to discover the grains

 #### orleans.grains.abstraction
 Here are all the interfaces of the grains defined. Our example grain will add up 2 numbers and return the result as a Task
> IMathGrain.cs
```C#
public interface IMathGrain : IGrainWithStringKey
{
	Task<int> Add(int a, int b);
}
```

#### orleans.grains
Here are all grain implementations. Note, the grains inherit from **Grain** and implement our defined interface
> MathGrain.cs
```c#
public class MathGrain : Grain, IMathGrain
{
	public Task<int> Add(int a, int b)
	{
		return Task.FromResult(a + b);
	}
}
```


# Primary Silo
The primary silo is our entrypoint for the clients and connection target for all other silos. Since it doesnt execute grains by itsef it acts more or less like a proxy into and out from the cluster. This silo project has no references to the actual grain Implementations, just the interfaces.

#### orleans.silo.main
Starting with a HostBuilder, with the help of *UseOrleans* we use the SiloBuilder to configure our Silo.
First we configure all ports and our listening ip with *ConfigureStaticClusterPrimarySilo*. Then a optional name, followed by the identification of our cluster and service. As last ive added the Dashboard, to enable some cool informations to look up.
Finally we create a host and run it. Our primary silo now is ready and can be started.
> Program.cs
```c#
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
			o.SiloName = "More primary than any school";
		})
		.Configure<ClusterOptions>(o =>
		{
			o.ClusterId = "the most static cluster on earth";
			o.ServiceId = "my cool orleans service";
		})
		.UseDashboard(x => x.HostSelf = true);
});
var host = builder.Build();
await host.RunAsync();
```


# Secondary Silos
The secondary silos are responsible to execute the grains. They are the only project that gets a reference to the grain implementations in *orleans.grains*.

#### orleans.silo
Pretty similar to the main silo we use a HostBuilder and again with the use of *UseOrleans* we access the silo builder. In there just the cluster configuration method changed. *ConfigureStaticClusterSecondarySilo* requires us again to declare all nessecary ports and our listening ip. Here we also need to specify the ip and port of our already running primary silo.
Then the usual host creation and run. 
Assign each secondary silo instance a unique name, and if you run them locally a unique port. Now you can run as many silos as you want or need. Each one will contact the primary silo and will be able to accept requests from there.
> Program.cs
```c#
var builder = Host.CreateDefaultBuilder(args);
builder.UseOrleans(siloBuilder =>
{
	siloBuilder
		.ConfigureStaticClusterSecondarySilo(
			advestisedIp: IPAddress.Loopback,
			siloPort: 25001,
			gatewayPort: 26000,
			primarySiloPort: IPEndPoint.Parse("127.0.0.1:25000"))
		.Configure<SiloOptions>(o =>
		{
			o.SiloName = "secondary means more power this time";
		})
		.Configure<ClusterOptions>(o =>
		{
			o.ClusterId = "the most static cluster on earth";
			o.ServiceId = "my cool orleans service";
		});
});
var host = builder.Build();
await host.RunAsync();
```

# Client
A minimalistic asp.net.core api will act as a client in this example. *asp.net.core* and *minimal api* parts of this example are assumed to be known. If not please reffer to the according docs.

#### orleans.api
Here we use the HostBuilder supplied by the template, to tell the host to *UseOrleansClient*. In there via *UseStaticClustering* we supply the main silo access point. Now we map a simple GET route that accepts the 2 input numbers and a *IClusterClient*. From this client we retreive the grain we want to execute. Then the grain can be used like any other async method with the await keyword. As last we return the result.
> Program.cs
```c#
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
		return Results.Ok(result);
	});
app.Run();
```

# Conclusion
With relative low effort we now are able to create a fully static cluster. This certainly gives the most control, but has its drawbacks. 
- The main silo has to be always reachable from the cluster and aswell from the clients
- All traffict flows through a single silo. This potentially creates a bottleneck.
It is reccomended to use the dynamic clustering approaches where possible, but in specific, usually development related, scenarios this static cluster approach has possible benefits.



