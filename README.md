
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

# Secondary Silos

# Client


