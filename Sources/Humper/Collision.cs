using Humper.Base;

namespace Humper
{
	public class Collision : ICollision
	{
		public Collision()
		{
		}

		public IBox Box { get; set; }

		public IBox Other { get { return this.Hit?.Box; } }

		public RectangleF Origin { get; set; }

		public RectangleF Goal { get; set; }

		public IHit Hit { get; set; }

		public bool HasCollided => this.Hit != null;
	}
}

