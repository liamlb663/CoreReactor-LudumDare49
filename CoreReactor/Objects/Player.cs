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
    public class Player : GameObject
    {

        #region Initialize

        //Inputs
        bool[] oldControls = new bool[9]{ true, true, true, true, true, true, true, true, true };
        bool[] controls;

        //X movemint
        private sbyte Xdir = 0;
        private sbyte LastXdir = 0;
        private float xVelo = 0;
        private Vector2 move = Vector2.Zero;
        private const float Speed = 5;
        private const float TopVelo = 2;
        private const float TopDeccel = 1;

        //Y movemint
        private const float trueGrav = 0.7f;
        private float grav = 0.7f;
        private float gravHoldMulti = 0.4f;
        private const int MAXVERTICALSPEED = 12;
        private const int jumpHeight = 8;
        private int jumpTimer = 60;
        private int jumpCheck = 10;
        private int groundTimer = 50;
        private int groundCheck = 5;

        public Player(Vector2 position)
        {
            this.position = position;
        }

        #endregion

        #region Main 4

        public override void Init(JsonRoom parentRoom)
        {
            base.Init(parentRoom);
            layerDepth = 0.9f;
            boundingBoxWidth = 16;
            boundingBoxHeight = 16;
            solid = false;

            rect = new Rectangle(0, 0, 16, 16);

            orgin = new Vector2(boundingBoxWidth / 2, boundingBoxHeight);
            scale = new Vector2(1f, 1f);

        }

        public override void Load(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            sprite = contentManager.Load<Texture2D>("Objects\\player");

            base.Load(contentManager, graphicsDevice);
        }

        public override void Update(bool[] controls)
        {
            if (this.controls != null)
                this.oldControls = this.controls;
            this.controls = controls;

            Xmovemint();
            Ymovemint();

            CollideMove();

            VarSet();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (move.X < 0)
                spriteEffect = SpriteEffects.FlipHorizontally;
            else if (move.X > 0)
                spriteEffect = SpriteEffects.None;

            base.Draw(spriteBatch);
        }

        #endregion

        #region Update Functions

        private void Xmovemint()
        {
            LastXdir = Xdir;
            Xdir = 0;
            if (controls[0])
                Xdir--;
            if (controls[3])
                Xdir++;

            if (Xdir != 0)
            {
                xVelo += (float)(Xdir / 0.5);
                xVelo = Math.Clamp(xVelo, -TopVelo, TopVelo);

                move.X += xVelo;
                move.X = Math.Clamp(move.X, -Speed, Speed);
            }
            else
            {
                if (LastXdir != 0)
                    xVelo = 0;

                xVelo += -(Math.Sign(move.X) * TopDeccel) /2;
                xVelo = Math.Clamp(xVelo, -TopDeccel, TopDeccel);

                move.X += xVelo;
                move.X = Math.Clamp(move.X, -Speed, Speed);

                if (Math.Abs(move.X) <= 1f)
                {
                    move.X = 0;
                    xVelo = 0;
                }
            }

        }

        private void Ymovemint()
        {

            ///Set Jump Timer
            if (controls[4] == true && oldControls[4] == false)
            {
                jumpTimer = 0;
            }

            //Grav lessens as i hold jump
            grav = trueGrav;
            if (controls[4] == true)
                grav = grav * gravHoldMulti;

            //Gravity
            if (move.Y < 2 && move.Y > -2)
                move.Y += grav * 0.9f;//Peak of jump
            else
                move.Y += grav;//Not peak of Jump

            move.Y = Math.Min(move.Y, MAXVERTICALSPEED);

            if (jumpTimer < jumpCheck && groundTimer < groundCheck)//JumpTimer Check
            {
                move.Y = -jumpHeight;
                jumpTimer = 60;
            }
        }

        private void CollideMove()
        {
            Vector2 intMove = new Vector2((int)move.X, (int)move.Y);

            if (CheckPosition(position + move, "Die"))
            {
                parentRoom.parentGame.InstantRestartGame();
            }

            if (CheckPosition(position + intMove, "Wall"))
            {
                if (CheckPosition(new Vector2(position.X + intMove.X, position.Y), "Wall"))
                {
                    while (!CheckPosition(new Vector2(position.X + Math.Sign(intMove.X), position.Y), "Wall"))
                    {
                        position.X += Math.Sign(intMove.X);
                    }
                    intMove.X = 0;
                    move.X = 0;
                }
                position.X += intMove.X;
                position.X = (int)position.X;

                if (CheckPosition(new Vector2(position.X, position.Y + intMove.Y), "Wall"))
                {
                    while (!CheckPosition(new Vector2(position.X, position.Y + Math.Sign(intMove.Y)), "Wall"))
                    {
                        position.Y += Math.Sign(intMove.Y);
                    }
                    intMove.Y = 0;
                    move.Y = 0;
                }
                position.Y += intMove.Y;
                position.Y = (int)position.Y;

            }
            else
            {
                position += intMove;
                position = new Vector2((int)position.X, (int)position.Y);
            }
        }

        #endregion

        #region Helper Functions
        
        private void VarSet()
        {
            if (CheckPosition(position + new Vector2(0, 1), "Wall"))
            {
                groundTimer = 0;
            }
            else
            {
                groundTimer++;
            }
        }

        #endregion
    }
}
