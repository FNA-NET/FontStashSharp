using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FontStashSharp
{
	internal static class Texture2DManager
	{
		public static Texture2D CreateTexture(GraphicsDevice device, int width, int height)
		{
			var texture2d = new Texture2D(device, width, height);

			return texture2d;
		}

		public static void SetTextureData(Texture2D texture, Rectangle bounds, byte[] data)
		{
			texture.SetData(0, bounds, data, 0, bounds.Width * bounds.Height * 4);
		}
	}
}