﻿using Microsoft.Xna.Framework;

namespace FontStashSharp.RichText
{
	public struct TextChunkGlyph
	{
		public int Index;
		public int Codepoint;
		public Rectangle Bounds;
		public int XAdvance;
		public int LineTop;
		public TextChunk TextChunk;
	}
}
