using Orleans;

namespace orleans.grains.abstraction;

public interface IMathGrain : IGrainWithStringKey
{
	Task<int> Add(int a, int b);
}