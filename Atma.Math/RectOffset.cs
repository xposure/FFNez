
namespace Atma
{
    public struct RectOffset
    {
        //public vec2 offset;
        public vec2 min;
        public vec2 max;

        public RectOffset(vec2 min, vec2 max)
        {
            this.min = min;
            this.max = max;
        }

        public RectOffset(float left, float right, float top, float bottom)
        {
            this.min = new vec2(left, top);
            this.max = new vec2(right, bottom);
        }

        public float left { get { return min.x; } set { min.x = value; } }
        public float right { get { return max.x; } set { max.x = value; } }
        public float top { get { return min.y; } set { min.y = value; } }
        public float bottom { get { return max.y; } set { max.y = value; } }

        public float horizontal { get { return left + right; } }
        public float vertical { get { return top + bottom; } }

        public vec2 size { get { return new vec2(horizontal, vertical); } }
        //public vec2 right { get { return new vec2(max.X, 0); } }

        //public vec2 top { get { return new vec2(0, min.Y); } }
        //public vec2 bottom { get { return new vec2(0, max.Y); } }

        //public vec2 sizeX { get { return new vec2(min.X + max.X, 0); } }
        //public vec2 sizeY { get { return new vec2(0, min.Y + max.Y); } }
        //public float xWidth { get { return left.X + right.X; } }
        //public float yHeight { get { return top.Y + bottom.Y; } }

        //public vec2 size { get { return new vec2(xWidth, yHeight); } }

        //public AxisAlignedBox topLeft { get { return AxisAlignedBox.FromRect(vec2.Zero + offset, min); } }
        //public AxisAlignedBox left { get { return AxisAlignedBox.FromRect(new vec2(0, min.Y) vec2.Zero + offset, min); } }
        //public AxisAlignedBox add(AxisAlignedBox box)
        //{
        //    return new AxisAlignedBox(box.minVector , box.maxVector + min + max);
        //}
    }
}

