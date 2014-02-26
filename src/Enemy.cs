using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpringGameJam2013
{
    class Enemy
    {
        public Texture2D Texture;
        public Vector2 Position;
        public float Speed;
        public bool Active;
        public int Damage;
        public int Health;
        public int Value;
        public Rectangle BoundingBox
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height); }
        }

        public Enemy(Texture2D texture, Vector2 position, float speed, int damage, int health, int value)
        {
            this.Texture = texture;
            this.Position = position;
            this.Speed = speed;
            this.Active = true;
            this.Damage = damage;
            this.Health = health;
            this.Value = value;
        }

        public void Update()
        {
            Position.X -= Speed;
            if ((Position.X < -BoundingBox.Width) || (Health <= 0))
                Active = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}
