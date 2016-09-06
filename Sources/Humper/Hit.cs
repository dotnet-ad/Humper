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

		public static IHit Resolve(Vector2 origin, Vector2 destination, IBox other)
		{
			var result = Resolve(origin, destination, other.Bounds);
			if (result != null) result.Box = other;
			return result;
		}

		public static Hit Resolve(RectangleF origin, RectangleF destination, RectangleF other)
		{
			var broadphaseArea = RectangleF.Union(origin,destination);

			if (broadphaseArea.Intersects(other) || broadphaseArea.Contains(other))
			{
				return ResolveNarrow(origin, destination, other);
			}

			return null;
		}

		public static Hit Resolve(Vector2 origin, Vector2 destination, RectangleF other)
		{
			var min = Vector2.Min(origin,destination);
			var size = Vector2.Max(origin, destination) - min;

			var broadphaseArea = new RectangleF(min, size);

			if (broadphaseArea.Intersects(other) || broadphaseArea.Contains(other))
			{
				return ResolveNarrow(origin, destination, other);
			}

			return null;
		}

		public static IHit Resolve(Vector2 point, IBox other)
		{
			if (other.Bounds.Contains(point))
			{
				var outside = PushOutside(point, other.Bounds);
				return new Hit()
				{
					Amount = 0,
					Box = other,
					Position = outside.Item1,
					Normal = outside.Item2,
				};
			}

			return null;
		}

		#endregion

		private static Tuple<Vector2, Vector2> PushOutside(Vector2 origin, RectangleF other)
		{
			var position = origin;
			var normal = Vector2.Zero;

			var top = origin.Y - other.Top;
			var bottom = other.Bottom - origin.Y;
			var left = origin.X - other.Left;
			var right = other.Right - origin.X;

			var min = Math.Min(top, Math.Min(bottom, Math.Min(right, left)));

			if (Math.Abs(min - top) < Constants.Threshold)
			{
				normal = -Vector2.UnitY;
				position = new Vector2(position.X, other.Top);
			}
			else if (Math.Abs(min - bottom) < Constants.Threshold)
			{
				normal = Vector2.UnitY;
				position = new Vector2(position.X, other.Bottom);
			}
			else if (Math.Abs(min - left) < Constants.Threshold)
			{
				normal = -Vector2.UnitX;
				position = new Vector2(other.Left, position.Y);
			}
			else if (Math.Abs(min - right) < Constants.Threshold)
			{
				normal = Vector2.UnitX;
				position = new Vector2(other.Right, position.Y);
			}

			return new Tuple<Vector2, Vector2>(position,normal);
		}

		private static Tuple<RectangleF,Vector2> PushOutside(RectangleF origin, RectangleF other)
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

			return new Tuple<RectangleF, Vector2>(position,normal);
		}

		private static Hit ResolveNarrow(RectangleF origin, RectangleF destination, RectangleF other)
		{
			// if starts inside, push it outside at the neareast place
			if (other.Contains(origin) || other.Intersects(origin))
			{
				var outside = PushOutside(origin, other);
				return new Hit()
				{
					Amount = 0,
					Position = outside.Item1.Location,
					Normal = outside.Item2,
				};
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
				Normal = GetNormal(invEntry, invExit, entry),
			};


			return result;
		}

		private static Hit ResolveNarrow(Vector2 origin, Vector2 destination, RectangleF other)
		{
			// if starts inside, push it outside at the neareast place
			if (other.Contains(origin))
			{
				var outside = PushOutside(origin, other);
				return new Hit()
				{
					Amount = 0,
					Position = outside.Item1,
					Normal = outside.Item2,
				};
			}

			var velocity = (destination - origin);

			Vector2 invEntry, invExit, entry, exit;

			if (velocity.X > 0)
			{
				invEntry.X = other.Left - origin.X;
				invExit.X = other.Right - origin.X;
			}
			else
			{
				invEntry.X = other.Right - origin.X;
				invExit.X = other.Left - origin.X;
			}

			if (velocity.Y > 0)
			{
				invEntry.Y = other.Top - origin.Y;
				invExit.Y = other.Bottom - origin.Y;
			}
			else
			{
				invEntry.Y = other.Bottom - origin.Y;
				invExit.Y = other.Top - origin.Y;
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
				(entry.X < 0.0f && (origin.X < other.Left || origin.X > other.Right)) ||
				entry.Y < 0.0f && (origin.Y < other.Top || origin.Y > other.Bottom))
				return null;
			
			var result = new Hit()
			{
				Amount = entryTime,
				Position = origin + velocity * entryTime,
				Normal = GetNormal(invEntry, invExit, entry),
			};

			return result;
		}

		private static Vector2 GetNormal(Vector2 invEntry, Vector2 invExit, Vector2 entry)
		{
			if (entry.X > entry.Y)
			{
				return (invEntry.X < 0.0f) || (Math.Abs(invEntry.X) < Constants.Threshold && invExit.X < 0) ? Vector2.UnitX : -Vector2.UnitX;
			}

			return (invEntry.Y < 0.0f || (Math.Abs(invEntry.Y) < Constants.Threshold && invExit.Y < 0)) ? Vector2.UnitY : -Vector2.UnitY;
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

