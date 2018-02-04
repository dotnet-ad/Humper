namespace Humper
{
	using Base;

	public interface ICollision
	{
		IBox Box { get; }

		IBox Other { get; }

		RectangleF Origin { get; }

		RectangleF Goal { get; }

		Hit Hit { get; }

		bool HasCollided { get; }
	}
}

