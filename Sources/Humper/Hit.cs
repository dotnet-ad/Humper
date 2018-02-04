namespace Humper
{
	using System;
	using Base;

	public class Hit : IHit
	{
		public Hit()
		{
			this.Normal = Vector2.Zero;
			this.Amount = 1.0f;
		}

		public IBox Box { get; set; }

		public Vector2 Normal { get; set; }

		public float Amount { get; set; }

		public Vector2 Position { get; set; }

		public float Remaining { get { return 1.0f - this.Amount; } }

		#region Public functions

		public static IHit Resolve(RectangleF origin, RectangleF destination, IBox other)
		{
			var result = Resolve(origin,destination, other.Bounds);
			if (result != null) result.Box = other;
			return result;
		}

		private static Hit Resolve(RectangleF origin, RectangleF destination, RectangleF other)
		{
			var broadphaseArea = new RectangleF(origin,destination);

			if (broadphaseArea.Intersects(other) || broadphaseArea.Contains(other))
			{
				return ResolveNarrow(origin, destination, other);
			}

			return null;
		}

		public static IHit Resolve(Vector2 origin, Vector2 destination, IBox other)
		{
			var result = Resolve(origin, destination, other.Bounds);
			if (result != null) result.Box = other;
			return result;
		}

		private static Hit Resolve(Vector2 origin, Vector2 destination, RectangleF other)
		{
			// Liang-Barsky algorithm : https://gist.github.com/ChickenProp/3194723
			var velocity = destination - origin;
			var p = new[] { -velocity.X, velocity.X, -velocity.Y, velocity.Y };
			var q = new[] { origin.X - other.Left, other.Right - origin.X, origin.Y - other.Top, other.Bottom - origin.Y };
			var u1 = float.MinValue;
			var u2 = float.MaxValue;

			for (int i = 0; i < 4; i++)
			{
				if (Math.Abs(p[i]) < Constants.Threshold)
				{
					if (q[i] < 0)
					{
						return null;
					}
				}
				else
				{
					var t = q[i] / p[i];
					if (p[i] < 0 && u1 < t)
					{
						u1 = t;
					}
					else if (p[i] > 0 && u2 > t)
					{
						u2 = t;
					}
				}
			}

			if (u1 > u2 || u1 > 1 || u1 < 0)
			{
				return null;
			}

			// TODO add normal

			return new Hit()
			{
				Amount = u1,
				Position = new Vector2(origin.X + u1 * velocity.X, origin.Y + u1 * velocity.Y),
			};
		}

		public static IHit Resolve(Vector2 point, IBox other)
		{
			// TODO add normal and nearest point on rectangle sides
			if (other.Bounds.Contains(point))
			{
				return new Hit()
				{
					Amount = 0,
					Box = other,
					Position = point,
				};
			}

			return null;
		}

		#endregion

		private static RectangleF PushOutside(RectangleF origin, RectangleF other)
		{
			var position = origin;
			var normal = Vector2.Zero;

			var top = origin.Center.Y - other.Top;
			var bottom = other.Bottom - origin.Center.Y;
			var left = origin.Center.X - other.Left;
			var right = other.Right - origin.Center.X;

			var min = Math.Min(top, Math.Min(bottom, Math.Min(right, left)));

			if (Math.Abs(min - top) < Constants.Threshold)
			{
				normal = -Vector2.UnitY;
				position.Location = new Vector2(position.Location.X, other.Top - position.Height);
			}
			else if (Math.Abs(min - bottom) < Constants.Threshold)
			{
				normal = Vector2.UnitY;
				position.Location = new Vector2(position.Location.X, other.Bottom);
			}
			else if (Math.Abs(min - left) < Constants.Threshold)
			{
				normal = -Vector2.UnitX;
				position.Location = new Vector2(other.Left - position.Width, position.Location.Y);
			}
			else if (Math.Abs(min - right) < Constants.Threshold)
			{
				normal = Vector2.UnitX;
				position.Location = new Vector2(other.Right, position.Location.Y);
			}

			return position;
		}

		private static Hit ResolveNarrow(RectangleF origin, RectangleF destination, RectangleF other)
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

			if (Math.Abs(velocity.X) < Constants.Threshold)
			{
				entry.X = float.MinValue;
				exit.X = float.MaxValue;
			}
			else
			{
				entry.X = invEntry.X / velocity.X;
				exit.X = invExit.X / velocity.X;
			}

			if (Math.Abs(velocity.Y) < Constants.Threshold)
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
				Position = origin.Location + velocity * entryTime,
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

		public bool IsNearest(IHit than, Vector2 origin)
		{
			if (this.Amount < than.Amount)
			{
				return true;
			}
			else if (this.Amount > than.Amount)
			{
				return true;
			}

			var thisDistance = (origin - this.Position).LengthSquared();
			var otherDistance = (origin - than.Position).LengthSquared();

			return thisDistance < otherDistance;
		}
	}
}

