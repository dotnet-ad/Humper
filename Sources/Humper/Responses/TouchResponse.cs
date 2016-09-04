namespace Humper.Responses
{
	using Base;

	public class TouchResponse : ICollisionResponse
	{
		public TouchResponse(ICollision collision)
		{
			this.Destination = collision.Hit.Position;
		}

		public RectangleF Destination { get; private set; }
	}
}

