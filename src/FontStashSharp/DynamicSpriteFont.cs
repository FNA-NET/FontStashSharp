using System;
using System.Collections.Generic;
using FreeTypeSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FontStashSharp
{
	public partial class DynamicSpriteFont : SpriteFontBase
	{
		private readonly Int32Map<DynamicFontGlyph> _storage = new Int32Map<DynamicFontGlyph>();
		private readonly Int32Map<int> Kernings = new Int32Map<int>();
		private FontMetrics[] IndexedMetrics;

		public FontSystem FontSystem { get; private set; }
		public FontStyle FontStyle { get; private set; }

		internal DynamicSpriteFont(FontSystem system, float size, int lineHeight, FontStyle fontStyle) : base(size, lineHeight)
		{
			if (system == null)
			{
				throw new ArgumentNullException(nameof(system));
			}

			FontSystem = system;
			FontStyle = fontStyle;
			RenderFontSizeMultiplicator = FontSystem.FontResolutionFactor;
		}

		internal Int32Map<DynamicFontGlyph> GetGlyphs()
		{
			return _storage;
		}

		private DynamicFontGlyph GetGlyphWithoutBitmap(int codepoint)
		{
			DynamicFontGlyph glyph;
			if (_storage.TryGetValue(codepoint, out glyph))
			{
				return glyph;
			}

			int fontSourceIndex;
			var g = FontSystem.GetCodepointIndex(codepoint, out fontSourceIndex);
			if (g == null)
			{
				_storage[codepoint] = null;
				return null;
			}

			var fontSize = FontSize * FontSystem.FontResolutionFactor;
			var font = FontSystem.FontSources[fontSourceIndex];

			int advance, x0, y0, x1, y1;
			font.GetGlyphMetrics(g.Value, fontSize, out advance, out x0, out y0, out x1, out y1);

			var gw = x1 - x0;
			var gh = y1 - y0;

			if (FontStyle.HasFlag(FontStyle.Bold) && !font.HasStyleFlag(FT_STYLE_FLAG.FT_STYLE_FLAG_BOLD))
			{
				gw += 1;
			}

			glyph = new DynamicFontGlyph
			{
				Codepoint = codepoint,
				Id = g.Value,
				FontSize = fontSize,
				FontSourceIndex = fontSourceIndex,
				RenderOffset = new Point(x0, y0),
				Size = new Point(gw, gh),
				XAdvance = advance,
			};

			_storage[codepoint] = glyph;

			return glyph;
		}

		private DynamicFontGlyph GetGlyphInternal(GraphicsDevice device, int codepoint)
		{
			var glyph = GetGlyphWithoutBitmap(codepoint);
			if (glyph == null)
			{
				return null;
			}

			if (device == null || glyph.Texture != null)
				return glyph;

			FontSystem.RenderGlyphOnAtlas(device, glyph, FontStyle);

			return glyph;
		}

		private DynamicFontGlyph GetDynamicGlyph(GraphicsDevice device, int codepoint)
		{
			var result = GetGlyphInternal(device, codepoint);
			if (result == null && FontSystem.DefaultCharacter != null)
			{
				result = GetGlyphInternal(device, FontSystem.DefaultCharacter.Value);
			}

			return result;
		}

		protected internal override FontGlyph GetGlyph(GraphicsDevice device, int codepoint)
		{
			return GetDynamicGlyph(device, codepoint);
		}

		private void GetMetrics(int fontSourceIndex, out FontMetrics result)
		{
			if (IndexedMetrics == null || IndexedMetrics.Length != FontSystem.FontSources.Count)
			{
				IndexedMetrics = new FontMetrics[FontSystem.FontSources.Count];
				for (var i = 0; i < IndexedMetrics.Length; ++i)
				{
					int ascent, descent, lineHeight;
					FontSystem.FontSources[i].GetMetricsForSize(FontSize * RenderFontSizeMultiplicator, out ascent, out descent, out lineHeight);

					IndexedMetrics[i] = new FontMetrics(ascent, descent, lineHeight);
				}
			}

			result = IndexedMetrics[fontSourceIndex];
		}

		internal override void PreDraw(TextSource source, out int ascent, out int lineHeight)
		{
			// Determine ascent and lineHeight from first character
			ascent = 0;
			lineHeight = 0;
			while (true)
			{
				int codepoint;
				if (!source.GetNextCodepoint(out codepoint))
				{
					break;
				}

				var glyph = GetDynamicGlyph(null, codepoint);
				if (glyph == null)
				{
					continue;
				}

				FontMetrics metrics;
				GetMetrics(glyph.FontSourceIndex, out metrics);
				ascent = metrics.Ascent;
				lineHeight = metrics.LineHeight;
				break;
			}

			source.Reset();
		}

		internal override Bounds InternalTextBounds(TextSource source, Vector2 position,
			float characterSpacing, float lineSpacing)
		{
			if (source.IsNull)
				return Bounds.Empty;

			var result = base.InternalTextBounds(source, position, characterSpacing, lineSpacing);

			return result;
		}

		private static int GetKerningsKey(int glyph1, int glyph2)
		{
			return ((glyph1 << 16) | (glyph1 >> 16)) ^ glyph2;
		}

		internal override float GetKerning(FontGlyph glyph, FontGlyph prevGlyph)
		{
			if (!FontSystem.UseKernings)
			{
				return 0.0f;
			}


			var dynamicGlyph = (DynamicFontGlyph)glyph;
			var dynamicPrevGlyph = (DynamicFontGlyph)prevGlyph;
			if (dynamicGlyph.FontSourceIndex != dynamicPrevGlyph.FontSourceIndex)
			{
				return 0.0f;
			}

			var key = GetKerningsKey(prevGlyph.Id, dynamicGlyph.Id);
			var result = 0;
			if (!Kernings.TryGetValue(key, out result))
			{
				var fontSource = FontSystem.FontSources[dynamicGlyph.FontSourceIndex];
				result = fontSource.GetGlyphKernAdvance(prevGlyph.Id, dynamicGlyph.Id, dynamicGlyph.FontSize);

				Kernings[key] = result;
			}

			return result;
		}
	}
}