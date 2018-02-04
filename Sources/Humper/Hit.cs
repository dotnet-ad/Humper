namespace Humper
{
	using System;
	using Base;

	public class Hit
	{
		public Hit()
		{
			this.Normal = Vector2.Zero;
			this.Amount = 1.0f;
		}

		public Vector2 Normal { get; set; }

		public float Amount { get; set; }

		public RectangleF Position { get; set; }

		public float Remaining { get { return 1.0f - this.Amount; } }

		public static Hit ResolveWithBroadphasing(RectangleF origin, RectangleF destination, RectangleF other)
		{
			var broadphaseArea = new RectangleF(origin,destination);

			if (broadphaseArea.Intersects(other) || broadphaseArea.Contains(other))
			{
				return Resolve(origin, destination, other);
			}

			return null;
		}

		private static RectangleF PushOutside(RectangleF origin, RectangleF other)
		{
			var position = origin;
			var normal = Vector2.Zero;

			var top = origin.Center.Y - other.Top;
			var bottom = other.Bottom - origin.Center.Y;
			var left = origin.Center.X - other.Left;
			var right = other.Right - origin.Center.X;

			var min = Math.Min(top, Math.Min(bottom, Math.Min(right, left)));

			if (min == top)
			{
				normal = -Vector2.UnitY;
				position.Location = new Vector2(position.Location.X, other.Top - position.Height);
			}
			else if (min == bottom)
			{
				normal = Vector2.UnitY;
				position.Location = new Vector2(position.Location.X, other.Bottom);
			}
			else if (min == left)
			{
				normal = -Vector2.UnitX;
				position.Location = new Vector2(other.Left - position.Width, position.Location.Y);
			}
			else if (min == right)
			{
				normal = Vector2.UnitX;
				position.Location = new Vector2(other.Right, position.Location.Y);
			}

			return position;
		}

		private static Hit Resolve(RectangleF origin, RectangleF destination, RectangleF other)
		{
			// if starts inside, push it outside at the neareast place
			if (other.Contains(origin) || other.Intersects(origin))
			{
				origin = PushOutside(origin, other);
			}

			var velocity = (destination.Location - origin.Location);
			Vector2 invEntry, invExit, entry, exit;

			if (velocity.X > 0)
			{
				invEntry.X = other.Left - origin.Right;
				invExit.X = other.Right - origin.Left;
			}
			else
			{
				invEntry.X = other.Right - origin.Left;
				invExit.X = other.Left - origin.Right;
			}

			if (velocity.Y > 0)
			{
				invEntry.Y = other.Top - origin.Bottom;
				invExit.Y = other.Bottom - origin.Top;
			}
			else
			{
				invEntry.Y = other.Bottom - origin.Top;
				invExit.Y = other.Top - origin.Bottom;
			}

			if (Math.Abs(velocity.X) < Constants.Epsilon)
			{
				entry.X = float.MinValue;
				exit.X = float.MaxValue;
			}
			else
			{
				entry.X = invEntry.X / velocity.X;
				exit.X = invExit.X / velocity.X;
			}

			if (Math.Abs(velocity.Y) < Constants.Epsilon)
			{
				entry.Y = float.MinValue;
				exit.Y = float.MaxValue;
			}
			else
			{
				entry.Y = invEntry.Y / velocity.Y;
				exit.Y = invExit.Y / velocity.Y;
			}

			if (entry.Y > 1.0f) entry.Y = float.MinValue;
			if (entry.X > 1.0f) entry.X = float.MinValue;

			var entryTime = Math.Max(entry.X, entry.Y);
			var exitTime = Math.Min(exit.X, exit.Y);

			if (
				(entryTime > exitTime || entry.X < 0.0f && entry.Y < 0.0f) ||
				(entry.X < 0.0f && (origin.Right < other.Left || origin.Left > other.Right)) ||
				entry.Y < 0.0f && (origin.Bottom < other.Top || origin.Top > other.Bottom))
				return null; 


			var result = new Hit()
			{
				Amount = entryTime,
				Position = new RectangleF(origin.Location + velocity * entryTime, origin.Size),
			};

			// Calculate normal of collided surface
			if (entry.X > entry.Y)
			{
				if (invEntry.X < 0.0f)
				{
					result.Normal = Vector2.UnitX;
				}
				else
				{
					result.Normal = -Vector2.UnitX;
				}
			}
			else
			{
				if (invEntry.Y < 0.0f)
				{
					result.Normal = Vector2.UnitY;
				}
				else
				{
					result.Normal = -Vector2.UnitY;
				}
			}

			return result;
		}

	}
}

