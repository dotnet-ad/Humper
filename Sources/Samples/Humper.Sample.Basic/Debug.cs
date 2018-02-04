using System;
using Humper.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Humper.Sample.Basic
{
	public static class Debug
	{
		private static Texture2D pixel;

		public static Rectangle ToRectangle(this RectangleF r)
		{
			return new Rectangle((int)r.X,(int)r.Y,(int)r.Width,(int)r.Height);
		}

		public static void Draw(this SpriteBatch spriteBatch, RectangleF rect, Color color)
		{
			spriteBatch.Draw(rect.ToRectangle(), color);
		}

		public static void Draw(this SpriteBatch spriteBatch, RectangleF rect, Color color, float fillOpacity)
		{
			spriteBatch.Draw(rect.ToRectangle(), color, fillOpacity);
		}

		public static void Draw(this SpriteBatch spriteBatch, Rectangle rect, Color color)
		{
			if (pixel == null)
			{
				pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
				pixel.SetData(new Color[] { Color.White });
			}

			spriteBatch.Draw(pixel, destinationRectangle: rect, color: color);
		}

		public static void Draw(this SpriteBatch spriteBatch, Rectangle rect, Color stroke, float fillOpacity)
		{
			if (pixel == null)
			{
				pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
				pixel.SetData(new Color[] { Color.White });
			}

			var fill = new Color(stroke, fillOpacity);
			spriteBatch.DrawFill(rect, fill);
			spriteBatch.DrawStroke(rect, stroke);
		}

		public static void DrawFill(this SpriteBatch spriteBatch, Rectangle rect, Color fill)
		{
			if (pixel == null)
			{
				pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
				pixel.SetData(new Color[] { Color.White });
			}

			spriteBatch.Draw(pixel, destinationRectangle: rect, color: fill);
		}

		public static void DrawStroke(this SpriteBatch spriteBatch, Rectangle rect, Color stroke)
		{
			if (pixel == null)
			{
				pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
				pixel.SetData(new Color[] { Color.White });
			}

			var left = new Rectangle(rect.Left, rect.Top, 1, rect.Height);
			var right = new Rectangle(rect.Right - 1, rect.Top, 1, rect.Height);
			var top = new Rectangle(rect.Left, rect.Top, rect.Width, 1);
			var bottom = new Rectangle(rect.Left, rect.Bottom - 1, rect.Width, 1);

			spriteBatch.Draw(pixel, destinationRectangle: left, color: stroke);
			spriteBatch.Draw(pixel, destinationRectangle: right, color: stroke);
			spriteBatch.Draw(pixel, destinationRectangle: top, color: stroke);
			spriteBatch.Draw(pixel, destinationRectangle: bottom, color: stroke);
		}
	}
}

