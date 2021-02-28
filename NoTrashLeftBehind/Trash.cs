using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoTrashLeftBehind
{
    public class Trash : Object
    {
        public bool Visable = false;
        public Sprite TrashSprite;
        public int TrashID;

        public Trash(Game1 game, CollisionBody collision, int trashID) : base(game, collision)
        {
            TrashID = trashID;
        }

        public new void Draw(SpriteBatch spriteBatch)
        {
            if(Visable)
            {
                this.TrashSprite.Draw(spriteBatch, Collision.Region());
            }
            else
            {
                //this.Sprite.Draw(spriteBatch, Collision.Region());
            }
        }

        public void LoadContent(Sprite tile, Sprite trash)
        {
            Sprite = tile;
            TrashSprite = trash;
        }

        public void Flip()
        {
            Visable = !Visable;
        }
    }
}
