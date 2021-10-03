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
    public class Core : GameObject
    {
        private int indexTimer = 0;
        private int indexFlip = 5;

        public Core(Vector2 position)
        {
            this.position = position;
        }

        public override void Init(JsonRoom parentRoom)
        {
            base.Init(parentRoom);
            position.Y += 2;
            boundingBoxWidth = 12*2;
            boundingBoxHeight = 16*2;
            rect.Width = 12;
            rect.Height = 16;
            rect.Y = 1;

            solid = false;

            orgin.X = 6;
            orgin.Y = 8;
            scale = new Vector2(2f);

        }

        public override void Load(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            sprite = contentManager.Load<Texture2D>("Objects\\Core");

            base.Load(contentManager, graphicsDevice);
        }

        public override void Update(bool[] controls)
        {
            base.Update(controls);
            var totalSeconds = parentRoom.parentGame.gameTime.TotalGameTime.TotalSeconds * 5;
            rotation = (float)(Math.Sin(totalSeconds)/2);

            try
            {
                if (CheckUnsolidObject(position).Contains(parentRoom.player) && parentRoom.thisRoom == Game1.ROOMS.Start)
                {
                    parentRoom.coreGrabbed = true;
                    active = false;
                }
            }
            catch (System.NullReferenceException)
            {

            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            indexFlip++;
            if (indexFlip > 5)
            {
                indexFlip = 0;
                indexTimer++;
                if (indexTimer > 5)
                {
                    indexTimer = 0;
                }
            }
            rect.X = (indexTimer * (boundingBoxWidth/2)) + (indexTimer * 2) + 1;
            base.Draw(spriteBatch);
        }

    }
}
