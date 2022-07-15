namespace cmdwtf.UnityTools.Dynamics
{
	public interface IDynamicsTransformComponent
	{

	}

	public interface IDynamicsTransformComponent<T>
		: IDynamicsTransformComponent where T : struct
	{
		T Value { get; }

		void Reset(T resetValue);

		T Update(float deltaTime, T current, T target);
	}
}
