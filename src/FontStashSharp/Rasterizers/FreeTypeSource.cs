using FreeTypeSharp;
using static FreeTypeSharp.FT;
using static FreeTypeSharp.FT_LOAD;
using static FreeTypeSharp.FT_Render_Mode_;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FontStashSharp.Rasterizers.FreeType
{
	public unsafe class FreeTypeSource
	{
		private static FT_LibraryRec_* _libraryHandle;
		private GCHandle _memoryHandle;
		private FT_FaceRec_* _faceRec;
		FT_Render_Mode_ _renderMode;

		public FreeTypeSource(byte[] data, FT_Render_Mode_ renderMode)
		{
			_renderMode = renderMode;

			FT_Error err;
			if (_libraryHandle == default)
			{
				FT_LibraryRec_* libraryRef;
				err = FT_Init_FreeType(&libraryRef);

				if (err != FT_Error.FT_Err_Ok)
					throw new FreeTypeException(err);

				_libraryHandle = libraryRef;
			}

			_memoryHandle = GCHandle.Alloc(data, GCHandleType.Pinned);

			FT_FaceRec_* faceRef;
			err = FT_New_Memory_Face(_libraryHandle, (byte*)_memoryHandle.AddrOfPinnedObject(), (IntPtr)data.Length, IntPtr.Zero, &faceRef);

			if (err != FT_Error.FT_Err_Ok)
				throw new FreeTypeException(err);

			_faceRec = faceRef;
		}

		~FreeTypeSource()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_faceRec != default)
			{
				FT_Done_Face(_faceRec);
				_faceRec = default;
			}

			if (_memoryHandle.IsAllocated)
				_memoryHandle.Free();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public int? GetGlyphId(int codepoint)
		{
			var result = FT_Get_Char_Index(_faceRec, (UIntPtr)codepoint);
			if (result == 0)
			{
				return null;
			}

			return (int?)result;
		}

		public int GetGlyphKernAdvance(int previousGlyphId, int glyphId, float fontSize)
		{
			FT_Vector_ kerning;
			if (FT_Get_Kerning(_faceRec, (uint)previousGlyphId, (uint)glyphId, FT_Kerning_Mode_.FT_KERNING_DEFAULT, &kerning) != FT_Error.FT_Err_Ok)
			{
				return 0;
			}

			return (int)kerning.x >> 6;
		}

		private void SetPixelSizes(float width, float height)
		{
			FT_Size_RequestRec_ request = new FT_Size_RequestRec_()
			{
				type = FT_Size_Request_Type_.FT_SIZE_REQUEST_TYPE_REAL_DIM,
				width = (IntPtr)(width * 64), // Convert pixel size to 26.6 fractional points
				height = (IntPtr)(height * 64),
				horiResolution = 72,
				vertResolution = 72
			};

			var err = FT_Request_Size(_faceRec, &request);
			if (err != FT_Error.FT_Err_Ok)
				throw new FreeTypeException(err);
		}

		private void LoadGlyph(int glyphId)
		{
			var err = FT_Load_Glyph(_faceRec, (uint)glyphId, FT_LOAD_DEFAULT | FT_LOAD_TARGET_NORMAL | FT_LOAD_COLOR);
			if (err != FT_Error.FT_Err_Ok)
				throw new FreeTypeException(err);
		}

		private void GetCurrentGlyph(out FT_GlyphSlotRec_ glyph)
		{
			glyph = Marshal.PtrToStructure<FT_GlyphSlotRec_>((IntPtr)_faceRec->glyph);
		}

		public void GetGlyphMetrics(int glyphId, float fontSize, out int advance, out int x0, out int y0, out int x1, out int y1)
		{
			SetPixelSizes(0, fontSize);
			LoadGlyph(glyphId);

			FT_GlyphSlotRec_ glyph;
			GetCurrentGlyph(out glyph);
			advance = (int)glyph.advance.x >> 6;
			x0 = (int)glyph.metrics.horiBearingX >> 6;
			y0 = -(int)glyph.metrics.horiBearingY >> 6;
			x1 = x0 + ((int)glyph.metrics.width >> 6);
			y1 = y0 + ((int)glyph.metrics.height >> 6);
		}

		public unsafe void GetMetricsForSize(float fontSize, out int ascent, out int descent, out int lineHeight)
		{
			SetPixelSizes(0, fontSize);

			ascent = (int)_faceRec->size->metrics.ascender >> 6;
			descent = (int)_faceRec->size->metrics.descender >> 6;
			lineHeight = (int)_faceRec->size->metrics.height >> 6;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static byte LuminanceFromLinearRGB(byte r, byte g, byte b)
		{
			// Luminance Y is defined by the CIE 1931 XYZ color space. Linear RGB to Y is a weighted average based on factors from the color conversion matrix:
			// Y = 0.2126*R + 0.7152*G + 0.0722*B. Computed on the integer pipe.
			return (byte)((4732UL * r + 46871UL * g + 13933UL * b) >> 16);
		}
		public unsafe void RasterizeGlyphBitmap(int glyphId, float fontSize, byte[] buffer, int startIndex, int outWidth, int outHeight, int outStride)
		{
			SetPixelSizes(0, fontSize);
			LoadGlyph(glyphId);

			FT_Render_Glyph(_faceRec->glyph, _renderMode);

			FT_GlyphSlotRec_ glyph;
			GetCurrentGlyph(out glyph);
			var ftbmp = glyph.bitmap;

			fixed (byte* bptr = buffer)
			{
				for (var y = 0; y < outHeight; ++y)
				{
					var pos = (y * outStride) + startIndex;
					byte* dst = bptr + pos;
					byte* src = (byte*)ftbmp.buffer + y * ftbmp.pitch;

					if (ftbmp.pixel_mode == FT_Pixel_Mode_.FT_PIXEL_MODE_GRAY)
					{
						for (var x = 0; x < outWidth; ++x)
						{
							var c = *src++;
							*dst++ = c;
							*dst++ = c;
							*dst++ = c;
							*dst++ = c;
						}
					}
					else if (ftbmp.pixel_mode == FT_Pixel_Mode_.FT_PIXEL_MODE_MONO)
					{
						for (var x = 0; x < outWidth; x += 8)
						{
							var bits = *src++;
							for (int b = 0; b < 8; b++)
							{
								var color = ((bits >> (7 - b)) & 1) == 0 ? 0 : 255;
								*dst++ = (byte)color;
								*dst++ = (byte)color;
								*dst++ = (byte)color;
								*dst++ = (byte)color;
							}
						}
					}
					else if (ftbmp.pixel_mode == FT_Pixel_Mode_.FT_PIXEL_MODE_LCD)
					{
						for (var x = 0; x < outWidth; ++x)
						{
							var r = *src++;
							var g = *src++;
							var b = *src++;
							*dst++ = r;
							*dst++ = g;
							*dst++ = b;
							*dst++ = LuminanceFromLinearRGB(r, g, b);
						}
					}
					else if (ftbmp.pixel_mode == FT_Pixel_Mode_.FT_PIXEL_MODE_BGRA)
					{
						for (var x = 0; x < outWidth; ++x)
						{
							var b = *src++;
							var g = *src++;
							var r = *src++;
							var a = *src++;
							*dst++ = r;
							*dst++ = g;
							*dst++ = b;
							*dst++ = a;
						}
					}
				}
			}
		}
	}
}
