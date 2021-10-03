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
    public class JsonRoom : Room
    {

        #region Initialize

        protected LevelData levelData;

        public SpriteFont arial;
        public List<GameObject> gameObjects = new List<GameObject>();
        public Player player;
        private Vector2 playerSpawn = Vector2.Zero;
        private List<Wall> walls = new List<Wall>();
        private List<Rectangle> cameraBounds = new List<Rectangle>();
        private List<Rectangle> exits = new List<Rectangle>();
        private Rectangle currentBound;
        protected int Time = 60 * 1;

        public bool coreGrabbed = true;

        protected Texture2D backgroundSprite;

        private SpriteBatch BackBatch;
        private SpriteBatch WallBatch;
        private SpriteBatch ObjectsBatch;
        private SpriteBatch UiBatch;

        private Viewport viewport;
        private Camera camera;

        public JsonRoom(Game1.ROOMS room) : base(room)
        {

        }

        #endregion

        #region Main 5

        public override void InitRoom(GraphicsDevice graphicsDevice, Game1 parentGame)
        {
            this.graphicsDevice = graphicsDevice;
            this.parentGame = parentGame;
            this.viewport = new Viewport(0, 0, Game1.ScreenWidth, Game1.ScreenHeight);
            this.camera = new Camera(viewport);
            this.currentBound = new Rectangle(0, 0, viewport.Width, viewport.Height);
            camera.CornerPosition(Vector2.Zero);

            CreateMap();

            for (int i = 0; i < cameraBounds.Count(); i++)
            {
                if (cameraBounds[i].Contains(player.position))
                {
                    currentBound = cameraBounds[i];
                }
            }
            for (int i = 0; i < 30; i++)
            {
                UpdateCamera();
            }

            for (int i = 0; i < gameObjects.Count(); i++)
            {
                gameObjects[i].Init(this);
            }
        }

        public override void LoadRoom(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            WallBatch = new SpriteBatch(graphicsDevice);
            BackBatch = new SpriteBatch(graphicsDevice);
            ObjectsBatch = new SpriteBatch(graphicsDevice);
            UiBatch = new SpriteBatch(graphicsDevice);
            arial = contentManager.Load<SpriteFont>("File");
            for (int i = 0; i < walls.Count(); i++)
            {
                walls[i].Load(contentManager, graphicsDevice);
            }

            for (int i = 0; i < gameObjects.Count(); i++)
            {
                gameObjects[i].Load(contentManager, graphicsDevice);
                gameObjects[i].UpdateParentRoom(walls, gameObjects);
            }
        }

        public override void UpdateRoom(GameTime gameTime, KeyboardState keyboardState, GamePadState gamePadState)
        {
            bool[] controls = CheckInputs(keyboardState, gamePadState);

            if (thisRoom == Game1.ROOMS.Mid)
            {
                Time--;
                Debug.WriteLine(Time);
                if (Time < 0)
                {
                    parentGame.InstantRestartGame();
                }
            }
            for (int i = 0; i < gameObjects.Count(); i++)
            {
                gameObjects[i].Update(controls);
            }

            for (int i = 0; i < exits.Count(); i++)
            {
                if (exits[i].Contains(player.center))
                {
                    if (coreGrabbed == true)
                    {
                        NextRoom();
                    }
                    else
                    {
                        player.position = playerSpawn;
                    }
                }
            }
        }

        public override void DrawRoom()
        {
            UpdateCamera();
            Resolution.BeginDraw();
            Matrix finalMatrix = Matrix.Multiply(camera.Transform, Resolution.getTransformationMatrix());

            DrawBackground();

            DrawWalls(finalMatrix);

            DrawObjects(finalMatrix);

        }

        public override void DestroyRoom()
        {
            gameObjects.Clear();
            walls.Clear();
            cameraBounds.Clear();
        }

        #endregion

        #region Helper Functions

        public override void UpdateCamera()
        {
            Rectangle viewportRectangle = new Rectangle((int)camera.Position.X, (int)camera.Position.Y, viewport.Width, viewport.Height);
            Rectangle targetRectangle = new Rectangle((int)player.center.X-(viewport.Width/2), (int)player.center.Y-(viewport.Height/2), viewport.Width, viewport.Height);

            targetRectangle.X = Math.Max(currentBound.X, targetRectangle.X);
            targetRectangle.Y = Math.Max(currentBound.Y, targetRectangle.Y);
            targetRectangle.X = Math.Min(currentBound.Right - viewport.Width, targetRectangle.X);
            targetRectangle.Y = Math.Min(currentBound.Bottom - viewport.Height, targetRectangle.Y);

            camera.CornerPosition(new Vector2(targetRectangle.X, targetRectangle.Y));

            camera.UpdateCamera(viewport);
        }
        
        private bool[] CheckInputs(KeyboardState keyboardState, GamePadState gamePadState)
        {
            bool[] output = new bool[9];
            Keys[] chars = new Keys[] { Keys.Left, Keys.Up, Keys.Down, Keys.Right, /*Jump*/Keys.Z, /*Shoot*/Keys.X, /*Swap*/Keys.LeftShift, Keys.A, Keys.S };
            Buttons[] buttons = new Buttons[] { Buttons.DPadLeft, Buttons.DPadUp, Buttons.DPadDown, Buttons.DPadRight, /*Jump*/Buttons.A, /*Shoot*/Buttons.X, Buttons.B, Buttons.RightShoulder, Buttons.LeftShoulder };

            for (int i = 0; i < chars.Length; i++)
            {
                output[i] = keyboardState.IsKeyDown(chars[i]);
            }

            for (int i = 0; i < buttons.Length; i++)
            {

                if (output[i] == false)
                    output[i] = gamePadState.IsButtonDown(buttons[i]);

            }

            return output;
        }

        private void CreateMap()
        {
            walls.AddRange( levelData.walls );
            player = levelData.player;
            gameObjects.Add(player);
            playerSpawn = player.position;
            gameObjects.AddRange(levelData.objects);
            cameraBounds.AddRange(levelData.cameraBounds);
            exits.AddRange(levelData.LevelTriggers);
        }

        protected virtual void NextRoom()
        {
            parentGame.InstantSetRoom(thisRoom);
        }

        #endregion

        #region Draw Functions

        private void DrawBackground()
        {
            BackBatch.Begin(SpriteSortMode.FrontToBack, null, SamplerState.PointClamp, null, null, null, Resolution.getTransformationMatrix());
            BackBatch.Draw(backgroundSprite, Vector2.Zero, Color.White);
            BackBatch.End();
        }

        private void DrawWalls(Matrix finalMatrix)
        {
            WallBatch.Begin(SpriteSortMode.FrontToBack, null, SamplerState.PointClamp, null, null, null, finalMatrix);
            for (int i = 0; i < walls.Count(); i++)
            {
                walls[i].Draw(WallBatch);
            }
            WallBatch.End();
        }

        private void DrawObjects(Matrix finalMatrix)
        {
            ObjectsBatch.Begin(SpriteSortMode.FrontToBack, null, SamplerState.PointClamp, null, null, null, finalMatrix);
            for (int i = 0; i < gameObjects.Count(); i++)
            {
                gameObjects[i].Draw(ObjectsBatch);
            }
            ObjectsBatch.End();


            UiBatch.Begin(SpriteSortMode.FrontToBack, null, null, null, null, null, Resolution.getTransformationMatrix());
            switch (thisRoom)
            {
                case Game1.ROOMS.Start:
                    UiBatch.DrawString(arial, "The core in unstable!\nYou don't have much time to contain it", new Vector2(8, 8), Color.Black);
                    break;
                case Game1.ROOMS.Mid:
                    UiBatch.DrawString(arial, "Time:\n" + ((int)(Time/60)) + " Seconds Left", new Vector2(8, 8), Color.Black);
                    break;
                case Game1.ROOMS.End:
                    UiBatch.DrawString(arial, "Good Job! Place the Core on the containment stand", new Vector2(8, 8), Color.Black);
                    break;
            }
            UiBatch.End();
        }

        #endregion
    }
}
