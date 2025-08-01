using System;

namespace FontStashSharp
{
	public static class FontSystemDefaults
	{
		private static int _textureWidth = 1024, _textureHeight = 1024;
		private static float _fontResolutionFactor = 1.0f;

		public static int TextureWidth
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

		public static int TextureHeight
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

		public static bool DisableAntialiasing { get; set; } = false;

		public static float FontResolutionFactor
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

		public static bool UseKernings { get; set; } = true;
		public static int? DefaultCharacter { get; set; } = '□';

		public static int TextStyleLineHeight { get; set; } = 2;
	}
}