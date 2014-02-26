using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpringGameJam2013
{
    class Player
    {
        public Texture2D Texture;
        public Vector2 Position;
        public float Speed;
        public bool Active;
        public int Health;
        public Rectangle BoundingBox
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height); }
        }

        public Player(Texture2D texture, Vector2 position, float speed)
        {
            this.Texture = texture;
            this.Position = position;
            this.Speed = speed;
            this.Active = true;
            this.Health = 100;
        }

        public void Update()
        {
            if (Health <= 0)
                Active = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}
