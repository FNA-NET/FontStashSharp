using System;
using FontStashSharp.Rasterizers.FreeType;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FreeTypeSharp;

namespace FontStashSharp
{
	public class FontAtlas
	{
		byte[] _pixelBuffer;
		byte[] _eraseBuffer;

		public int Width { get; private set; }

		public int Height { get; private set; }

		public int NodesNumber { get; private set; }

		internal FontAtlasNode[] Nodes { get; private set; }

		public Texture2D Texture { get; set; }

		public FontAtlas(int w, int h, int count, Texture2D texture)
		{
			Width = w;
			Height = h;
			Texture = texture;
			Nodes = new FontAtlasNode[count];
			Nodes[0].X = 0;
			Nodes[0].Y = 0;
			Nodes[0].Width = w;
			NodesNumber++;
		}

		public void InsertNode(int idx, int x, int y, int w)
		{
			if (NodesNumber + 1 > Nodes.Length)
			{
				var oldNodes = Nodes;
				var newLength = Nodes.Length == 0 ? 8 : Nodes.Length * 2;
				Nodes = new FontAtlasNode[newLength];
				for (var i = 0; i < oldNodes.Length; ++i)
				{
					Nodes[i] = oldNodes[i];
				}
			}

			for (var i = NodesNumber; i > idx; i--)
				Nodes[i] = Nodes[i - 1];
			Nodes[idx].X = x;
			Nodes[idx].Y = y;
			Nodes[idx].Width = w;
			NodesNumber++;
		}

		public void RemoveNode(int idx)
		{
			if (NodesNumber == 0)
				return;
			for (var i = idx; i < NodesNumber - 1; i++)
				Nodes[i] = Nodes[i + 1];
			NodesNumber--;
		}

		public void Reset(int w, int h)
		{
			Width = w;
			Height = h;
			NodesNumber = 0;
			Nodes[0].X = 0;
			Nodes[0].Y = 0;
			Nodes[0].Width = w;
			NodesNumber++;
		}

		public bool AddSkylineLevel(int idx, int x, int y, int w, int h)
		{
			InsertNode(idx, x, y + h, w);
			for (var i = idx + 1; i < NodesNumber; i++)
				if (Nodes[i].X < Nodes[i - 1].X + Nodes[i - 1].Width)
				{
					var shrink = Nodes[i - 1].X + Nodes[i - 1].Width - Nodes[i].X;
					Nodes[i].X += shrink;
					Nodes[i].Width -= shrink;
					if (Nodes[i].Width <= 0)
					{
						RemoveNode(i);
						i--;
					}
					else
					{
						break;
					}
				}
				else
				{
					break;
				}

			for (var i = 0; i < NodesNumber - 1; i++)
				if (Nodes[i].Y == Nodes[i + 1].Y)
				{
					Nodes[i].Width += Nodes[i + 1].Width;
					RemoveNode(i + 1);
					i--;
				}

			return true;
		}

		public int RectFits(int i, int w, int h)
		{
			var x = Nodes[i].X;
			var y = Nodes[i].Y;
			if (x + w > Width)
				return -1;
			var spaceLeft = w;
			while (spaceLeft > 0)
			{
				if (i == NodesNumber)
					return -1;
				y = Math.Max(y, Nodes[i].Y);
				if (y + h > Height)
					return -1;
				spaceLeft -= Nodes[i].Width;
				++i;
			}

			return y;
		}

		public bool AddRect(int rw, int rh, ref int rx, ref int ry)
		{
			var besth = Height;
			var bestw = Width;
			var besti = -1;
			var bestx = -1;
			var besty = -1;
			for (var i = 0; i < NodesNumber; i++)
			{
				var y = RectFits(i, rw, rh);
				if (y != -1)
					if (y + rh < besth || y + rh == besth && Nodes[i].Width < bestw)
					{
						besti = i;
						bestw = Nodes[i].Width;
						besth = y + rh;
						bestx = Nodes[i].X;
						besty = y;
					}
			}

			if (besti == -1)
				return false;
			if (!AddSkylineLevel(besti, bestx, besty, rw, rh))
				return false;

			rx = bestx;
			ry = besty;
			return true;
		}

		public void RasterizeGlyphBitmap(DynamicFontGlyph glyph, FreeTypeSource fontSource, FontStyle fontStyle)
		{
			if (glyph.IsEmpty)
			{
				return;
			}

			fontSource.RasterizeGlyphBitmap(glyph.Id, glyph.FontSize, fontStyle);

			if (fontStyle.HasFlag(FontStyle.Italic) && !fontSource.HasStyleFlag(FT_STYLE_FLAG.FT_STYLE_FLAG_ITALIC))
			{
				fontSource.GetCurrentGlyph(out var glyphRec);
				// Set new advance and size for italic glyph
				glyph.XAdvance = (int)glyphRec.advance.x >> 6;
				glyph.Size = new Point((int)glyphRec.bitmap.width, (int)glyphRec.bitmap.rows);
			}
		}

		public void RenderGlyph(GraphicsDevice graphicsDevice, DynamicFontGlyph glyph, FreeTypeSource fontSource, FontStyle fontStyle)
		{
			if (glyph.IsEmpty)
			{
				return;
			}

			const int cBytesPerPixel = 4;

			// Render glyph to pixel buffer(rgba32)
			var bufferSize = glyph.Size.X * glyph.Size.Y * cBytesPerPixel;
			var pixelBuffer = _pixelBuffer;

			if ((pixelBuffer == null) || (pixelBuffer.Length < bufferSize))
			{
				pixelBuffer = new byte[bufferSize];
				_pixelBuffer = pixelBuffer;
			}
			Array.Clear(pixelBuffer, 0, bufferSize);

			var eraseBufferSize = (glyph.Size.X + FontSystem.GlyphPad * 2) * (glyph.Size.Y + FontSystem.GlyphPad * 2) * 4;
			if ((_eraseBuffer == null) || (_eraseBuffer.Length < eraseBufferSize))
			{
				_eraseBuffer = new byte[eraseBufferSize];
			}

			// Create the atlas texture if required
			if (Texture == null)
			{
				Texture = Texture2DManager.CreateTexture(graphicsDevice, Width, Height);
			}

			// Erase an area where we are going to place a glyph
			var eraseArea = glyph.TextureRectangle;
			eraseArea.X = Math.Max(eraseArea.X - FontSystem.GlyphPad, 0);
			eraseArea.Y = Math.Max(eraseArea.Y - FontSystem.GlyphPad, 0);
			eraseArea.Width += FontSystem.GlyphPad * 2;
			if (eraseArea.Right > Width)
			{
				eraseArea.Width = Width - eraseArea.X;
			}
			eraseArea.Height += FontSystem.GlyphPad * 2;
			if (eraseArea.Bottom > Height)
			{
				eraseArea.Height = Height - eraseArea.Y;
			}

			Texture2DManager.SetTextureData(Texture, eraseArea, _eraseBuffer);

			fontSource.FetchGlyphBitmapBuffer(glyph.Id,
				glyph.FontSize,
				pixelBuffer,
				0,
				glyph.Size.X,
				glyph.Size.Y,
				glyph.Size.X * cBytesPerPixel, fontStyle);

			// Render glyph to texture
			Texture2DManager.SetTextureData(Texture, glyph.TextureRectangle, pixelBuffer);
		}
	}
}