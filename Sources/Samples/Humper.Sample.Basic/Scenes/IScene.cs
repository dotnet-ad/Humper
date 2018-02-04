namespace Humper.Sample.Basic
{
	using Microsoft.Xna.Framework;

	public interface IScene
	{
		void Initialize();

		World World { get; }

		void Update(GameTime time);
	}
}

