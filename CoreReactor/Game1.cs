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
    public class Game1 : Game
    {
        public GraphicsDeviceManager graphics;
        private KeyboardState keyboardState;
        private GamePadState gamePadState;
        public GameTime gameTime;

        //Room Indexing and information
        public enum ROOMS
        {
            Start,
            Mid,
            End
        }
        public Room[] StartRooms = new Room[] {
            new Start1(ROOMS.Start),
            new Start2(ROOMS.Start)
        };
        public Room[] MidRooms = new Room[] {
            new Mid1(ROOMS.Mid),
            new Mid2(ROOMS.Mid),
            new Mid3(ROOMS.Mid)
        };
        public Room[] EndRooms = new Room[] {
            new End1(ROOMS.End),
            new End2(ROOMS.End)
        };
        public Room activeRoom;
        public Room lastRoom;

        //Game Settings
        public const int ScreenWidth = 512;
        public const int ScreenHeight = 288;

        //Game
        private int middleLevels = 0;
        private int restartTimer = 90;
        private int restartCheck = 50;
        private bool restarting = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Resolution.Init(ref graphics);
            Resolution.SetVirtualResolution(ScreenWidth, ScreenHeight);
            int maxRes = (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / ScreenWidth);
            Resolution.SetResolution(ScreenWidth * 2, ScreenHeight * 2, false);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Random random = new Random();
            activeRoom = StartRooms[random.Next(StartRooms.Length)];
            base.Initialize();
        }

        protected override void LoadContent()
        {

            InitRoom();

        }

        protected override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
            gamePadState = GamePad.GetState(0);
            this.gameTime = gameTime;

            activeRoom.UpdateRoom(gameTime, keyboardState, gamePadState);

            if (restartTimer < restartCheck)
            {
                restartTimer++;
            }
            else
            {
                if (restarting == true)
                {
                    InstantSetRoom(ROOMS.End);
                    restarting = false;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            activeRoom.DrawRoom();

            base.Draw(gameTime);
        }

        public void InitRoom()
        {
            activeRoom.InitRoom(GraphicsDevice, this);
            activeRoom.LoadRoom(Content, GraphicsDevice);
        }


        public void InstantSetRoom(ROOMS oOMS)
        {

            activeRoom.DestroyRoom();
            lastRoom = activeRoom;

            Random random = new Random((int)gameTime.TotalGameTime.TotalMilliseconds);
            switch (oOMS)
            {
                case ROOMS.Start:
                    activeRoom = MidRooms[random.Next(MidRooms.Length)];
                    middleLevels = 0;
                    break;

                case ROOMS.Mid:
                    middleLevels++;
                    if (middleLevels < 3)
                    {
                        activeRoom = MidRooms[random.Next(MidRooms.Length)];
                    }
                    else
                    {
                        activeRoom = EndRooms[random.Next(EndRooms.Length)];
                        middleLevels = 0;
                    }
                    break;

                case ROOMS.End:
                    activeRoom = StartRooms[random.Next(StartRooms.Length)];
                    middleLevels = 0;
                    break;
            }

            InitRoom();
            for (int i = 0; i < 2; i++)
            {
                activeRoom.UpdateRoom(gameTime, keyboardState, gamePadState);
            }
            for (int i = 0; i < 2; i++)
            {
                activeRoom.DrawRoom();
            }

        }

        public void RestartGame()
        {
            restartTimer = 0;
            restarting = true;
        }

        public void InstantRestartGame()
        {
            restartTimer = 90;
            restarting = true;
        }
    }
}
