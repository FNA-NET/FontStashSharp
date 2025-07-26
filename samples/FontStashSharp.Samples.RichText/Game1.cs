using System;
using System.Collections.Generic;
using System.IO;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FontStashSharp.Samples
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		private static readonly string[] Strings = new[]
		{
			"First line./nSecond line.",
			"This is /c[red]colored /c[#00f0fa]ext, /cdcolor could be set either /c[lightGreen]by name or /c[#fa9000ff]by hex code.",
			"Text in default font./n/f[Roboto-Bold.ttf, 24]Bold and smaller font. /f[Roboto-Italic.ttf, 48]Italic and larger font./n/fdBack to the default font.",
			"E=mc/v[-8]2/n/vdMass–energy equivalence.",
			"A small tree: /i[mangrove1.png]",
			"A small /c[red]tree: /v[8]/i[mangrove1.png]/vd/cd/tuand some text",
			"This is the first line. This is the second line. This is the third line.",
			"This is the first line. This is the second line. This is the third line.",
			"This is the first line. This is the second line. This is the third line.",
		};

		private readonly GraphicsDeviceManager _graphics;

		public static Game1 Instance { get; private set; }

		private SpriteBatch _spriteBatch;
		private Texture2D _white;
		private bool _animatedScaling = false;
		private RichTextLayout _richText;
		private int _stringIndex = 0;
		private readonly Dictionary<string, FontSystem> _fontCache = new Dictionary<string, FontSystem>();
		private readonly Dictionary<string, Texture2D> _textureCache = new Dictionary<string, Texture2D>();

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

			RichTextDefaults.FontResolver = p =>
			{
				// Parse font name and size
				var args = p.Split(',');
				var fontName = args[0].Trim();
				var fontSize = int.Parse(args[1].Trim());

				// _fontCache is field of type Dictionary<string, FontSystem>
				// It is used to cache fonts
				FontSystem fontSystem;
				if (!_fontCache.TryGetValue(fontName, out fontSystem))
				{
					// Load and cache the font system
					fontSystem = new FontSystem();
					fontSystem.AddFont(File.ReadAllBytes(Path.Combine(Utility.AssetsDirectory, fontName)));
					_fontCache[fontName] = fontSystem;
				}

				// Return the required font
				return fontSystem.GetFont(fontSize);
			};

			RichTextDefaults.ImageResolver = p =>
			{
				Texture2D texture;

				// _textureCache is field of type Dictionary<string, Texture2D>
				// it is used to cache textures
				if (!_textureCache.TryGetValue(p, out texture))
				{
					using (var stream = File.OpenRead(Path.Combine(Utility.AssetsDirectory, p)))
					{
						texture = Texture2D.FromStream(GraphicsDevice, stream);
					}

					_textureCache[p] = texture;
				}

				return new TextureFragment(texture);
			};

			var fontSystem = new FontSystem();
			fontSystem.AddFont(File.ReadAllBytes(Path.Combine(Utility.AssetsDirectory, @"Roboto-Regular.ttf")));

			_richText = new RichTextLayout
			{
				Font = fontSystem.GetFont(32),
				Text = Strings[_stringIndex],
				VerticalSpacing = 8,
			};

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
				//				_currentFontSystem.UseKernings = !_currentFontSystem.UseKernings;
			}

			if (KeyboardUtils.IsPressed(Keys.LeftShift))
			{
				_animatedScaling = !_animatedScaling;
			}

			if (KeyboardUtils.IsPressed(Keys.Space))
			{
				++_stringIndex;
				if (_stringIndex >= Strings.Length)
				{
					_stringIndex = 0;
				}

				_richText.Text = Strings[_stringIndex];
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
			_spriteBatch.DrawString(_richText.Font, "Press 'Space' to switch between strings.", Vector2.Zero, Color.White);

			Vector2 scale = _animatedScaling
				? new Vector2(1 + .25f * (float)Math.Sin(total.TotalSeconds * .5f))
				: Vector2.One;

			var viewportSize = new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

			_richText.Width = null;
			_richText.Height = null;

			if (_stringIndex < 6)
			{
			}
			else if (_stringIndex == 6)
			{
				_richText.Width = 300;
			} else
			{
				_richText.Width = 260;

				if (_stringIndex >= 7)
				{
					_richText.Height = 100;
				}

				if (_stringIndex == 7)
				{
					_richText.AutoEllipsisMethod = AutoEllipsisMethod.Character;
				} else if (_stringIndex == 8)
				{
					_richText.AutoEllipsisMethod = AutoEllipsisMethod.Word;
				}
			}

			var position = new Vector2(0, viewportSize.Y / 2 - _richText.Size.Y / 2);
			var size = _richText.Size;

			if (_richText.Width != null)
			{
				size.X = _richText.Width.Value;
			}

			if (_richText.Height != null)
			{
				size.Y = _richText.Height.Value;
			}

			var rect = new Rectangle((int)position.X,
				(int)position.Y,
				(int)(size.X * scale.X),
				(int)(size.Y * scale.Y));
			_spriteBatch.Draw(_white, rect, Color.Green);

			_richText.Draw(_spriteBatch, position, Color.White, scale: scale);

			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}