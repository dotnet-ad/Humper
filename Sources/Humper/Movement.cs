namespace Humper
{
	using Base;
	using System.Collections.Generic;
	using System.Linq;

	public class Movement : IMovement
	{
		public Movement()
		{
			this.Collisions = new ICollision[0];
		}

		public IEnumerable<ICollision> Collisions { get; set; }

		public bool HasCollided { get { return this.Collisions.Any((cw) => cw.HasCollided); } }

		public RectangleF Origin { get; set; }

		public RectangleF Destination { get; set; }

	}
}

