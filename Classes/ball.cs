using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WindowsGame1.Classes
{
    public class ball
    {
        public Texture2D texture;
        public Rectangle rectangle;

        public Vector2 origin;
        public Vector2 position;
        public float rotation;

        public static Random rand = new Random();
        public Vector2 velocity = new Vector2();
        public const float tangentVelocity = 7f;

        public ball (Texture2D newTexture)
        {
            texture = newTexture;
            rectangle = new Rectangle(0, 0, texture.Width, texture.Height);
        }

        public void initialize(GraphicsDevice gDevice)
        {
            this.rotation = rand.Next(1, 360);
            velocity.X = (float)Math.Cos(rotation) * tangentVelocity;
            velocity.Y = (float)Math.Sin(rotation) * tangentVelocity;
            rectangle.X = rand.Next(rectangle.Width * 3, gDevice.Viewport.Width - 100);
            rectangle.Y = rand.Next(rectangle.Height * 3, gDevice.Viewport.Height - 100);
            origin = new Vector2(rectangle.Width / 2, rectangle.Height / 2);
            position = new Vector2(rectangle.X, rectangle.Y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, null, Color.White, rotation, origin, SpriteEffects.None, 0);
        }

        public void Update()
        {
            position.X += velocity.X;
            position.Y += velocity.Y;
            rectangle.X = (int)position.X;
            rectangle.Y = (int)position.Y;
        }

        public void resetSpeed()
        {
            rotation = rand.Next(1, 360);
            velocity.X = (float)Math.Cos(rotation) * tangentVelocity;
            velocity.Y = (float)Math.Sin(rotation) * tangentVelocity;
        }

        public void randomize(GraphicsDevice gDevice)
        {
            rotation = rand.Next(1, 360);
            rectangle.X = rand.Next(rectangle.Width * 3, gDevice.Viewport.Width - 100);
            rectangle.Y = rand.Next(rectangle.Height * 3, gDevice.Viewport.Height - 100);
            origin = new Vector2(rectangle.Width / 2, rectangle.Height / 2);
            position = new Vector2(rectangle.X, rectangle.Y);
        }

        public void checkBounds(GraphicsDevice gDevice)
        {
            if (position.X > gDevice.Viewport.Width - rectangle.Width/2 || position.X < rectangle.Width/2)
            {
                velocity.X *= (-1);
            }
            if (position.Y > gDevice.Viewport.Height - rectangle.Height/2 || position.Y < rectangle.Height/2)
            {
                velocity.Y *= (-1);
            }
            if(!gDevice.Viewport.Bounds.Contains(rectangle.X, rectangle.Y))
            {
                initialize(gDevice);
            }
            Update();
        }
    }
}
