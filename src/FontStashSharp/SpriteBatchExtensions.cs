using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FontStashSharp
{
	public static class SpriteBatchExtensions
	{
		/// <summary>
		/// Draws a text
		/// </summary>
		/// <param name="batch">A SpriteBatch</param>
		/// <param name="text">The text which will be drawn</param>
		/// <param name="position">The drawing location on screen</param>
		/// <param name="color">A color mask</param>
		/// <param name="rotation">A rotation of this text in radians</param>
		/// <param name="origin">Center of the rotation</param>
		/// <param name="scale">A scaling of this text. Null means the scaling is (1, 1)</param>
		/// <param name="layerDepth">A depth of the layer of this string</param>
		public static float DrawString(this SpriteBatch batch, SpriteFontBase font, string text, Vector2 position, Color color,
			float rotation = 0, Vector2 origin = default(Vector2), Vector2? scale = null,
			float layerDepth = 0.0f, float characterSpacing = 0.0f, float lineSpacing = 0.0f,
			TextStyle textStyle = TextStyle.None)
		{
			return font.DrawText(batch, text, position, color, rotation, origin, scale, layerDepth, characterSpacing, lineSpacing, textStyle);
		}

		/// <summary>
		/// Draws a text
		/// </summary>
		/// <param name="batch">A SpriteBatch</param>
		/// <param name="text">The text which will be drawn</param>
		/// <param name="position">The drawing location on screen</param>
		/// <param name="colors">Colors of glyphs</param>
		/// <param name="rotation">A rotation of this text in radians</param>
		/// <param name="origin">Center of the rotation</param>
		/// <param name="scale">A scaling of this text. Null means the scaling is (1, 1)</param>
		/// <param name="layerDepth">A depth of the layer of this string</param>
		public static float DrawString(this SpriteBatch batch, SpriteFontBase font, string text, Vector2 position, Color[] colors,
			float rotation = 0, Vector2 origin = default(Vector2), Vector2? scale = null, 
			float layerDepth = 0.0f, float characterSpacing = 0.0f, float lineSpacing = 0.0f,
			TextStyle textStyle = TextStyle.None)
		{
			return font.DrawText(batch, text, position, colors, rotation, origin, scale, layerDepth, characterSpacing, lineSpacing, textStyle);
		}

		/// <summary>
		/// Draws a text
		/// </summary>
		/// <param name="batch">A SpriteBatch</param>
		/// <param name="text">The text which will be drawn</param>
		/// <param name="position">The drawing location on screen</param>
		/// <param name="color">A color mask</param>
		/// <param name="rotation">A rotation of this text in radians</param>
		/// <param name="origin">Center of the rotation</param>
		/// <param name="scale">A scaling of this text. Null means the scaling is (1, 1)</param>
		/// <param name="layerDepth">A depth of the layer of this string</param>
		public static float DrawString(this SpriteBatch batch, SpriteFontBase font, StringSegment text, Vector2 position, Color color,
			float rotation = 0, Vector2 origin = default(Vector2), Vector2? scale = null, 
			float layerDepth = 0.0f, float characterSpacing = 0.0f, float lineSpacing = 0.0f,
			TextStyle textStyle = TextStyle.None)
		{
			return font.DrawText(batch, text, position, color, rotation, origin, scale, layerDepth, characterSpacing, lineSpacing, textStyle);
		}

		/// <summary>
		/// Draws a text
		/// </summary>
		/// <param name="batch">A SpriteBatch</param>
		/// <param name="text">The text which will be drawn</param>
		/// <param name="position">The drawing location on screen</param>
		/// <param name="colors">Colors of glyphs</param>
		/// <param name="rotation">A rotation of this text in radians</param>
		/// <param name="origin">Center of the rotation</param>
		/// <param name="scale">A scaling of this text. Null means the scaling is (1, 1)</param>
		/// <param name="layerDepth">A depth of the layer of this string</param>
		public static float DrawString(this SpriteBatch batch, SpriteFontBase font, StringSegment text, Vector2 position, Color[] colors,
			float rotation = 0, Vector2 origin = default(Vector2), Vector2? scale = null, 
			float layerDepth = 0.0f, float characterSpacing = 0.0f, float lineSpacing = 0.0f,
			TextStyle textStyle = TextStyle.None)
		{
			return font.DrawText(batch, text, position, colors, rotation, origin, scale, layerDepth, characterSpacing, lineSpacing, textStyle);
		}

		/// <summary>
		/// Draws a text
		/// </summary>
		/// <param name="batch">A SpriteBatch</param>
		/// <param name="text">The text which will be drawn</param>
		/// <param name="position">The drawing location on screen</param>
		/// <param name="color">A color mask</param>
		/// <param name="rotation">A rotation of this text in radians</param>
		/// <param name="origin">Center of the rotation</param>
		/// <param name="scale">A scaling of this text. Null means the scaling is (1, 1)</param>
		/// <param name="layerDepth">A depth of the layer of this string</param>
		public static float DrawString(this SpriteBatch batch, SpriteFontBase font, StringBuilder text, Vector2 position, Color color,
			float rotation = 0, Vector2 origin = default(Vector2), Vector2? scale = null, 
			float layerDepth = 0.0f, float characterSpacing = 0.0f, float lineSpacing = 0.0f,
			TextStyle textStyle = TextStyle.None)
		{
			return font.DrawText(batch, text, position, color, rotation, origin, scale, layerDepth, characterSpacing, lineSpacing, textStyle);
		}

		/// <summary>
		/// Draws a text
		/// </summary>
		/// <param name="batch">A SpriteBatch</param>
		/// <param name="text">The text which will be drawn</param>
		/// <param name="position">The drawing location on screen</param>
		/// <param name="colors">Colors of glyphs</param>
		/// <param name="rotation">A rotation of this text in radians</param>
		/// <param name="origin">Center of the rotation</param>
		/// <param name="scale">A scaling of this text. Null means the scaling is (1, 1)</param>
		/// <param name="layerDepth">A depth of the layer of this string</param>
		public static float DrawString(this SpriteBatch batch, SpriteFontBase font, StringBuilder text, Vector2 position, Color[] colors,
			float rotation = 0, Vector2 origin = default(Vector2), Vector2? scale = null, 
			float layerDepth = 0.0f, float characterSpacing = 0.0f, float lineSpacing = 0.0f,
			TextStyle textStyle = TextStyle.None)
		{
			return font.DrawText(batch, text, position, colors, rotation, origin, scale, layerDepth, characterSpacing, lineSpacing, textStyle);
		}
	}
}