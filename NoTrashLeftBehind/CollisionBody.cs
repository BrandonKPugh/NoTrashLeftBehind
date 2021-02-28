using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace NoTrashLeftBehind
{
    public abstract class CollisionBody
    {
        public enum ShapeType
        {
            Rectangle,
            Circle
        }
        public Object Parent;
        public Vector2 Position;
        public ShapeType Shape;
        public uint id;
        private static uint nextId = 0;
        public static Dictionary<uint, CollisionBody> bodies = new Dictionary<uint, CollisionBody>();

        public abstract Rectangle Region();

        public bool CollidesWith(CollisionBody other)
        {
            if (this.Shape == ShapeType.Circle && other.Shape == ShapeType.Circle)
            {
                CircleBody a = (CircleBody)this;
                CircleBody b = (CircleBody)other;
                return (Vector2.Distance(this.Position, other.Position) <= a.Radius + b.Radius);
            }
            else if (this.Shape == ShapeType.Circle && other.Shape == ShapeType.Rectangle)
            {
                return RectCircCollision((CircleBody)this, (RectangleBody)other);
            }
            else if (this.Shape == ShapeType.Rectangle && other.Shape == ShapeType.Circle)
            {
                return RectCircCollision((CircleBody)other, (RectangleBody)this);
            }
            else if (this.Shape == ShapeType.Rectangle && other.Shape == ShapeType.Rectangle)
            {
                RectangleBody a = (RectangleBody)this;
                RectangleBody b = (RectangleBody)other;

                return (a.Position.X <= b.Position.X + b.Size.X && a.Position.X + a.Size.X >= b.Position.X) &&
                    (a.Position.Y <= b.Position.Y + b.Size.Y && a.Position.Y + a.Size.Y >= b.Position.Y);
            }
            else
            {
                throw new NotImplementedException("Attempted collision with a shape that doesn't exist");
            }
        }

        public bool CollidesWith(Vector2 vector)
        {
            RectangleBody rect = new RectangleBody(vector, new Vector2(1, 1));
            return CollidesWith(rect);
        }

        private bool RectCircCollision(CircleBody a, RectangleBody b)
        {
            float nearestX = Clamp(a.Position.X, b.Position.X, b.Position.X + b.Size.X);
            float nearestY = Clamp(a.Position.Y, b.Position.Y, b.Position.Y + b.Size.Y);
            Vector2 nearestPoint = new Vector2(nearestX, nearestY);
            return (Vector2.Distance(nearestPoint, a.Position) <= a.Radius);
        }

        private float Clamp(float val, float min, float max)
        {
            return Math.Min(Math.Max(val, min), max);
        }

        public static uint NextId()
        {
            return nextId++;
        }

        public static CollisionBody GetById(uint id)
        {
            return bodies[id];
        }
    }
}
