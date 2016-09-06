namespace Humper.Sample.Basic
{
	using System;
	using Responses;
	using Microsoft.Xna.Framework;
	using Microsoft.Xna.Framework.Input;

	public class TopdownScene : WorldScene
	{
		public TopdownScene()
		{
		}

		private IBox player1, player2;

		public override void Initialize()
		{
			this.World = new World(1024, 700);

			this.player1 = this.World.Create(50, 50, 24, 24).AddTags(Tags.Group1);
			this.player2 = this.World.Create(100, 50, 24, 24).AddTags(Tags.Group1);

			// Map
			this.World.Create(100, 100, 150, 20).AddTags(Tags.Group2);
			this.World.Create(180, 140, 200, 200).AddTags(Tags.Group2);
			this.World.Create(190, 20, 80, 400).AddTags(Tags.Group2);
		}

		public override void Update(GameTime time)
		{
			var delta = (float)time.ElapsedGameTime.TotalMilliseconds;

			UpdatePlayer(this.player1, delta, Keys.Left, Keys.Up, Keys.Right, Keys.Down);
			UpdatePlayer(this.player2, delta, Keys.F, Keys.T, Keys.H, Keys.G);
		}

		private void UpdatePlayer(IBox player, float delta, Keys left, Keys up, Keys right, Keys down)
		{
			var velocity = Vector2.Zero;

			var k = Keyboard.GetState();
			if (k.IsKeyDown(right)) 
				velocity.X += 0.1f;
			if (k.IsKeyDown(left)) 
				velocity.X -= 0.1f;
			if (k.IsKeyDown(down)) 
				velocity.Y += 0.1f;
			if (k.IsKeyDown(up)) 
				velocity.Y -= 0.1f;
			
			var move = player.Move(player.X + delta * velocity.X, player.Y + delta * velocity.Y, (collision) => CollisionResponses.Slide);

		}

	}
}

