using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FontStashSharp.Samples
{
	/// <summary>
	/// Indicates how text is aligned.
	/// </summary>
	public enum Alignment
	{
		Left,
		Center,
		Right
	}

	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		private const int EffectAmount = 0;
		private const int CharacterSpacing = 2;
		private const int LineSpacing = 4;

		private readonly GraphicsDeviceManager _graphics;

		public static Game1 Instance { get; private set; }

		private SpriteBatch _spriteBatch;
		private FontSystem _fontSystem;
		private DynamicSpriteFont _font;

		private Texture2D _white;
		private bool _drawBackground = false;
		private bool _animatedScaling = false;

		private static readonly Color[] ColoredTextColors = new Color[]
		{
			Color.Red,
			Color.Blue,
			Color.Green,
			Color.Aquamarine,
			Color.Azure,
			Color.Chartreuse,
			Color.Lavender,
			Color.OldLace,
			Color.PaleGreen,
			Color.SaddleBrown,
			Color.IndianRed,
			Color.ForestGreen,
			Color.Khaki
		};

		public Game1()
		{
			Instance = this;

			_graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 1200,
				PreferredBackBufferHeight = 800
			};

			Window.AllowUserResizing = true;

			IsMouseVisible = true;
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			// Simple
			_fontSystem = new FontSystem();
			_fontSystem.AddFont(File.ReadAllBytes(@"Fonts/DroidSans.ttf"));
			_fontSystem.AddSystemFont("simsun");
			_fontSystem.AddSystemFont("seguiemj");
			_fontSystem.AddFont(File.ReadAllBytes(@"Fonts/DroidSansJapanese.ttf"));
			_fontSystem.AddFont(File.ReadAllBytes(@"Fonts/Symbola-Emoji.ttf"));

			_white = new Texture2D(GraphicsDevice, 1, 1);
			_white.SetData(new[] { Color.White });

			GC.Collect();
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			KeyboardUtils.Begin();

			if (KeyboardUtils.IsPressed(Keys.Space))
			{
				_drawBackground = !_drawBackground;
			}

			if (KeyboardUtils.IsPressed(Keys.Enter))
			{
				_fontSystem.UseKernings = !_fontSystem.UseKernings;
			}

			if (KeyboardUtils.IsPressed(Keys.LeftShift))
			{
				_animatedScaling = !_animatedScaling;
			}

			KeyboardUtils.End();
		}

		private void DrawString(string text, ref Vector2 cursor, Alignment alignment, Color[] glyphColors, Vector2 scale)
		{
			Vector2 dimensions = _font.MeasureString(text);
			Vector2 origin = AlignmentOrigin(alignment, dimensions);

			if (_drawBackground)
			{
				var backgroundRect = new Rectangle((int)Math.Round(cursor.X - origin.X * scale.X),
					(int)Math.Round(cursor.Y - origin.Y * scale.Y),
					(int)Math.Round(dimensions.X * scale.X),
					(int)Math.Round(dimensions.Y * scale.Y));
				DrawRectangle(backgroundRect, Color.Green);

				var glyphs = _font.GetGlyphs(text, cursor, origin, scale);
				foreach (var r in glyphs)
				{
					DrawRectangle(r.Bounds, Color.Gray);
				}
			}

			_spriteBatch.DrawString(_font, text, cursor, glyphColors,
				scale: scale, origin: origin);
			cursor.Y += dimensions.Y + LineSpacing;
		}

		private void DrawString(string text, ref Vector2 cursor, Alignment alignment, Color color, Vector2 scale)
		{
			Vector2 dimensions = _font.MeasureString(text, 
				characterSpacing: CharacterSpacing, lineSpacing: LineSpacing);
			Vector2 origin = AlignmentOrigin(alignment, dimensions);

			if (_drawBackground)
			{
				var backgroundRect = new Rectangle((int)Math.Round(cursor.X - origin.X * scale.X),
					(int)Math.Round(cursor.Y - origin.Y * scale.Y),
					(int)Math.Round(dimensions.X * scale.X),
					(int)Math.Round(dimensions.Y * scale.Y));
				DrawRectangle(backgroundRect, Color.Green);

				var glyphs = _font.GetGlyphs(text, cursor, origin, scale, 
					characterSpacing: CharacterSpacing, lineSpacing: LineSpacing);
				foreach (var g in glyphs)
				{
					DrawRectangle(g.Bounds, Color.Gray);
				}
			}

			_spriteBatch.DrawString(_font, text, cursor, color, scale: scale, origin: origin,
				characterSpacing: CharacterSpacing, lineSpacing: LineSpacing);
			cursor.Y += dimensions.Y + LineSpacing;
		}

		private void DrawString(string text, ref Vector2 cursor, Alignment alignment, Vector2 scale)
		{
			DrawString(text, ref cursor, alignment, Color.White, scale);
		}

		private void DrawRectangle(Rectangle rectangle, Color color)
		{
			_spriteBatch.Draw(_white, rectangle, color);
		}

		private static Vector2 AlignmentOrigin(Alignment alignment, Vector2 dimensions)
		{
			switch (alignment)
			{
				case Alignment.Left:
					return Vector2.Zero;
				case Alignment.Center:
					return new Vector2(dimensions.X / 2, 0);
				case Alignment.Right:
					return new Vector2(dimensions.X, 0);
				default:
					return Vector2.Zero;
			}
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			TimeSpan total = gameTime.TotalGameTime;


			Vector2 scale = _animatedScaling
				? new Vector2(1 + .25f * (float)Math.Sin(total.TotalSeconds * .5f))
				: Vector2.One;

			// TODO: Add your drawing code here
			_spriteBatch.Begin();

			Vector2 cursor = Vector2.Zero;

			// Render some text

			_font = _fontSystem.GetFont(18, FontStyle.Regular);
			DrawString("ABCDEFG abcdefg 1234567890 你好世界😄", ref cursor, Alignment.Left, scale);
			_font = _fontSystem.GetFont(18, FontStyle.Bold);
			DrawString("ABCDEFG abcdefg 1234567890 你好世界😄", ref cursor, Alignment.Left, scale);
			_font = _fontSystem.GetFont(18, FontStyle.Italic);
			DrawString("ABCDEFG abcdefg 1234567890 你好世界😄", ref cursor, Alignment.Left, scale);
			DrawString("The quick いろは brown\nfox にほへ jumps over\nt🙌h📦e l👏a👏zy dog adfasoqiw yraldh ald halwdha ldjahw dlawe havbx get872rq", ref cursor, Alignment.Left, scale);

			_font = _fontSystem.GetFont(30);
			DrawString("The quick いろは brown\nfox にほへ jumps over\nt🙌h📦e l👏a👏zy dog", ref cursor, Alignment.Left, Color.Bisque, scale);

			DrawString("Colored Text", ref cursor, Alignment.Left, ColoredTextColors, scale);

			// Render some scaled text with alignment using origin.

			Vector2 columnCursor = cursor;
			DrawString("Left-Justified", ref columnCursor, Alignment.Left, new Vector2(.75f) * scale);


			var width = GraphicsDevice.Viewport.Width;

			columnCursor = new Vector2(width / 2f, cursor.Y);
			DrawString("Centered", ref columnCursor, Alignment.Center, new Vector2(1) * scale);

			columnCursor = new Vector2(width, cursor.Y);
			DrawString("Right-Justified", ref columnCursor, Alignment.Right, new Vector2(1.5f) * scale);

			cursor = new Vector2(0, columnCursor.Y);

			// Render the atlas texture
			_font = _fontSystem.GetFont(26);
			DrawString("Texture:", ref cursor, Alignment.Left, Vector2.One);

			var texture = _fontSystem.Atlases[0].Texture;
			_spriteBatch.Draw(texture, cursor, Color.White);

			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}