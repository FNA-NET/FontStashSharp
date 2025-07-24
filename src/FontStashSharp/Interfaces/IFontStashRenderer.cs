using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FontStashSharp.Interfaces
{
	public interface IFontStashRenderer
	{
		GraphicsDevice GraphicsDevice { get; }

		void Draw(Texture2D texture, Vector2 pos, Rectangle? src, Color color, float rotation, Vector2 scale, float depth);
	}
}
