namespace Humper.Sample.Basic
{
	using System;
	using Responses;
	using Microsoft.Xna.Framework;
	using Microsoft.Xna.Framework.Input;
	using System.Linq;

	public class PlatformerScene : IScene
	{
		public PlatformerScene()
		{
		}

		public World World { get; private set; }

		private IBox player1, platform;

		private Vector2 platformVelocity = Vector2.UnitX * 0.05f;

		public void Initialize()
		{
			this.World = new World(1024, 700);

			this.SpawnPlayer();

			this.platform = this.World.Create(0, 200, 100, 20).AddTags(Tags.Group4);

			// Map
			this.World.Create(0, 300, 400, 20).AddTags(Tags.Group2);
			this.World.Create(380, 320, 20, 80).AddTags(Tags.Group2);
			this.World.Create(380, 400, 300, 20).AddTags(Tags.Group2);
			this.World.Create(420, 200, 200, 20).AddTags(Tags.Group2);
			this.World.Create(680, 220, 20, 200).AddTags(Tags.Group2);
			this.World.Create(680, 200, 200, 20).AddTags(Tags.Group2);

			this.World.Create(400, 300, 280, 100).AddTags(Tags.Group3);
		}

		private void SpawnPlayer()
		{
			if(this.player1 != null)
				this.World.Remove(this.player1);

			this.player1 = this.World.Create(50, 50, 10, 24).AddTags(Tags.Group1);
			this.velocity = Vector2.Zero;
		}

		public void Update(GameTime time)
		{
			var delta = (float)time.ElapsedGameTime.TotalMilliseconds;

			UpdatePlatform(this.platform, delta);
			UpdatePlayer(this.player1, delta, Keys.Left, Keys.Up, Keys.Right, Keys.Down);
		}

		private Vector2 velocity = Vector2.Zero;
		private KeyboardState state;
		private float timeInRed;
		private bool onPlatform;

		private void UpdatePlatform(IBox platform, float delta)
		{
			if ((platform.X < 50 && platformVelocity.X < 0) || (platform.X > 300 && platformVelocity.X > 0))
			{
				this.platformVelocity.X *= -1;
			}

			platform.Move(platform.X + this.platformVelocity.X * delta, platform.Y, (collistion) => CollisionResponses.None);
		}

		private void UpdatePlayer(IBox player, float delta, Keys left, Keys up, Keys right, Keys down)
		{
			velocity.Y += delta * 0.001f;
			velocity.X = 0;

			var k = Keyboard.GetState();
			if (k.IsKeyDown(right))
				velocity.X += 0.1f;
			if (k.IsKeyDown(left))
				velocity.X -= 0.1f;
			if (state.IsKeyUp(up) && k.IsKeyDown(up))
				velocity.Y -= 0.5f;

			if (onPlatform)
				velocity += platformVelocity;

			if (timeInRed > 0)
				velocity.Y *= 0.75f;

			// Moving player
			var move = player.Move(player.X + delta * velocity.X, player.Y + delta * velocity.Y, (collision) =>
			{
				if (collision.Other.HasTag(Tags.Group3))
				{
					return CollisionResponses.Cross;
				}

				return CollisionResponses.Slide;
			});

			// Testing if on moving platform
			onPlatform = move.Collisions.Any((c) => c.Other.HasTag(Tags.Group4));

			// Testing if on ground
			if (move.Collisions.Any((c) => c.Other.HasTag(Tags.Group4, Tags.Group2) && (c.Hit.Normal.Y < 0)))
			{
				velocity.Y = 0;
			}

			// Testing if in red water
			if (move.Collisions.Any((c) => c.Other.HasTag(Tags.Group3)))
			{
				timeInRed += delta;
				if (timeInRed > 3000)
					SpawnPlayer();
			}
			else
			{
				timeInRed = 0;
			}

			state = k;
		}
	}
}

