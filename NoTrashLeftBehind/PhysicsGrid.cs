using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace NoTrashLeftBehind
{
    public class PhysicsGrid
    {
        public List<PhysicsGridTile> Tiles;
        private int columns;
        private int rows;
        private int tileSize;
        private int maxWidth;
        private int maxHeight;

        public PhysicsGrid(Point point1, int width, int height, int tileSize)
        {
            this.tileSize = tileSize;
            int x = 0;
            int y = 0;
            Tiles = new List<PhysicsGridTile>();
            for(y = point1.Y; y < point1.Y + height; y += tileSize)
            {
                for(x = point1.X; x < point1.X + width; x += tileSize)
                {
                    Tiles.Add(new PhysicsGridTile(new Point(x, y), tileSize));
                }
            }
            columns = width / tileSize;
            rows = height / tileSize;
            maxWidth = width;
            maxHeight = height;
        }

        public PhysicsGridTile GetTile(int x, int y)
        {
            return Tiles[y * columns + x];
        }

        public Dictionary<uint, CollisionBody> GetPotentialCollisions(CollisionBody toCheck)
        {
            int xMin = 0;
            int xMax = 0;
            int yMin = 0;
            int yMax = 0;

            if(toCheck.Shape == CollisionBody.ShapeType.Circle)
            {
                CircleBody body = (CircleBody)toCheck;
                xMin = ((int)(Math.Max(body.Position.X - body.Radius / 2, 0)) / tileSize);
                xMax = (int)(Math.Min(body.Position.X + body.Radius / 2, maxWidth - 1)) / tileSize;
                yMin = ((int)(Math.Max(body.Position.Y - body.Radius / 2, 0)) / tileSize);
                yMax = (int)(Math.Min(body.Position.Y + body.Radius / 2, maxHeight - 1)) / tileSize;
            }
            else if(toCheck.Shape == CollisionBody.ShapeType.Rectangle)
            {
                RectangleBody body = (RectangleBody)toCheck;
                xMin = ((int)(Math.Max(body.Position.X, 0)) / tileSize);
                xMax = (int)(Math.Min(body.Position.X + body.Size.X, 49)) / tileSize;
                yMin = ((int)(Math.Max(body.Position.Y, 0)) / tileSize);
                yMax = (int)(Math.Min(body.Position.Y + body.Size.Y, 49)) / tileSize;
            }

            // Get every possible CollisionBody
            Dictionary<uint, CollisionBody> potentials = new Dictionary<uint, CollisionBody>();
            for (int y = yMin; y <= yMax; y++)
            {
                for (int x = xMin; x <= xMax; x++)
                {
                    foreach(CollisionBody c in Tiles[y * columns + x].Data.Values)
                    {
                        if(!potentials.ContainsKey(c.id))
                        {
                            potentials.Add(c.id, c);
                        }
                    }
                }
            }

            return potentials;
        }

        public void Add(CollisionBody body)
        {
            foreach(PhysicsGridTile tile in GetCoveredTiles(body))
            {
                tile.Add(body);
            }
        }

        private List<PhysicsGridTile> GetCoveredTiles(CollisionBody toAdd)
        {
            int xMin = 0;
            int xMax = 0;
            int yMin = 0;
            int yMax = 0;

            if (toAdd.Shape == CollisionBody.ShapeType.Circle)
            {
                CircleBody body = (CircleBody)toAdd;
                xMin = ((int)(Math.Max(body.Position.X - body.Radius / 2, 0)) / tileSize);
                xMax = (int)(Math.Min(body.Position.X + body.Radius / 2, maxWidth - 1)) / tileSize;
                yMin = ((int)(Math.Max(body.Position.Y - body.Radius / 2, 0)) / tileSize);
                yMax = (int)(Math.Min(body.Position.Y + body.Radius / 2, maxHeight - 1)) / tileSize;
            }
            else if (toAdd.Shape == CollisionBody.ShapeType.Rectangle)
            {
                RectangleBody body = (RectangleBody)toAdd;
                xMin = ((int)(Math.Max(body.Position.X, 0)) / tileSize);
                xMax = (int)(Math.Min(body.Position.X + body.Size.X, maxWidth - 1)) / tileSize;
                yMin = ((int)(Math.Max(body.Position.Y, 0)) / tileSize);
                yMax = (int)(Math.Min(body.Position.Y + body.Size.Y, maxWidth - 1)) / tileSize;
            }

            List<PhysicsGridTile> toReturn = new List<PhysicsGridTile>();
            for (int y = yMin; y <= yMax; y++)
            {
                for (int x = xMin; x <= xMax; x++)
                {
                    toReturn.Add(Tiles[y * columns + x]);
                }
            }

            return toReturn;
        }

        public void Remove(CollisionBody body)
        {
            foreach (PhysicsGridTile tile in GetCoveredTiles(body))
            {
                tile.Remove(body.id);
            }
        }
    }
}
