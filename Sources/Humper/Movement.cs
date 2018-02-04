namespace Humper
{
	using Base;
	using System.Collections.Generic;
	using System.Linq;

	public class Movement : IMovement
	{
		public Movement()
		{
			this.Hits = new IHit[0];
		}

		public IEnumerable<IHit> Hits { get; set; }

		public bool HasCollided { get { return this.Hits.Any(); } }

		public RectangleF Origin { get; set; }

		public RectangleF Destination { get; set; }

		public RectangleF Goal { get; set; }
	}
}

