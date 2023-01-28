using orleans.grains.abstraction;
using Orleans;

namespace orleans.grains;

public class MathGrain : Grain, IMathGrain
{
	public Task<int> Add(int a, int b)
	{
		return Task.FromResult(a + b);
	}

}