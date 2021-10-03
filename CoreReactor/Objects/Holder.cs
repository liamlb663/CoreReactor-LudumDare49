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
    public class Holder : GameObject
    {
        private int indexTimer = 0;
        private int indexFlip = 5;

        public Holder(Vector2 position)
        {
            this.position = position;
        }

        public override void Init(JsonRoom parentRoom)
        {
            base.Init(parentRoom);
            position.Y += 2;
            boundingBoxWidth = 16 * 2;
            boundingBoxHeight = 15 * 2;
            rect.Width = 16;
            rect.Height = 15;
            rect.Y = 1;
            position.Y += 30;
            solid = false;

            orgin.X = 8;
            orgin.Y = 15;
            scale = new Vector2(2f);

        }

        public override void Load(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            sprite = contentManager.Load<Texture2D>("Objects\\Holder");

            base.Load(contentManager, graphicsDevice);
        }

        public override void Update(bool[] controls)
        {
            base.Update(controls);
            try
            {
                if (CheckUnsolidObject(position).Contains(parentRoom.player) && parentRoom.coreGrabbed == true)
                {
                    parentRoom.coreGrabbed = false;
                    Vector2 tpos = new Vector2(position.X, position.Y - 64);
                    Core core = new Core(tpos);
                    core.Init(parentRoom);
                    core.Load(parentRoom.parentGame.Content, parentRoom.parentGame.GraphicsDevice);
                    parentRoom.gameObjects.Add(core);
                    parentRoom.parentGame.RestartGame();
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
                if (indexTimer > 1)
                {
                    indexTimer = 0;
                }
            }
            rect.X = (indexTimer * (boundingBoxWidth / 2)) + (indexTimer * 2) + 1;
            base.Draw(spriteBatch);
        }

    }
}
