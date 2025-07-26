﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FontStashSharp.RichText
{
	public class TextureFragment : IRenderable
	{
		public Texture2D Texture { get; private set; }
		public Rectangle Region { get; private set; }

		public Point Size
		{
			get
			{
				return new Point((int)(Region.Width * Scale.X + 0.5f), (int)(Region.Height * Scale.Y + 0.5f));
			}
		}

		public Vector2 Scale = Vector2.One;

		public TextureFragment(Texture2D texture, Rectangle region)
		{
			if (texture == null)
			{
				throw new ArgumentNullException(nameof(texture));
			}

			Texture = texture;
			Region = region;
		}

		public TextureFragment(Texture2D texture) :
			this(texture, new Rectangle(0, 0, texture.Width, texture.Height))
		{
		}

		public void Draw(FSRenderContext context, Vector2 position, Color color)
		{
			context.DrawImage(Texture, Region, position, Scale, Color.White);
		}
	}
}