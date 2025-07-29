using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FontStashSharp
{
	public class FontSystemSettings
	{
		private int _textureWidth = 1024, _textureHeight = 1024;
		private float _fontResolutionFactor = 1.0f;

		public int TextureWidth
		{
			get => _textureWidth;

			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value));

				}

				_textureWidth = value;
			}
		}

		public int TextureHeight
		{
			get => _textureHeight;

			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value));

				}

				_textureHeight = value;
			}
		}

		public float FontResolutionFactor
		{
			get => _fontResolutionFactor;
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value), value, "This cannot be smaller than 0");
				}

				_fontResolutionFactor = value;
			}
		}

		/// <summary>
		/// Use existing texture for storing glyphs
		/// If this is set, then TextureWidth & TextureHeight are ignored
		/// </summary>
		public Texture2D ExistingTexture { get; set; }

		/// <summary>
		/// Defines rectangle of the used space in the ExistingTexture
		/// </summary>
		public Rectangle ExistingTextureUsedSpace { get; set; }

		public FontSystemSettings()
		{
			TextureWidth = FontSystemDefaults.TextureWidth;
			TextureHeight = FontSystemDefaults.TextureHeight;
			FontResolutionFactor = FontSystemDefaults.FontResolutionFactor;
		}

		public FontSystemSettings Clone()
		{
			return new FontSystemSettings
			{
				TextureWidth = TextureWidth,
				TextureHeight = TextureHeight,
				FontResolutionFactor = FontResolutionFactor,
				ExistingTexture = ExistingTexture,
				ExistingTextureUsedSpace = ExistingTextureUsedSpace,
			};
		}
	}
}