using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace CoreReactor
{
    public class Wall
    {
        protected float layerDepth = 0.5f;
        protected Vector2 position = Vector2.Zero;
        protected Vector2 size = Vector2.Zero;
        protected Color DrawColor = Color.Black;
        public string Type = "Wall";
        public Rectangle rectangle
        {
            get
            {
                return new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
            }
        }

        private Texture2D sprite;
        private Vector2 tileSize = new Vector2(32, 32);

        public Wall(Vector2 position, Vector2 size)
        {
            this.position = position;
            this.size = size;
        }

        public Wall(int x, int y, int width, int height)
        {
            this.position = new Vector2(x, y);
            this.size = new Vector2(width, height);
        }

        public virtual void Load(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            sprite = new Texture2D(graphicsDevice, (int)tileSize.X, (int)tileSize.Y, false, SurfaceFormat.Color);
            Color[] colorData = new Color[(int)tileSize.X * (int)tileSize.Y];

            for (int i = 0; i < tileSize.X * tileSize.Y; i++)
                colorData[i] = Color.White;
            sprite.SetData<Color>(colorData);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, null, DrawColor, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 1f);
        }

    }

    public class DieWall : Wall
    {

        public DieWall(Vector2 position, Vector2 size) : base(position, size)
        {

        }
        public DieWall(int x, int y, int width, int height) : base(x, y, width, height)
        {
        }
        public override void Load(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            DrawColor = Color.Red;
            Type = "Die";
            base.Load(contentManager, graphicsDevice);
        }
    }

}
