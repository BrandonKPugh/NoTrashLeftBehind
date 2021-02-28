using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoTrashLeftBehind
{
    public class Selection : Object
    {
        bool Visable = false;
        public Selection(Game1 game, CollisionBody collision) : base(game, collision)
        {
            
        }

        public void Update(bool show)
        {
            if (show)
            {
                this.Visable = true;
            }
            else
            {
                this.Visable = false;
            }
        }

        public void Update(bool show, Vector2 position)
        {
            if(show)
            {
                this.Visable = true;
                this.Collision.Position = position;
            }
            else
            {
                this.Visable = false;
            }
        }

        public new void Draw(SpriteBatch spriteBatch)
        {
            if(Visable)
            {
                ((Object)this).Draw(spriteBatch);
            }
        }
    }
}
