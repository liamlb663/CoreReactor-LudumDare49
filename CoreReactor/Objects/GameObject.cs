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
    public class GameObject
    {

        #region Initialize Variables

        public JsonRoom parentRoom;
        public List<GameObject> gameObjects = new List<GameObject>();
        public List<Wall> walls = new List<Wall>();

        protected Texture2D sprite;
        protected Texture2D flashSprite; // For flashing white when hit
        protected Texture2D activeSprite;
        public Color drawColor = Color.White;
        protected Rectangle rect = new Rectangle(0, 0, 32, 32);

        public Vector2 position;
        public Vector2 scale = new Vector2(1f);
        public float rotation = 0f;
        public SpriteEffects spriteEffect = SpriteEffects.None;

        public float layerDepth = 0.6f;
        protected Vector2 orgin = Vector2.Zero;
        public bool active = true;
        public Vector2 center
        {
            get
            {
                return position + new Vector2(boundingBoxWidth / 2, boundingBoxHeight / 2);
            }
        }

        public bool solid = false;
        protected int boundingBoxWidth, boundingBoxHeight;
        protected Vector2 boundingBoxOffset;
        protected Texture2D boundingBoxSprite;
        protected const bool drawBoundingBox = true;
        protected Color debugColor = new Color(120, 120, 120, 120);

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)((position.X + boundingBoxOffset.X)-(orgin.X*scale.X)), (int)((position.Y + boundingBoxOffset.Y)-(orgin.Y*scale.Y)), boundingBoxWidth, boundingBoxHeight);
            }

        }

        #endregion

        #region Main 5

        public virtual void Init(JsonRoom parentRoom)
        {
            this.parentRoom = parentRoom;
        }

        public virtual void Load(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            boundingBoxSprite = new Texture2D(graphicsDevice, boundingBoxWidth, boundingBoxHeight, false, SurfaceFormat.Color);
            Color[] colorData = new Color[boundingBoxWidth * boundingBoxHeight];

            for (int i = 0; i < boundingBoxWidth * boundingBoxHeight; i++)
                colorData[i] = Color.White;
            boundingBoxSprite.SetData<Color>(colorData);

            colorData = new Color[sprite.Width * sprite.Height];
            flashSprite = new Texture2D(graphicsDevice, sprite.Width, sprite.Height);
            sprite.GetData<Color>(colorData);
            for (int i = 0; i < colorData.Length; i++)
            {
                if (colorData[i] != new Color(0, 0, 0, 0))
                {
                    colorData[i] = new Color(255, 255, 255, 255);
                }
                else
                {
                    colorData[i] = new Color(0, 0, 0, 0);
                }
            }
            flashSprite.SetData<Color>(colorData);

            //Set to default
            activeSprite = sprite;
        }

        public virtual void Update(bool[] controls)
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (active)
            {
                if (activeSprite != null)
                    spriteBatch.Draw(activeSprite, new Vector2((int)position.X, (int)position.Y), rect, drawColor, rotation, orgin, scale, spriteEffect, layerDepth);

                if (drawBoundingBox == true && boundingBoxSprite != null)
                    spriteBatch.Draw(boundingBoxSprite, BoundingBox, null, new Color(120, 120, 120, 120), 0f, Vector2.Zero, SpriteEffects.None, 1f);
            }
        }

        public virtual void Dispose()
        {
            sprite.Dispose();
        }

        #endregion

        #region Helper Functions

        protected virtual bool CheckPosition(Vector2 pos, string type = null)
        {
            bool WallCheck = CheckWall(pos, type);
            if (WallCheck)
                return true;

            bool ObjectCheck = CheckObject(pos, type);
            if (ObjectCheck)
                return true;

            return false;
        }

        protected virtual bool CheckWall(Vector2 pos, string type = null)
        {
            Vector2 placeHold = this.position;
            this.position = pos;

            for (int i = 0; i < walls.Count(); i++)
            {
                if (BoundingBox.Intersects(walls[i].rectangle) == true)
                {
                    if (type != null)
                    {
                       bool typeCheck = walls[i].Type == type;
                        if (typeCheck)
                        {
                            this.position = placeHold;
                            return true;
                        }
                    }
                    else
                    {
                        this.position = placeHold;
                        return true;
                    }
                }
            }
            this.position = placeHold;
            return false;
        }

        protected virtual bool CheckObject(Vector2 pos, string type = null)
        {
            Vector2 placeHold = this.position;
            this.position = pos;

            for (int i = 0; i < gameObjects.Count(); i++)
            {
                if (gameObjects[i].solid == true)
                {
                    if (BoundingBox.Intersects(gameObjects[i].BoundingBox) == true && gameObjects[i] != this && gameObjects[i].active)
                    {
                        if (type != null)
                        {
                            bool typeCheck = (gameObjects[i].GetType().BaseType.ToString() == type);
                            if (!typeCheck)
                                typeCheck = (gameObjects[i].GetType().ToString() == type);
                            if (typeCheck)
                            {
                                this.position = placeHold;
                                return true;
                            }
                        }
                        else
                        {
                            this.position = placeHold;
                            return true;
                        }
                    }
                }
            }
            this.position = placeHold;
            return false;
        }

        /// <summary>
        /// Checks for collision includes objects that aren't solid
        /// </summary>
        /// <param name="pos">Position to check for collision</param>
        /// <param name="type">Type of object to collide with in string form</param>
        /// <returns>Returns Bool of collision, true is collided</returns>
        protected virtual List<GameObject> CheckUnsolidObject(Vector2 pos, string type = null)
        {
            Vector2 placeHold = this.position;
            this.position = pos;
            List<GameObject                                                                                                                         > output = new List<GameObject>();

            for (int i = 0; i < gameObjects.Count(); i++)
            {
                if (BoundingBox.Intersects(gameObjects[i].BoundingBox) == true && gameObjects[i] != this && gameObjects[i].active)
                {
                    if (type != null)
                    {
                        bool typeCheck = (gameObjects[i].GetType().BaseType.ToString() == type);
                        if (!typeCheck)
                            typeCheck = (gameObjects[i].GetType().ToString() == type);
                        if (typeCheck)
                        {
                            this.position = placeHold;
                            output.Add(gameObjects[i]);
                        }
                    }
                    else
                    {
                        this.position = placeHold;
                        output.Add(gameObjects[i]);
                    }
                }
            }
            this.position = placeHold;

            if (output.Count != 0)
                return output;

            return null;
        }

        public virtual GameObject UpdateParentRoom(List<Wall> walls, List<GameObject> gameObjects)
        {
            this.walls = walls;
            this.gameObjects = gameObjects;
            return this;
        }

        #endregion

    }
}
