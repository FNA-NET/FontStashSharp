using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using FontStashSharp.Rasterizers.FreeType;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FreeTypeSharp;
using static FreeTypeSharp.FT_Render_Mode_;

namespace FontStashSharp
{
	public partial class FontSystem : IDisposable
	{
		public const int GlyphPad = 2;

		private readonly List<FreeTypeSource> _fontSources = new List<FreeTypeSource>();
		private readonly Int32Map<DynamicSpriteFont> _fonts = new Int32Map<DynamicSpriteFont>();
		private readonly FontSystemSettings _settings;

		private FontAtlas _currentAtlas;

		public int TextureWidth => _settings.TextureWidth;
		public int TextureHeight => _settings.TextureHeight;

		public float FontResolutionFactor => _settings.FontResolutionFactor;

		public Texture2D ExistingTexture => _settings.ExistingTexture;
		public Rectangle ExistingTextureUsedSpace => _settings.ExistingTextureUsedSpace;

		public bool UseKernings { get; set; } = true;
		public int? DefaultCharacter { get; set; } = ' ';

		internal List<FreeTypeSource> FontSources => _fontSources;

		public List<FontAtlas> Atlases { get; } = new List<FontAtlas>();
		public FontAtlas CurrentAtlas => _currentAtlas;

		public event EventHandler CurrentAtlasFull;

		public FontSystem(FontSystemSettings settings)
		{
			if (settings == null)
			{
				throw new ArgumentNullException(nameof(settings));
			}

			_settings = settings.Clone();

			UseKernings = FontSystemDefaults.UseKernings;
			DefaultCharacter = FontSystemDefaults.DefaultCharacter;
		}

		public FontSystem() : this(new FontSystemSettings())
		{
		}

		public void Dispose()
		{
			if (_fontSources != null)
			{
				foreach (var font in _fontSources)
					font.Dispose();
				_fontSources.Clear();
			}

			if (Atlases != null)
			{
				foreach (var atlas in Atlases)
					if (atlas.Texture is IDisposable dispTexture)
						dispTexture.Dispose();

				Atlases.Clear();
			}

			SetFontAtlas(null);
			_fonts.Clear();
		}

		public void AddFont(byte[] data, FT_Render_Mode_ renderMode = FT_RENDER_MODE_NORMAL)
		{
			var fontSource = FreeTypeLoader.Load(data, renderMode);
			_fontSources.Add(fontSource);
		}

		public void AddFont(Stream stream, FT_Render_Mode_ renderMode = FT_RENDER_MODE_NORMAL)
		{
			AddFont(stream.ToByteArray(), renderMode);
		}

		public DynamicSpriteFont GetFont(float fontSize)
		{
			var intSize = fontSize.FloatAsInt();
			DynamicSpriteFont result;
			if (_fonts.TryGetValue(intSize, out result))
			{
				return result;
			}

			if (_fontSources.Count == 0)
			{
				throw new Exception("Could not create a font without a single font source. Use AddFont to add at least one font source.");
			}

			var fontSource = _fontSources[0];

			int ascent, descent, lineHeight;
			fontSource.GetMetricsForSize(fontSize, out ascent, out descent, out lineHeight);

			result = new DynamicSpriteFont(this, fontSize, lineHeight);
			_fonts[intSize] = result;
			return result;
		}

		public void SetFontAtlas(FontAtlas fontAtlas)
		{
			if (fontAtlas != null && !Atlases.Contains(fontAtlas))
			{
				Atlases.Add(fontAtlas);
			}
			_currentAtlas = fontAtlas;
		}

		public void Reset()
		{
			Atlases.Clear();
			_fonts.Clear();
			SetFontAtlas(null);
		}

		internal int? GetCodepointIndex(int codepoint, out int fontSourceIndex)
		{
			fontSourceIndex = 0;
			var g = default(int?);

			for (var i = 0; i < _fontSources.Count; ++i)
			{
				var f = _fontSources[i];
				g = f.GetGlyphId(codepoint);
				if (g != null)
				{
					fontSourceIndex = i;
					break;
				}
			}

			return g;
		}

		private FontAtlas CreateFontAtlas(GraphicsDevice device, int textureWidth, int textureHeight)
		{
			Texture2D existingTexture = null;
			if (ExistingTexture != null && Atlases.Count == 0)
			{
				existingTexture = ExistingTexture;
			}

			FontAtlas fontAtlas = new FontAtlas(textureWidth, textureHeight, 256, existingTexture);

			// If existing texture is used, mark existing used rect as used
			if (existingTexture != null && !ExistingTextureUsedSpace.IsEmpty)
			{
				if (!fontAtlas.AddSkylineLevel(0, ExistingTextureUsedSpace.X, ExistingTextureUsedSpace.Y, ExistingTextureUsedSpace.Width, ExistingTextureUsedSpace.Height))
				{
					throw new Exception(string.Format("Unable to specify existing texture used space: {0}", ExistingTextureUsedSpace));
				}

				// TODO: Clear remaining space
			}

			return fontAtlas;
		}

		internal void RenderGlyphOnAtlas(GraphicsDevice device, DynamicFontGlyph glyph)
		{
			var textureSize = new Point(TextureWidth, TextureHeight);

			if (ExistingTexture != null)
			{
				textureSize = new Point(ExistingTexture.Width, ExistingTexture.Height);
			}

			int gx = 0, gy = 0;
			var gw = glyph.Size.X + GlyphPad * 2;
			var gh = glyph.Size.Y + GlyphPad * 2;

			// If CurrentAtlas is null create a new one
			if (CurrentAtlas == null)
			{
				SetFontAtlas(CreateFontAtlas(device, textureSize.X, textureSize.Y));
			}
			var atlas = CurrentAtlas;
			if (!atlas.AddRect(gw, gh, ref gx, ref gy))
			{
				CurrentAtlasFull?.Invoke(this, EventArgs.Empty);

				// Create a new atlas if it was not set during the CurrentAtlasFull event
				if (CurrentAtlas == atlas)
				{
					SetFontAtlas(CreateFontAtlas(device, textureSize.X, textureSize.Y));
				}
				atlas = CurrentAtlas;

				// Try to add again
				if (!atlas.AddRect(gw, gh, ref gx, ref gy))
				{
					throw new Exception(string.Format("Could not add rect to the newly created atlas. gw={0}, gh={1}", gw, gh));
				}
			}

			glyph.TextureOffset.X = gx + GlyphPad;
			glyph.TextureOffset.Y = gy + GlyphPad;

			atlas.RenderGlyph(device, glyph, FontSources[glyph.FontSourceIndex]);

			glyph.Texture = atlas.Texture;
		}
	}
}