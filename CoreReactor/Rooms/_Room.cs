using System.Linq;
using System.Collections.Generic;
using System.Text;
using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace CoreReactor
{
    public class Room
    {

        protected GraphicsDevice graphicsDevice;
        public Game1 parentGame;
        public Game1.ROOMS thisRoom;

        public Room(Game1.ROOMS room)
        {
            this.thisRoom = room;
        }

        public virtual void InitRoom(GraphicsDevice graphicsDevice, Game1 parentGame)
        {
            this.graphicsDevice = graphicsDevice;
            this.parentGame = parentGame;
        }

        public virtual void LoadRoom(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {

        }

        public virtual void UpdateRoom(GameTime gameTime, KeyboardState keyboardState, GamePadState gamePadState)
        {

        }

        public virtual void DrawRoom()
        {



        }

        public virtual void DestroyRoom()
        {

        }

        public virtual void UpdateCamera()
        {

        }

    }
}
