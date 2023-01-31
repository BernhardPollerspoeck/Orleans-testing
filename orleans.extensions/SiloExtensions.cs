using Microsoft.Extensions.Hosting;
using Orleans.Hosting;
using System.Net;

namespace orleans.extensions;


public static class SiloExtensions
{

	public static ISiloBuilder ConfigureStaticClusterPrimarySilo(
		this ISiloBuilder builder,
		IPAddress advestisedIp,
		ushort siloPort,
		ushort gatewayPort)
	{
		builder
			.ConfigureEndpoints(advestisedIp, siloPort, gatewayPort)
			.UseDevelopmentClustering((IPEndPoint?)null);

		return builder;
	}

	public static ISiloBuilder ConfigureStaticClusterSecondarySilo(
		this ISiloBuilder builder,
		IPAddress advestisedIp,
		ushort siloPort,
		ushort gatewayPort, 
		IPEndPoint? primarySiloPort)
	{
		builder
			.ConfigureEndpoints(advestisedIp, siloPort, gatewayPort)
			.UseDevelopmentClustering(primarySiloPort);

		return builder;
	}



}
