namespace Humper.Sample.Basic
{
	using System;
	using Responses;
	using Microsoft.Xna.Framework;
	using Microsoft.Xna.Framework.Input;
	using System.Linq;
	using System.Collections.Generic;

	public class ParticlesScene : WorldScene
	{
		public class Particle
		{
			public static Random random = new Random();

			public Particle(IBox box)
			{
				this.Box = box;
				this.Velocity = new Vector2((float)random.NextDouble() * 0.1f, 0);
			}

			public IBox Box { get; set; }

			public Vector2 Velocity { get; set; }

			public void Update(float delta)
			{
				Velocity = Velocity + Vector2.UnitY * delta * 0.001f;

				var move = this.Box.Move(Box.X + delta * Velocity.X, Box.Y + delta * Velocity.Y, (collision) =>
				{
					return CollisionResponses.Bounce;
				});

				// Testing if on ground
				if (move.Hits.Any((c) => (c.Normal.Y < 0)))
				{
					Velocity = Velocity * new Vector2(1,-1);
				}

				// Testing if on wall
				if (move.Hits.Any((c) => (c.Normal.X != 0)))
				{
					Velocity = Velocity * new Vector2(-1, 1);
				}
			}
		}

		public ParticlesScene()
		{
		}

		private IBox player1;

		private Vector2 platformVelocity = Vector2.UnitX * 0.05f;

		public override void Initialize()
		{
			this.World = new World(1024, 700);

			this.SpawnPlayer();

			// Map
			this.World.Create(0, 0, 1024, 20).AddTags(Tags.Group2);
			this.World.Create(0, 20, 20, 660).AddTags(Tags.Group2);
			this.World.Create(1004, 20, 20, 660).AddTags(Tags.Group2);
			this.World.Create(0, 680, 1024, 20).AddTags(Tags.Group2);

			for (int x = 24; x < 1000; x+=40)
			{
				var box = this.World.Create(x, 40, 10, 10).AddTags(Tags.Group3);
				this.particles.Add(new Particle(box));
				box = this.World.Create(x, 80, 10, 10).AddTags(Tags.Group3);
				this.particles.Add(new Particle(box));
			}

		}

		private void SpawnPlayer()
		{
			if (this.player1 != null)
				this.World.Remove(this.player1);

			this.player1 = this.World.Create(50, 100, 50, 30).AddTags(Tags.Group1);
			this.velocity = Vector2.Zero;
		}

		public override void Update(GameTime time)
		{
			var delta = (float)time.ElapsedGameTime.TotalMilliseconds;

			foreach (var p in this.particles)
			{
				p.Update(delta);
			}
			UpdatePlayer(this.player1, delta, Keys.Left, Keys.Up, Keys.Right, Keys.Down);
		}

		private Vector2 velocity = Vector2.Zero;
		private KeyboardState state;
		private float timeInRed;
		private List<Particle> particles = new List<Particle>();

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

			// Moving player
			var move = player.Move(player.X + delta * velocity.X, player.Y + delta * velocity.Y, (collision) =>
			{
				return CollisionResponses.Slide;
			});

			// Testing if on ground
			if (move.Hits.Any((c) => c.Box.HasTag(Tags.Group2, Tags.Group3) && (c.Normal.Y < 0)))
			{
				velocity.Y = 0;
			}

			state = k;
		}
	}
}

