using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace NoTrashLeftBehind
{
    public class SpriteSheet
    {
        public Texture2D sheetTexture;
        private string sheetFileName;
        public int ImagePixelWidth;
        public int ImagePixelHeight;
        public int Offset;
        public int Gutter;
        public int Columns;
        public int Rows;

        public Dictionary<string, Sprite> Sprites;

        public SpriteSheet(string sheetFileName, int pixelWidth, int pixelHeight, int offset, int gutter)
        {
            this.sheetFileName = sheetFileName;
            this.ImagePixelWidth = pixelWidth;
            this.ImagePixelHeight = pixelHeight;
            this.Offset = offset;
            this.Gutter = gutter;
            Sprites = new Dictionary<string, Sprite>();

        }

        public void AddSprite(string name, Sprite sprite)
        {
            Sprites.Add(name, sprite);
        }

        public Sprite GetSprite(string name)
        {
            return Sprites[name];
        }

        public void LoadContent(ContentManager content)
        {
            sheetTexture = content.Load<Texture2D>(sheetFileName);
            this.Columns = (sheetTexture.Width - Offset) / (ImagePixelWidth + Gutter);
            this.Rows = (sheetTexture.Height - Offset) / (ImagePixelHeight + Gutter);

            foreach(Sprite s in Sprites.Values)
            {
                s.LoadContent();
            }
        }
    }
}
