using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Humper.Sample.Basic
{
	public abstract class WorldScene : IScene
	{
		public WorldScene()
		{
		}

		protected World World { get; set; }
		 
		public virtual string Message
		{
			get
			{
				return $"[Up,Right,Down,Left]: move\n[Space]: show grid";
			} 
		}

		private SpriteBatch spriteBatch;

		private SpriteFont font;

		public virtual void Draw(SpriteBatch sb)
		{
			var b = this.World.Bounds;
			this.spriteBatch = sb;
			this.World.DrawDebug((int)b.X, (int)b.Y, (int)b.Width, (int)b.Height, DrawCell, DrawBox, DrawString);
		}

		private void DrawCell(int x, int y, int w, int h, float alpha)
		{
			if (Keyboard.GetState().IsKeyDown(Keys.Space))
				spriteBatch.DrawStroke(new Rectangle(x, y, w, h), new Color(Color.White, alpha));
		}

		private void DrawBox(IBox box)
		{
			Color color;

			if (box.HasTag(Tags.Group1))
				color = Color.White;
			else if (box.HasTag(Tags.Group3))
				color = Color.Red;
			else if (box.HasTag(Tags.Group4))
				color = Color.Green;
			else if (box.HasTag(Tags.Group5))
				color = Color.Yellow;
			else
				color = new Color(165, 155, 250);

			spriteBatch.Draw(box.Bounds, color, 0.3f);
		}

		public void LoadContent(ContentManager content)
		{
			this.font = content.Load<SpriteFont>("font");
		}

		private void DrawString(string message, int x, int y, float alpha)
		{
			var size = this.font.MeasureString(message);
			if (Keyboard.GetState().IsKeyDown(Keys.Space))
				spriteBatch.DrawString(this.font, message, new Vector2(x - size.X / 2, y - size.Y / 2), new Color(Color.White, alpha));
		}

		public abstract void Initialize();


		public abstract void Update(GameTime time);
	}
}

