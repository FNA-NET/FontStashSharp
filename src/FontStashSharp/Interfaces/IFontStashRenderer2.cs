using Microsoft.Xna.Framework.Graphics;

namespace FontStashSharp.Interfaces
{
	public interface IFontStashRenderer2
	{
		GraphicsDevice GraphicsDevice { get; }

		void DrawQuad(Texture2D texture, ref VertexPositionColorTexture topLeft, ref VertexPositionColorTexture topRight, ref VertexPositionColorTexture bottomLeft, ref VertexPositionColorTexture bottomRight);
	}
}
