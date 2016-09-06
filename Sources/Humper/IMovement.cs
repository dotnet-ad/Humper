namespace Humper
{
	using System.Collections.Generic;
	using Base;

	public interface IMovement
	{
		IEnumerable<IHit> Hits { get; }

		bool HasCollided { get; }

		RectangleF Origin { get; }

		RectangleF Goal { get; }

		RectangleF Destination { get; }
	}
}

