using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace NoTrashLeftBehind
{
    public static class Config
    {
        public static int GAME_WIDTH = 900;
        public static int GAME_HEIGHT = 900;
        public static int VIEWPORT_WIDTH = 900;
        public static int VIEWPORT_HEIGHT = 900;

        public static string SHEET_CONFIG_FILE_NAME = "Content\\AssetsConfig.txt";
        public static string TRASH_SHEET_CONFIG_FILE_NAME = "Content\\TrashConfig.txt";
        public static string OVERVIEW_CONFIG_FILE_NAME = "Content\\OverviewConfig.txt";

        public const float TICK_INTERVAL = 1000 / 60f;

        public static int SELECTION_WIDTH = 3;
        public static int SELECTION_HEIGHT = 3;
        public static Color SELECTION_COLOR = Color.Yellow;
        public static float SELECTION_OPACITY = 0.25f;
        //public static GRID_INFO GAME_GRID = new GRID_INFO(9, 9, 0.05625f, .1f, .45f, .8f);
        public static GRID_INFO GAME_GRID = new GRID_INFO(7, 7, 0.11111111f, 0.11111111f, 0.777777777f, 0.7777777f);

        public struct GRID_INFO
        {
            private float x, y, width, height;
            private int tilesX, tilesY;

            public GRID_INFO(int tilesWide, int tilesHigh, float x_percent, float y_percent, float width_percent, float height_percent)
            {
                this.tilesX = tilesWide;
                this.tilesY = tilesHigh;
                this.x = x_percent;
                this.y = y_percent;
                this.width = width_percent;
                this.height = height_percent;
            }
            public int TilesWide { get { return tilesX; } }
            public int TilesHigh { get { return tilesY; } }

            public int X { get { return (int)(this.x * Config.GAME_WIDTH); } }
            public int Y { get { return (int)(this.y * Config.GAME_HEIGHT); } }
            public int Width { get { return (int)(this.width * Config.GAME_WIDTH); } }
            public int Height { get { return (int)(this.height * Config.GAME_HEIGHT); } }
            public Rectangle Rect { get { return new Rectangle(X, Y, Width, Height); } }
        }

        // GRID_INFO(tilesWide, tilesHigh, x, y, width, height);

        public static Color GRID_COLOR = Color.Red;
    }
}
