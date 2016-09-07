namespace Humper.Sample.Basic
{
	using Microsoft.Xna.Framework;
	using Microsoft.Xna.Framework.Content;
	using Microsoft.Xna.Framework.Graphics;

	public interface IScene
	{
		void Initialize();

		void LoadContent(ContentManager content);

		void Update(GameTime time);

		void Draw(SpriteBatch sb);

		string Message { get; }
	}
}

