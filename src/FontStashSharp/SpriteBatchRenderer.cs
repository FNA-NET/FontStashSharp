using FontStashSharp.Interfaces;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FontStashSharp
{
	internal class SpriteBatchRenderer : IFontStashRenderer
	{
		public static readonly SpriteBatchRenderer Instance = new SpriteBatchRenderer();

		private SpriteBatch _batch;

		public GraphicsDevice GraphicsDevice => _batch.GraphicsDevice;

		public SpriteBatch Batch
		{
			get
			{
				return _batch;
			}

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				if (value == _batch)
				{
					return;
				}

				_batch = value;
			}
		}

		private SpriteBatchRenderer()
		{
		}

		public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 scale, float depth)
		{
			_batch.Draw(texture,
				position,
				sourceRectangle,
				color,
				rotation,
				Vector2.Zero,
				scale,
				SpriteEffects.None,
				depth);
		}
	}
}
