using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoTrashLeftBehind
{
    public class Board : Object
    {
        private SpriteSheet trashTextures;

        List<Trash> TrashTiles;
        int Width;
        int Height;
        int TypesOfTrash;
        Selection Selection;

        private bool mouseDown = false;

        private Random rand = new Random();

        public Board(Game1 game, CollisionBody collision) : base(game, collision)
        {
            TrashTiles = new List<Trash>();
        }

        public void Initialize(int width, int height, int typesOfTrash)
        {
            Width = width;
            Height = height;
            TypesOfTrash = typesOfTrash;

            float tileWidth = (float)this.Collision.Region().Width / width;
            float tileHeight = (float)this.Collision.Region().Height / height;

            for (int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    Vector2 position = new Vector2(this.Collision.Position.X + tileWidth * x, this.Collision.Position.Y + tileHeight * y);
                    Vector2 size = new Vector2(tileWidth, tileHeight);
                    RectangleBody collision = new RectangleBody(position, size);

                    Trash newTrash = new Trash(this.Game, collision, rand.Next() % typesOfTrash);
                    TrashTiles.Add(newTrash);
                }
            }

            RectangleBody rect = new RectangleBody(new Vector2(0, 0), new Vector2(tileWidth * Config.SELECTION_WIDTH, tileHeight * Config.SELECTION_HEIGHT));
            Selection = new Selection(Game, rect);

            ScrambleBoard();
        }

        public void LoadContent(Sprite tile, SpriteSheet trashSheet, Sprite selection)
        {
            this.Sprite = tile;
            trashTextures = trashSheet;

            List<Sprite> TrashSprites = new List<Sprite>();

            for(int i = 0; i < TypesOfTrash; i++)
            {
                Sprite sprite = trashSheet.GetSprite("trash-" + i);
                TrashSprites.Add(sprite);
            }

            for (int i = 0; i < TrashTiles.Count; i++)
            {
                TrashTiles[i].LoadContent(tile, TrashSprites[TrashTiles[i].TrashID]);
            }

            Selection.LoadContent(selection);
        }

        public void Update(MouseState state)
        {

            if(this.Collision.CollidesWith(state.Position.ToVector2()))
            {
                Vector2 nearestPos = nearestPosition(state.Position.ToVector2());
                Selection.Update(true, nearestPos);

                if (state.LeftButton == ButtonState.Pressed && !mouseDown)
                {
                    FlipTiles(nearestPos);
                    mouseDown = true;
                }
                else if(mouseDown && state.LeftButton == ButtonState.Released)
                {
                    mouseDown = false;
                }
            }
            else
            {
                Selection.Update(false);
            }
        }

        public new void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointClamp);

            for (int i = 0; i < TrashTiles.Count; i++)
            {
                TrashTiles[i].Draw(spriteBatch);
            }

            Selection.Draw(spriteBatch);

            spriteBatch.End();
        }

        public void FlipTile(Trash trash)
        {
            trash.Flip();
        }

        public void FlipTiles(List<Trash> trashList)
        {
            foreach(Trash trash in trashList)
            {
                trash.Flip();
            }
        }

        public void ScrambleBoard()
        {
            for(int i = 0; i < 20 + rand.Next() % 20; i++)
            {
                int xPos = rand.Next() % (Width - Config.SELECTION_WIDTH + 1);
                int yPos = rand.Next() % (Height - Config.SELECTION_HEIGHT + 1);
                for(int y = 0; y < Config.SELECTION_HEIGHT; y++)
                {
                    for(int x = 0; x < Config.SELECTION_WIDTH; x++)
                    {
                        FlipTile(xPos + x, yPos + y);
                    }
                }
            }
        }

        public void FlipTiles(Vector2 selectionPosition)
        {
            int xMin = (int)((selectionPosition.X - this.Collision.Position.X) / ((int)this.Collision.Region().Width / Width));
            int YMin = (int)((selectionPosition.Y - this.Collision.Position.Y) / ((int)this.Collision.Region().Height / Height));
            for (int y = YMin; y < YMin + Config.SELECTION_HEIGHT; y++)
            {
                for (int x = xMin; x < xMin + Config.SELECTION_WIDTH; x++)
                {
                    FlipTile(x, y);
                }
            }

        }

        public void FlipTile(int x, int y)
        {
            TrashTiles[y * Width + x].Flip();
        }

        public void ScrambleTrashTextures()
        {
            foreach(Trash trash in TrashTiles)
            {
                trash.TrashID = rand.Next() % TypesOfTrash;

                trash.LoadContent(Sprite, trashTextures.GetSprite("trash-" + trash.TrashID));
            }
        }

        private Vector2 nearestPosition(Vector2 position)
        {
            int mouseX = (int)position.X;
            int mouseY = (int)position.Y;
            int boardX = (int)this.Collision.Position.X;
            int boardY = (int)this.Collision.Position.Y;
            int boardWidth = (int)this.Collision.Region().Width;
            int boardHeight = (int)this.Collision.Region().Height;
            int relMouseX = mouseX - boardX;
            int relMouseY = mouseY - boardY;
            float tileWidth = (float)this.Collision.Region().Width / Width;
            float tileHeight = (float)this.Collision.Region().Height / Height;
            float selOffsetX = ((Config.SELECTION_WIDTH - 1) / 2f * tileWidth);
            float selOffsetY = ((Config.SELECTION_HEIGHT - 1) / 2f * tileHeight);
            Vector2 nearestPosition = new Vector2((int)(((int)((relMouseX - selOffsetX) / tileWidth) * tileWidth) + boardX), (int)(((int)((relMouseY - selOffsetY) / tileHeight) * tileHeight) + boardY));
            return Vector2.Clamp(nearestPosition, this.Collision.Position, new Vector2(boardX + boardWidth - Config.SELECTION_WIDTH * tileWidth, boardY + boardHeight - Config.SELECTION_HEIGHT * tileHeight));
        }
    }
}
