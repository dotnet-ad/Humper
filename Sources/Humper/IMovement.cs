namespace Humper
{
	using System.Collections.Generic;
	using Base;

	public interface IMovement
	{
		IEnumerable<IHit> Hits { get; }

		bool HasCollided { get; }

		RectangleF Origin { get; }

		RectangleF Destination { get; }
	}
}

