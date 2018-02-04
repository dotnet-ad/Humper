using System;
using Humper.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Humper.Sample.Basic
{
	public class PreviewScene : IScene
	{
		public PreviewScene()
		{
		}

		private RectangleF origin;

		private RectangleF destination;

		private RectangleF collision;

		private RectangleF other;

		private RectangleF normal;

		private bool moveDestination;

		private KeyboardState previous;

		public void Draw(SpriteBatch sb)
		{
			sb.Draw(origin, color: Color.Green, fillOpacity: 0.3f);
			sb.Draw(destination, color: Color.Red, fillOpacity: 0.3f);
			sb.Draw(other, color: Color.Fuchsia, fillOpacity: 0.3f);
			sb.Draw(collision, color: Color.Orange, fillOpacity: 0.0f);
			sb.Draw(normal, color: Color.Orange, fillOpacity: 0.3f);
		}

		public void Initialize()
		{
			this.origin = new RectangleF(0, 0, 100, 100);
			this.destination = new RectangleF(400, 300, 100, 100);

			this.other = new RectangleF(200, 200, 500, 120);
		}


		public void Update(GameTime time)
		{
			var state = Keyboard.GetState();
			if (previous.IsKeyUp(Keys.N) && state.IsKeyDown(Keys.N))
			{
				moveDestination = !moveDestination;
			}
			previous = state;

			var m = Mouse.GetState().Position;
			var pos =new Base.Vector2(m.X, m.Y);

			if (moveDestination)
			{
				this.destination.Location = pos;
			}
			else
			{
				this.origin.Location = pos;
			}

			// Calculate collision
			var hit = Hit.Resolve(origin, destination, other);

			if (hit != null)
			{
				this.collision = new RectangleF(hit.Position, origin.Size);
				this.normal = new RectangleF(this.collision.Center + hit.Normal * 50, new Base.Vector2(5, 5));
			}
			else
			{
				this.collision = new RectangleF();
				this.normal = new RectangleF();
			}
		}

		public void LoadContent(ContentManager content)
		{

		}
	}
}

