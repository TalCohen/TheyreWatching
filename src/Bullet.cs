using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpringGameJam2013
{
    class Bullet
    {
        Viewport viewport;

        public Texture2D Texture;
        public Vector2 Position;
        public float Speed;
        public bool Active;
        public int Damage;
        public Rectangle BoundingBox
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height); }
        }

        public Bullet(Texture2D texture, Vector2 position, float speed, int damage, Viewport viewport)
        {
            this.Texture = texture;
            this.Position = position;
            this.Speed = speed;
            this.Active = true;
            this.viewport = viewport;
            this.Damage = damage;
        }

        public void Update()
        {
            //Moves bullet to the right and terminates if off-screen.
                Position.X += Speed;

            if (Position.X > viewport.Width)
                Active = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}
