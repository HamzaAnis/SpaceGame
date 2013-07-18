using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using WindowsGame1.Classes;

namespace WindowsGame1
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        KeyboardState newState = Keyboard.GetState();
        KeyboardState oldState = Keyboard.GetState();

        bool gameStarted = false;
        bool lifeCollectable = false;
        bool playerIsAlive = true;
        public ball[] balls = new ball[6]; 
        public static Random rand = new Random();
        uint i = 0;
        double timer = 1;
        uint score;
        int lives;

        Vector2 startVector;

        SpriteFont myFont;

        Texture2D playerTexture;
        Rectangle playerRectangle;

        Texture2D bgTexture;
        Rectangle bgRectangle;

        Texture2D pickupTexture;
        Rectangle pickupRectangle;

        Texture2D lifeTexture;
        Rectangle lifeRectangle;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.graphics.PreferredBackBufferWidth = 1000;
            this.graphics.PreferredBackBufferHeight = 600;
            this.graphics.IsFullScreen = true;

            this.IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = true;

            if (this.graphics.IsFullScreen)
            {
                this.graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                this.graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
            startVector = new Vector2(GraphicsDevice.Viewport.Width / 2 - 100, GraphicsDevice.Viewport.Height / 2 - 50);

            score = 0;
            lives = 10;
            foreach (ball element in balls)
            {
                element.initialize(GraphicsDevice);
            }
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            myFont = Content.Load<SpriteFont>("font");
            playerTexture = Content.Load<Texture2D>("character");
            playerRectangle = new Rectangle(10, 100, playerTexture.Width, playerTexture.Height);
            pickupTexture = Content.Load<Texture2D>("pickup");
            pickupRectangle = new Rectangle(500, 300, pickupTexture.Width, pickupTexture.Height);
            lifeTexture = Content.Load<Texture2D>("life");
            lifeRectangle = new Rectangle(0, 0, lifeTexture.Width, lifeTexture.Height);
            bgTexture = Content.Load<Texture2D>("background");
            bgRectangle = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            while (i < balls.Length)
            {
                balls[i] = new ball(Content.Load<Texture2D>("meteor"));
                i++;
            }
        }

        protected override void UnloadContent()
        {
            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                UnloadContent();
                this.Exit();
            }

            if (lives < 0)
            {
                playerIsAlive = false;
                endGame();
            }
            else
            {
                timer += gameTime.ElapsedGameTime.TotalSeconds;
                foreach (ball element in balls)
                {
                    element.checkBounds(GraphicsDevice);
                    if (playerRectangle.Intersects(element.rectangle) && playerIsAlive == true && timer > 2)
                    {
                        lives--;
                        element.randomize(GraphicsDevice);
                    }
                }

                if ((int)timer % 4 == 0)
                {
                    foreach (ball element in balls)
                    {
                        if (element.velocity.X > 0)
                        {
                            element.velocity.X += 0.1f;
                        }
                        else
                        {
                            element.velocity.X -= 0.1f;
                        }

                        if (element.velocity.Y > 0)
                        {
                            element.velocity.Y += 0.1f;
                        }
                        else
                        {
                            element.velocity.Y -= 0.1f;
                        }
                    }
                    timer++;
                }

                playerRectangle.X = (int)MathHelper.Clamp(Mouse.GetState().X, 0, GraphicsDevice.Viewport.Width - playerRectangle.Width);
                playerRectangle.Y = (int)MathHelper.Clamp(Mouse.GetState().Y, 0, GraphicsDevice.Viewport.Height - playerRectangle.Height);

                if (playerRectangle.Intersects(lifeRectangle) && playerIsAlive == true && lifeCollectable == true)
                {
                    lives++;
                    lifeCollectable = false;
                }

                if (playerRectangle.Intersects(pickupRectangle) && playerIsAlive == true)
                {
                    pickupRectangle.X = rand.Next(pickupRectangle.Width * 2, GraphicsDevice.Viewport.Width - pickupRectangle.Width);
                    pickupRectangle.Y = rand.Next(pickupRectangle.Height * 2, GraphicsDevice.Viewport.Height - pickupRectangle.Height);
                    score += (uint)timer;
                    if (rand.Next(0, 100) <= 5 && lifeCollectable == false)
                    {
                        lifeRectangle.X = rand.Next(lifeRectangle.Width * 2, GraphicsDevice.Viewport.Width - lifeRectangle.Width * 2);
                        lifeRectangle.Y = rand.Next(lifeRectangle.Height * 2, GraphicsDevice.Viewport.Height - lifeRectangle.Height * 2);
                        lifeCollectable = true;
                    }
                }
                base.Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(bgTexture, bgRectangle, Color.White);
            if (lives < 1)
            {
                spriteBatch.DrawString(myFont, " Press Space!\nFinal Score:", startVector, Color.White);
                spriteBatch.DrawString(myFont, Convert.ToString(score), new Vector2(startVector.X + 200, startVector.Y + 37), Color.White);
            }

            newState = Keyboard.GetState();

            if (gameStarted == false)
            {
                spriteBatch.DrawString(myFont, "Press Space!", startVector, Color.White);
                if (newState.IsKeyDown(Keys.Space) && oldState.IsKeyUp(Keys.Space))
                {
                    gameStarted = true;
                }
            }

            if (lives > 0 && gameStarted == true)
            {
                spriteBatch.Draw(playerTexture, playerRectangle, Color.White);
                spriteBatch.Draw(pickupTexture, pickupRectangle, Color.White);
                foreach (ball element in balls)
                {
                    element.Draw(spriteBatch);
                }
                spriteBatch.DrawString(myFont, "Score:", new Vector2(10, 10), Color.White);
                spriteBatch.DrawString(myFont, Convert.ToString(score), new Vector2(110, 10), Color.White);
                spriteBatch.DrawString(myFont, "Lives:", new Vector2(10, 40), Color.White);
                spriteBatch.DrawString(myFont, Convert.ToString(lives), new Vector2(110, 40), Color.White);
                if (lifeCollectable == true && playerIsAlive == true)
                {
                    spriteBatch.Draw(lifeTexture, lifeRectangle, Color.White);
                }
            }

            oldState = newState;
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public void endGame()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                foreach (ball element in balls)
                {
                    element.resetSpeed();
                    element.initialize(GraphicsDevice);
                }
                score = 0;
                lives = 10;
                playerIsAlive = true;
                timer = 1;
            }
        }
    }
}
