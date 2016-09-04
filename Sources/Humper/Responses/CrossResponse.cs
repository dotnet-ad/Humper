namespace Humper.Responses
{
	using Base;

	public class CrossResponse : ICollisionResponse
	{
		public CrossResponse(ICollision collision)
		{
			this.Destination = collision.Goal;
		}

		public RectangleF Destination { get; private set; }
	}
}

