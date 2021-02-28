using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NoTrashLeftBehind
{
    public class Object
    {
        public Game1 Game;
        public CollisionBody Collision;
        public Vector2 Velocity;
        public Vector2 Acceleration;
        public Sprite Sprite;

        public Object(Game1 game, CollisionBody collision)
        {
            Game = game;
            Collision = collision;
            Collision.Parent = this;

            Velocity = new Vector2(0f);
            Acceleration = new Vector2(0f);
            
        }

        public void Update(GameTime gameTime)
        {
            float speedRatio = ((float) gameTime.ElapsedGameTime.TotalMilliseconds) / (1000f / 60f);
            Velocity += Acceleration * speedRatio;
            Collision.Position += Velocity * speedRatio;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            this.Sprite.Draw(spriteBatch, Collision.Region());
        }

        public void LoadContent(Sprite sprite)
        {
            //Texture = content.Load<Texture2D>(textureString);
            Sprite = sprite;
        }
    }
}
