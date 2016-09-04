namespace Humper
{
	using System.Collections.Generic;
	using Base;

	public interface IMovement
	{
		IEnumerable<ICollision> Collisions { get; }

		bool HasCollided { get; }

		RectangleF Origin { get; }

		RectangleF Destination { get; }
	}
}

