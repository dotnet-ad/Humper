namespace Humper
{
	using System;
	using System.Collections.Generic;
	using Base;
	using Responses;

	public class World
	{
		public World(float width, float height, float cellSize = 64)
		{
			var iwidth = (int)Math.Ceiling(width / cellSize);
			var iheight = (int)Math.Ceiling(height / cellSize);

			this.grid = new Grid(iwidth, iheight, cellSize);
		}

		public RectangleF Bounds => new RectangleF(0, 0, this.grid.Width , this.grid.Height);

		#region Boxes

		private Grid grid;

		public IBox Create(float x, float y, float width, float height)
		{
			var box = new Box(this, x, y, width, height);
			this.grid.Add(box);
			return box;
		}

		public IEnumerable<IBox> Find(float x, float y, float w, float h)
		{
			return this.grid.QueryBoxes(x, y, w, h);
		}

		public IEnumerable<IBox> Find(RectangleF area)
		{
			return this.Find(area.X, area.Y, area.Width, area.Height);
		}

		public bool Remove(IBox box)
		{
			return this.grid.Remove(box);
		}

		public void Update(IBox box, RectangleF from)
		{
			this.grid.Update(box, from);
		}

		#endregion

		#region Physics

		public IMovement Simulate(Box box, float x, float y, Func<ICollision, ICollisionResponse> filter)
		{
			x = Math.Max(0, Math.Min(x, this.Bounds.Right - box.Width));
			y = Math.Max(0, Math.Min(y, this.Bounds.Bottom - box.Height));

			var origin = box.Bounds;
			var destination = new RectangleF(new Vector2(x, y), box.Bounds.Size);

			var result = new Movement()
			{
				Origin = origin,
				Destination = destination,
			};

			var wrap = new RectangleF(origin, destination);
			var boxes = this.grid.QueryBoxes(wrap.X, wrap.Y, wrap.Width, wrap.Height);

			Hit nearest = null;
			IBox nearestOther = null;

			foreach (var other in boxes)
			{
				if (other != box)
				{
					var hit = Hit.ResolveWithBroadphasing(origin, destination, other.Bounds);
					if (hit != null && (nearest == null || nearest.Amount > hit.Amount))
					{
						nearest = hit;
						nearestOther = other;
					}
				}
			}

			var collision = new Collision()
			{
				Box = box,
				Hit = nearest,
				Other = nearestOther,
				Origin = box.Bounds,
				Goal = new RectangleF(x, y, box.Width, box.Height),
			};

			result.Destination = destination;

			if (collision.HasCollided)
			{
				var collisions = new List<ICollision>() { collision };
				var response = filter(collision);
				if (response != null && result.Destination != response.Destination)
				{
					result.Destination = response.Destination;
					var nextMovement = this.Simulate(box, result.Destination.X, result.Destination.Y, filter);
					result.Destination = nextMovement.Destination;
					collisions.AddRange(nextMovement.Collisions);
				}
				result.Collisions = collisions;
			}

			return result;
		}

		#endregion

		#region Diagnostics

		public void DrawDebug(int x, int y, int w, int h, Action<int,int,int,int,float> drawCell, Action<IBox> drawBox, Action<string,int,int, float> drawString)
		{
			// Drawing boxes
			var boxes = this.grid.QueryBoxes(x, y, w, h);
			foreach (var box in boxes)
			{
				drawBox(box);
			}

			// Drawing cells
			var cells = this.grid.QueryCells(x, y, w, h);
			foreach (var cell in cells)
			{
				var count = cell.Count();
				var alpha = count > 0 ? 1f : 0.4f;
				drawCell((int)cell.Bounds.X, (int)cell.Bounds.Y, (int)cell.Bounds.Width, (int)cell.Bounds.Height, alpha);
				drawString(count.ToString(), (int)cell.Bounds.Center.X, (int)cell.Bounds.Center.Y,alpha);
			}
		}

		#endregion

	}
}

