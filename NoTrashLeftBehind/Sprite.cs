using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace NoTrashLeftBehind
{
    public class Sprite
    {
        private const int FRAME_RATE = 60;

        public int PixelWidth;
        public int PixelHeight;
        private int columns;
        private int rows;
        private List<List<int>> sheetIndex;
        private SpriteSheet spriteSheet;
        private SpriteEffects flip;
        public Color TextureColor = Color.White;
        private List<List<Rectangle>> sourceRects;
        //public TimeSpan timer;
        //public float AnimationSpeed;
        //public int Frame = 0;
        public float Depth = 0.5f;

        public Sprite(SpriteSheet sheet, int pixelWidth, int pixelHeight, int imagesX, int imagesY, List<List<int>> frames, SpriteEffects toFlip = SpriteEffects.None)
        {
            spriteSheet = sheet;
            PixelWidth = pixelWidth;
            PixelHeight = pixelHeight;
            columns = imagesX;
            rows = imagesY;
            sheetIndex = frames;
            flip = toFlip;
            //timer = new TimeSpan(0);
            //AnimationSpeed = 0.5f;
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle rect)
        {
            List<Rectangle> sources = sourceRects[0];

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    Rectangle source = sources[y * columns + x];
                    Rectangle region = rect;
                    Rectangle dest = new Rectangle(region.X + ((region.Width / columns) * x), region.Y + ((region.Height / rows) * y), region.Width / columns, region.Height / rows);
                    spriteBatch.Draw(spriteSheet.sheetTexture, dest, source, TextureColor, 0f, new Vector2(0), flip, Depth);

                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle rect, SpriteAnimationData animationData)
        {
            int frame = animationData.Frame;
            List<Rectangle> sources = sourceRects[frame];

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    Rectangle source = sources[y * columns + x];
                    Rectangle region = rect;
                    Rectangle dest = new Rectangle(region.X + ((region.Width / columns) * x), region.Y + ((region.Height / rows) * y), region.Width / columns, region.Height / rows);
                    spriteBatch.Draw(spriteSheet.sheetTexture, dest, source, TextureColor, 0f, new Vector2(0), flip, Depth);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle rect, Color color)
        {
            Color old = TextureColor;
            TextureColor = color;
            Draw(spriteBatch, rect);
            TextureColor = color;
        }

        /*
        public void Draw(SpriteBatch spriteBatch, Rectangle rect, int frame)
        {
            List<Rectangle> sources = sourceRects[frame];
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    Rectangle source = sources[y * columns + x];
                    Rectangle region = rect;
                    Rectangle dest = new Rectangle(region.X + ((region.Width / columns) * x), region.Y + ((region.Height / rows) * y), region.Width / columns, region.Height / rows);
                    spriteBatch.Draw(spriteSheet.sheetTexture, dest, source, TextureColor, 0f, new Vector2(0), flip, Depth);
                }
            }
        }
        */

        public void LoadContent()
        {
            sourceRects = new List<List<Rectangle>>();
            int eachImageX = (PixelWidth / columns);
            int eachImageY = (PixelHeight / rows);
            for (int i = 0; i < sheetIndex.Count; i++)
            {
                sourceRects.Add(new List<Rectangle>());
                List<int> list = sheetIndex[i];
                for (int j = 0; j < list.Count; j++)
                {
                    int pos = list[j];
                    int x = pos % spriteSheet.Columns;
                    int y = pos / spriteSheet.Columns;
                    sourceRects[i].Add(new Rectangle(spriteSheet.Offset + x * (eachImageX + spriteSheet.Gutter), spriteSheet.Offset + y * (eachImageY + spriteSheet.Gutter), eachImageX, eachImageY));
                }
            }
        }

        public int Frames
        {
            get
            {
                return sourceRects.Count;
            }
        }

        /*
        public void Update(GameTime gameTime)
        {
            // Could optionally account for gameTime delay here to potentially skip animation frames when stuttering
            timer = timer.Add(gameTime.ElapsedGameTime);
            if (timer.TotalMilliseconds > sourceRects.Count * MillisecondsPerFrame)
            {
                timer = new TimeSpan(0);
            }
        }

        private float MillisecondsPerFrame
        {
            get
            {
                return (int)((1000f / AnimationSpeed) / FRAME_RATE);
            }
            set
            {
                AnimationSpeed = (1000f / FRAME_RATE) / value;
            }
        }

        public void ResetAnimation()
        {
            timer = new TimeSpan(0);
        }
        */
    }
}