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
    public class Mid2 : JsonRoom
    {

        public Mid2(Game1.ROOMS room) : base(room)
        {

        }

        public override void InitRoom(GraphicsDevice graphicsDevice, Game1 parentGame)
        {
            levelData = new LevelData("C:\\Users\\liaml\\Documents\\Code Shit\\CoreReactor\\CoreReactor\\Json\\Mid2.json");
            Time = 60 * 8;

            base.InitRoom(graphicsDevice, parentGame);
        }

        public override void LoadRoom(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {

            backgroundSprite = contentManager.Load<Texture2D>("Backgrounds\\bg01");

            base.LoadRoom(contentManager, graphicsDevice);
        }

    }
}
