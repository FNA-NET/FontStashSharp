using System;
using System.Collections.Generic;
using System.IO;
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
		private const int EffectAmount = 1;
		private const string Text = "The quick brown\nfox jumps over\nthe lazy dog";
		private const int CharacterSpacing = 4;
		private const int LineSpacing = 8;

		private readonly GraphicsDeviceManager _graphics;

		public static Game1 Instance { get; private set; }

		private SpriteBatch _spriteBatch;
		private FontSystem _fontSystem;

		private Texture2D _white;
		private bool _animatedScaling = false;
		private float _angle;

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

			_white = new Texture2D(GraphicsDevice, 1, 1);
			_white.SetData(new[] { Color.White });

			GC.Collect();
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			KeyboardUtils.Begin();

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

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			TimeSpan total = gameTime.TotalGameTime;

			// TODO: Add your drawing code here
			_spriteBatch.Begin();

			Vector2 scale = _animatedScaling
				? new Vector2(1 + .25f * (float)Math.Sin(total.TotalSeconds * .5f))
				: Vector2.One;

			var position = new Vector2(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 2);

			var font = _fontSystem.GetFont(32);
			var size = font.MeasureString(Text, scale, characterSpacing: CharacterSpacing, lineSpacing: LineSpacing);
			var rads = (float)(_angle * Math.PI / 180);
			var normalizedOrigin = new Vector2(0.5f, 0.5f);

			_spriteBatch.Draw(_white, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y), 
				null, Color.Green, rads, normalizedOrigin, SpriteEffects.None, 0.0f);
			_spriteBatch.DrawString(font, Text, position, Color.White, rads, size * normalizedOrigin, scale, characterSpacing: CharacterSpacing, lineSpacing: LineSpacing);

			_spriteBatch.End();

			_angle += 0.4f;

			while (_angle >= 360.0f)
			{
				_angle -= 360.0f;
			}

			base.Draw(gameTime);
		}
	}
}